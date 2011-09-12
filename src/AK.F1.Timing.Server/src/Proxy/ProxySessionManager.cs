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

        private readonly string _username;
        private readonly string _password;

        private readonly CancellationToken _cancellationToken;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private Task _dispatchMessagesTask;
        private readonly ManualResetEventSlim _dispatchMessagesCompleteEvent = new ManualResetEventSlim();
        // 1 MiB is sufficient; the largest TMS, as of 2011-09-11, is 1073989 bytes (2011\07-canada).
        private readonly ByteBuffer _messageHistory = new ByteBuffer(1024 * 1024);
        private readonly ReaderWriterLockSlim _messageHistoryLock = new ReaderWriterLockSlim();

        private Task _readMessagesTask;
        // TODO consider specifying a bounded capacity.
        private readonly BlockingCollection<byte[]> _readMessageQueue = new BlockingCollection<byte[]>();

        private int _nextSessionId;
        private readonly IDictionary<int, ProxySession> _sessions = new Dictionary<int, ProxySession>();
        private readonly ReaderWriterLockSlim _sessionsLock = new ReaderWriterLockSlim();
        private readonly ManualResetEventSlim _sessionsEmptyEvent = new ManualResetEventSlim(true);

        private static readonly ILog Log = LogManager.GetLogger(typeof(ProxySessionManager));

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initilises a new instance of the <see cref="AK.F1.Timing.Server.Proxy.ProxySessionManager"/>
        /// class and specifies the <paramref name="username"/> and <paramref name="password"/> of the
        /// user to autenticate as.
        /// </summary>
        /// <param name="username">The user's live-timing username.</param>
        /// <param name="password">The user's live-timing password.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="username"/> or <paramref name="password"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <paramref name="username"/> or <paramref name="password"/> is empty.
        /// </exception>
        public ProxySessionManager(string username, string password)
        {
            Guard.NotNullOrEmpty(username, "username");
            Guard.NotNullOrEmpty(password, "password");

            _username = username;
            _password = password;
            _cancellationToken = _cancellationTokenSource.Token;
            _dispatchMessagesTask = new Task(DispatchMessages, _cancellationToken);
            _dispatchMessagesTask.Start();
            _readMessagesTask = new Task(ReadMessages, _cancellationToken);
            _readMessagesTask.Start();
        }

        /// <inheritdoc/>
        public void Handle(Socket client)
        {
            CheckDisposed();
            Guard.NotNull(client, "client");

            StartSession(client);
        }

        #endregion

        #region Protected Interface.

        /// <inheritdoc/>
        protected override void DisposeOfManagedResources()
        {
            Log.Info("stopping");
            _cancellationTokenSource.Cancel();
            Task.WaitAll(_dispatchMessagesTask, _readMessagesTask);
            if(!_sessionsEmptyEvent.IsSet)
            {
                Log.Info("stopping sessions");
                ForEachSession(DisposeOf, throwIfCancellationRequested: false);
                _sessionsEmptyEvent.Wait();
            }
            DisposeOf(_cancellationTokenSource);
            DisposeOf(_dispatchMessagesCompleteEvent);
            DisposeOf(_dispatchMessagesTask);
            DisposeOf(_messageHistoryLock);
            DisposeOf(_readMessagesTask);
            DisposeOf(_readMessageQueue);
            DisposeOf(_sessionsEmptyEvent);
            DisposeOf(_sessionsLock);
            Log.Info("stopped");
        }

        #endregion

        #region Private Impl.

        private void ReadMessages()
        {
            Log.Info("read task started");
            try
            {
                //using(var reader = F1Timing.Live.Read(F1Timing.Live.Login(_username, _password)))
                using(var reader = F1Timing.Playback.Read(@"proxy.tms"))
                using(var buffer = new MemoryStream(4096))
                using(var writer = new DecoratedObjectWriter(buffer))
                {
                    reader.PlaybackSpeed = 5.0;
                    Message message;
                    do
                    {
                        ThrowIfCancellationRequested();
                        message = reader.Read();
                        buffer.SetLength(0L);
                        writer.WriteMessage(message);
                        _readMessageQueue.Add(buffer.ToArray());
                    } while(message != null);
                }
            }
            catch(OperationCanceledException) { }
            catch(Exception exc)
            {
                Log.Fatal(exc);
            }
            finally
            {
                _readMessageQueue.CompleteAdding();
                Log.Info("read task stopped");
            }
        }

        private void DispatchMessages()
        {
            Log.Info("dispatch task started");
            try
            {
                byte[] message;
                // Prevent the session queues from growing unwieldy by an placing arbitrary upper bound
                // on the capacity of the dispatch queue.
                const int dispatchQueueCapacity = 32;
                var dispatchQueue = new Queue<byte[]>(dispatchQueueCapacity);
                while(_readMessageQueue.TryTake(out message, Timeout.Infinite, _cancellationToken))
                {
                    dispatchQueue.Clear();
                    _messageHistoryLock.InWriteLock(() =>
                    {
                        do
                        {
                            dispatchQueue.Enqueue(message);
                            _messageHistory.Append(message);
                        } while(dispatchQueue.Count < dispatchQueueCapacity &&
                            _readMessageQueue.TryTake(out message, 0, _cancellationToken));
                        ForEachSession(session => session.SendAsync(dispatchQueue));
                    });
                }
                _dispatchMessagesCompleteEvent.Set();
                ForEachSession(session => session.CompleteAsync());
            }
            catch(OperationCanceledException) { }
            catch(Exception exc)
            {
                Log.Fatal(exc);
            }
            finally
            {
                _dispatchMessagesCompleteEvent.Set();
                Log.Info("dispatch task stopped");
            }
        }

        private void StartSession(Socket client)
        {
            var session = CreateSession(client);
            // Lock order is critical to prevent duplicate message buffers from being sent.
            _messageHistoryLock.InReadLock(() =>
            {
                _sessionsLock.InWriteLock(() =>
                {
                    _sessions.Add(session.Id, session);
                    session.SendAsync(_messageHistory.CreateSnapshot());
                    if(_dispatchMessagesCompleteEvent.IsSet)
                    {
                        session.CompleteAsync();
                    }
                    Log.InfoFormat("started, id={0}, endpoint={1}, open={2}",
                        session.Id, client.RemoteEndPoint, _sessions.Count);
                    _sessionsEmptyEvent.Reset();
                });
            });
        }

        private ProxySession CreateSession(Socket client)
        {
            var session = new ProxySession(NextSessionId(), client);
            session.Disposed += OnSessionDisposed;
            return session;
        }

        private void OnSessionDisposed(object sender, EventArgs e)
        {
            var session = (ProxySession)sender;
            session.Disposed -= OnSessionDisposed;
            _sessionsLock.InWriteLock(() =>
            {
                _sessions.Remove(session.Id);
                Log.InfoFormat("removed, id={0}, open={1}", session.Id, _sessions.Count);
                if(_sessions.Count == 0)
                {
                    _sessionsEmptyEvent.Set();
                }
            });
        }

        private void ForEachSession(Action<ProxySession> action, bool throwIfCancellationRequested = true)
        {
            _sessionsLock.InReadLock(() =>
            {
                foreach(var session in _sessions.Values)
                {
                    if(throwIfCancellationRequested)
                    {
                        ThrowIfCancellationRequested();
                    }
                    try
                    {
                        action(session);
                    }
                    catch(ObjectDisposedException)
                    {
                        // The session may have been disposed of whilst we hold the read lock and
                        // therefore couldn't remove itself from the collection.
                    }
                }
            });
        }

        private void ThrowIfCancellationRequested()
        {
            _cancellationToken.ThrowIfCancellationRequested();
        }

        private int NextSessionId()
        {
            // TODO not sure what to do on overflow, seems unlikely...
            return Interlocked.Increment(ref _nextSessionId);
        }

        #endregion
    }
}
