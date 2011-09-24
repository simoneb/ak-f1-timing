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
using System.Net;
using System.Net.Sockets;
using System.Threading;
using AK.F1.Timing.Utility;
using log4net;

namespace AK.F1.Timing.Server
{
    /// <summary>
    /// A simple TCP server which hands connections to a <see cref="AK.F1.Timing.Server.ISocketHandler"/>.
    /// This class cannot be inherited.
    /// </summary>
    [Serializable]
    public sealed class TcpServer : DisposableBase
    {
        #region Fields.

        private int _started;
        private readonly IPEndPoint _endpoint;
        private readonly int _connectionBacklog;
        private volatile ISocketHandler _handler;
        private volatile Socket _listener;
        private volatile SocketAsyncEventArgs _acceptEvent;

        private static readonly ILog Log = LogManager.GetLogger(typeof(TcpServer));

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="AK.F1.Timing.Server.TcpServer"/> class.
        /// </summary>
        /// <param name="endpoint">The remote end point to listen on.</param>
        /// <param name="handler">The client connection handler.</param>
        /// <param name="backlog">The accept connection backlog.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="endpoint"/> or <paramref name="handler"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="backlog"/> is not positive.
        /// </exception>
        public TcpServer(IPEndPoint endpoint, ISocketHandler handler, int backlog = 50)
        {
            Guard.NotNull(endpoint, "endpoint");
            Guard.NotNull(handler, "handler");
            Guard.InRange(backlog > 0, "backlog");

            _endpoint = endpoint;
            _handler = handler;
            _connectionBacklog = backlog;
        }

        /// <summary>
        /// Starts the server accepting incoming connection requests.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when this instance has been disposed of.
        /// </exception>
        /// <exception cref="System.Security.SecurityException">
        /// A caller higher in the call stack does not have permission for the requested operation.
        /// </exception>
        /// <exception cref="System.Net.Sockets.SocketException">
        /// Thrown when an error occurred when attempting to bind the underlying socket.
        /// </exception>
        public void Start()
        {
            CheckDisposed();
            if(Interlocked.CompareExchange(ref _started, 1, 0) == 0)
            {
                Log.Info("starting");
                InitialiseListener();
                InitialiseAcceptEvent();
                AcceptAsync();
                Log.InfoFormat("started, endpoint={0}", _endpoint);
            }
        }

        #endregion

        #region Protected Interface.

        /// <inheritdoc/>
        protected override void DisposeOfManagedResources()
        {
            Log.Info("stopping");
            DisposeOfListener();
            DisposeOfAcceptEvent();
            DisposeOfHandler();
            Log.Info("stopped");
        }

        #endregion

        #region Private Impl.

        private void DisposeOfHandler()
        {
            DisposeOf(_handler);
            _handler = null;
        }

        private void InitialiseAcceptEvent()
        {
            _acceptEvent = new SocketAsyncEventArgs();
            _acceptEvent.Completed += OnAcceptEventCompleted;
        }

        private void DisposeOfAcceptEvent()
        {
            if(_acceptEvent != null)
            {
                _acceptEvent.Completed -= OnAcceptEventCompleted;
                DisposeOf(_acceptEvent);
                _acceptEvent = null;
            }
        }

        private void InitialiseListener()
        {
            _listener = new Socket(_endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _listener.Bind(_endpoint);
            _listener.Listen(_connectionBacklog);
        }

        private void DisposeOfListener()
        {
            DisposeOf(_listener);
            _listener = null;
        }

        private void AcceptAsync()
        {
            var listener = _listener;
            var acceptEvent = _acceptEvent;
            if(listener != null && acceptEvent != null && !listener.AcceptAsync(acceptEvent))
            {
                OnAcceptEventCompleted(listener, acceptEvent);
            }
        }

        private void OnAcceptEventCompleted(object sender, SocketAsyncEventArgs e)
        {
            if(e.SocketError == SocketError.OperationAborted)
            {
                return;
            }
            if(e.SocketError != SocketError.Success)
            {
                Log.ErrorFormat("accept failed, error={0}", e.SocketError);
                Dispose();
                return;
            }
            var handler = _handler;
            if(handler == null)
            {
                DisposeOf(e.AcceptSocket);
                return;
            }
            try
            {
                handler.Handle(e.AcceptSocket);
                e.AcceptSocket = null;
                AcceptAsync();
            }
            catch(Exception exc)
            {
                Log.Fatal(exc);
                DisposeOf(e.AcceptSocket);
                Dispose();
            }
        }

        private void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
