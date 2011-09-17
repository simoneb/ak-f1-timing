// Copyright 2011 Andy Kernahan
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using AK.F1.Timing.Serialization;
using AK.F1.Timing.Server.Extensions;
using AK.F1.Timing.Server.IO;
using AK.F1.Timing.Server.Threading;
using AK.F1.Timing.Utility;
using log4net;

namespace AK.F1.Timing.Server.Proxy
{
    /// <summary>
    /// A <see cref="AK.F1.Timing.Server.ISocketHandler"/> which creates and manages a
    /// <see cref="AK.F1.Timing.Server.Proxy.ProxySession"/> for each connected client.
    /// Connected client are first sent all historial mesages from current active session
    /// and then go onto receive instantaneous updates. This class cannot be inherited.
    /// </summary>
    /// <threadsafety static="true" instance="true"/>
    public sealed class ProxySessionManager : DisposableBase, ISocketHandler
    {
        #region Fields.

        private readonly CancellationToken _cancellationToken;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private Task _mainTask;
        private readonly AutoResetEventSlim _wakeUpMainTask = new AutoResetEventSlim();

        // Prevent the session queues from growing unwieldy by an placing arbitrary upper bound
        // on the capacity of the message dispatch queue. This has the effect of limiting the
        // number of messages processsed per cycle when there are active sessions.
        private const int DispatchMessagesCapacity = 32;
        private readonly Queue<byte[]> _dispatchMessages = new Queue<byte[]>(DispatchMessagesCapacity);
        // 1 MiB is sufficient; largest TMS, as of 2011-09-11, is 1073989 bytes (2011\07-canada\race.tms).
        private readonly ByteBuffer _dispatchedMessageHistory = new ByteBuffer(1024 * 1024);

        private int _nextSessionId;
        private readonly IDictionary<int, ProxySession> _sessions = new Dictionary<int, ProxySession>();
        private readonly BlockingCollection<ProxySession> _sessionsPendingStart = new BlockingCollection<ProxySession>();
        private readonly BlockingCollection<ProxySession> _sessionsPendingRemove = new BlockingCollection<ProxySession>();

        private Task _readMessagesTask;
        private readonly IMessageReader _messageReader;
        private readonly BlockingCollection<byte[]> _pendingMessages = new BlockingCollection<byte[]>();

        private static readonly ILog Log = LogManager.GetLogger(typeof(ProxySessionManager));

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initilises a new instance of the <see cref="AK.F1.Timing.Server.Proxy.ProxySessionManager"/>
        /// class and specified the source <see cref="AK.F1.Timing.IMessageReader"/>.
        /// </summary>
        /// <param name="reader">The source <see cref="AK.F1.Timing.IMessageReader"/>.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>        
        public ProxySessionManager(IMessageReader reader)
        {
            Guard.NotNull(reader, "reader");

            _messageReader = reader;
            _cancellationToken = _cancellationTokenSource.Token;
            _readMessagesTask = new Task(ReadMessagesTask, _cancellationToken);
            _readMessagesTask.ContinueFaultWith(_ => Dispose());
            _readMessagesTask.Start();
            _mainTask = new Task(MainTask, _cancellationToken);
            _mainTask.ContinueFaultWith(_ => Dispose());
            _mainTask.Start();
        }

        /// <inheritdoc/>
        public void Handle(Socket client)
        {
            CheckDisposed();
            Guard.NotNull(client, "client");

            AddPendingStart(client);
        }

        #endregion

        #region Protected Interface.

        /// <inheritdoc/>
        protected override void DisposeOfManagedResources()
        {
            Log.Info("stopping tasks");
            _cancellationTokenSource.Cancel();
            try
            {
                Task.WaitAll(_mainTask, _readMessagesTask);
            }
            catch(AggregateException) { }
            Log.Info("stopping sessions");
            _sessionsPendingStart.ForEach(DisposeOf);
            ForEachSession(DisposeOf, throwIfCancellationRequested: false);
            ProcessPendingRemoves();
            DisposeOf(_cancellationTokenSource);
            DisposeOf(_mainTask);
            DisposeOf(_messageReader);
            DisposeOf(_readMessagesTask);
            DisposeOf(_pendingMessages);
            DisposeOf(_sessionsPendingStart);
            DisposeOf(_sessionsPendingRemove);
            DisposeOf(_wakeUpMainTask);
            Log.Info("stopped");
        }

        #endregion

        #region Private Impl.

        private void ReadMessagesTask()
        {
            Log.Info("read task started");
            try
            {
                using(var buffer = new MemoryStream(4096))
                using(var writer = new DecoratedObjectWriter(buffer))
                {
                    Message message;
                    do
                    {
                        ThrowIfCancellationRequested();
                        message = _messageReader.Read();
                        buffer.SetLength(0L);
                        writer.WriteMessage(message);
                        _pendingMessages.Add(buffer.ToArray());
                        _wakeUpMainTask.Set();
                    } while(message != null);
                    _pendingMessages.CompleteAdding();
                    _wakeUpMainTask.Set();
                }
                Log.InfoFormat("completed reading messages");
            }
            catch(OperationCanceledException) { }
            catch(Exception exc)
            {
                Log.Fatal(exc);
                throw;
            }
            finally
            {
                Log.Info("read task stopped");
            }
        }

        private void MainTask()
        {
            Log.Info("main task started");
            try
            {
                var completedActiveSessions = false;
                while(!_cancellationToken.IsCancellationRequested)
                {
                    _wakeUpMainTask.Wait(_cancellationToken);
                    ProcessPendingRemoves();
                    if(!_pendingMessages.IsCompleted)
                    {
                        ProcessPendingMessages();
                    }
                    else if(!completedActiveSessions)
                    {
                        ForEachSession(session => session.CompleteAsync());
                        completedActiveSessions = true;
                    }
                    ProcessPendingStarts(_pendingMessages.IsCompleted);
                }
            }
            catch(OperationCanceledException) { }
            catch(Exception exc)
            {
                Log.Fatal(exc);
                throw;
            }
            finally
            {
                Log.Info("main task stopped");
            }
        }

        private void ProcessPendingMessages()
        {
            if(_pendingMessages.Count == 0)
            {
                return;
            }
            byte[] message;
            while(_dispatchMessages.Count < DispatchMessagesCapacity &&
                _pendingMessages.TryTake(out message, 0, _cancellationToken))
            {
                if(_sessions.Count > 0)
                {
                    _dispatchMessages.Enqueue(message);
                }
                _dispatchedMessageHistory.Append(message);
            }
            if(_dispatchMessages.Count > 0)
            {
                ForEachSession(session =>
                {
                    session.SendAsync(_dispatchMessages);
                    if(_pendingMessages.IsCompleted)
                    {
                        session.CompleteAsync();
                    }
                });
                _dispatchMessages.Clear();
            }
        }

        private void AddPendingStart(Socket client)
        {
            var session = new ProxySession(NextSessionId(), client);
            session.Disposed += OnSessionDisposed;
            _sessionsPendingStart.Add(session);
            _wakeUpMainTask.Set();
        }

        private void ProcessPendingStarts(bool completeSessions)
        {
            if(_sessionsPendingStart.Count == 0)
            {
                return;
            }
            ProxySession session;
            while(_sessionsPendingStart.TryTake(out session, 0, _cancellationToken))
            {
                if(session.IsDisposed)
                {
                    continue;
                }
                try
                {
                    session.SendAsync(_dispatchedMessageHistory.CreateSnapshot());
                    if(completeSessions)
                    {
                        session.CompleteAsync();
                    }
                    _sessions.Add(session.Id, session);
                    Log.InfoFormat("started, id={0}, open={1}", session.Id, _sessions.Count);
                }
                catch(ObjectDisposedException) { }
            }
        }

        private void OnSessionDisposed(object sender, EventArgs e)
        {
            var session = (ProxySession)sender;
            session.Disposed -= OnSessionDisposed;
            _sessionsPendingRemove.Add(session);
            _wakeUpMainTask.Set();
        }

        private void ProcessPendingRemoves()
        {
            if(_sessionsPendingRemove.Count == 0)
            {
                return;
            }
            ProxySession session;
            while(_sessionsPendingRemove.TryTake(out session, 0))
            {
                _sessions.Remove(session.Id);
                Log.InfoFormat("removed, id={0}, open={1}", session.Id, _sessions.Count);
            }
        }

        private void ForEachSession(Action<ProxySession> action, bool throwIfCancellationRequested = true)
        {
            foreach(var session in _sessions.Values)
            {
                if(throwIfCancellationRequested)
                {
                    ThrowIfCancellationRequested();
                }
                if(session.IsDisposed)
                {
                    continue;
                }
                try
                {
                    action(session);
                }
                catch(ObjectDisposedException) { }
            }
        }

        private void ThrowIfCancellationRequested()
        {
            _cancellationToken.ThrowIfCancellationRequested();
        }

        private int NextSessionId()
        {
            return Interlocked.Increment(ref _nextSessionId);
        }

        private void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
