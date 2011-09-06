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
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using AK.F1.Timing.Serialization;
using AK.F1.Timing.Utility;
using log4net;

namespace AK.F1.Timing.Server.Live
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class LiveSocketHandler : DisposableBase, ISocketHandler
    {
        #region Fields.

        private readonly Task _readerTask;
        private readonly Task _dispatcherTask;
        private readonly IMessageReader _messageReader;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly IList<byte[]> _messageHistory = new List<byte[]>(10000);
        private readonly SemaphoreSlim _messageReadyEvent = new SemaphoreSlim(1);
        private readonly LinkedList<Session> _sessions = new LinkedList<Session>();
        private readonly ReaderWriterLockSlim _sessionsLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        private static readonly ILog Log = LogManager.GetLogger(typeof(LiveSocketHandler));

        #endregion

        #region Public Interface.

        public LiveSocketHandler(AuthenticationToken token)
        {
            Guard.NotNull(token, "token");

            _messageReader = F1Timing.Live.Read(token);
            _cancellationTokenSource = new CancellationTokenSource();
            _dispatcherTask = new Task(DispatchMessages);
            _dispatcherTask.ContinueWith(_ => DisposeOf(this));
            _dispatcherTask.Start();
            _readerTask = new Task(ReadMessages);
            _readerTask.ContinueWith(_ => DisposeOf(this));
            _readerTask.Start();
        }

        /// <inheritdoc/>
        public void Handle(Socket client)
        {
            Guard.NotNull(client, "client");
        }


        #endregion

        #region Protected Interface.

        /// <inheritdoc/>
        protected override void DisposeOfManagedResources()
        {
            _cancellationTokenSource.Cancel();
            Task.WaitAll(_dispatcherTask, _readerTask);
            DisposeOf(_cancellationTokenSource);
            DisposeOf(_dispatcherTask);
            DisposeOf(_messageReader);
            DisposeOf(_messageReadyEvent);
            DisposeOf(_readerTask);
            DisposeOf(_sessionsLock);
        }

        #endregion

        #region Private Impl.

        private void DispatchMessages()
        {
            var cancellationToken = _cancellationTokenSource.Token;
            try
            {
                while(true)
                {
                    _messageReadyEvent.Wait(cancellationToken);
                    _sessionsLock.EnterReadLock();
                    try
                    {
                        foreach(var session in _sessions)
                        {
                            //DispatchMessage(session, 
                        }
                    }
                    finally
                    {
                        _sessionsLock.ExitReadLock();
                    }
                }
            }
            catch(Exception exc)
            {
                if(!(exc is OperationCanceledException))
                {
                    Log.Fatal(exc);
                }
            }
        }

        private void ReadMessages()
        {
            var cancellationToken = _cancellationTokenSource.Token;
            try
            {
                using(var memoryStream = new MemoryStream())
                using(var messageWriter = new DecoratedObjectWriter(memoryStream))
                {
                    while(true)
                    {
                        var message = _messageReader.Read();
                        if(cancellationToken.IsCancellationRequested)
                        {
                            break;
                        }
                        memoryStream.SetLength(0L);
                        messageWriter.WriteMessage(message);
                        _messageHistory.Add(memoryStream.ToArray());
                        _messageReadyEvent.Release();
                    }
                }
            }
            catch(Exception exc)
            {
                if(!(exc is OperationCanceledException))
                {
                    Log.Fatal(exc);
                }
            }
        }

        private class Session { }



        #endregion
    }
}
