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

using System.IO;
using System.Net;
using System.Net.Sockets;
using AK.F1.Timing.Serialization;

namespace AK.F1.Timing.Proxy
{
    /// <summary>
    /// An <see cref="AK.F1.Timing.IMessageReader"/> which reads <see cref="AK.F1.Timing.Message"/>s
    /// from an enpoint which itself proxies messages read from the live-timing service. This class
    /// cannot be inherited.
    /// </summary>
    public sealed class ProxyMessageReader : MessageReaderBase
    {
        #region Fields.

        private Socket _socket;
        private IObjectReader _reader;
        private readonly IPEndPoint _endpoint;

        /// <summary>
        /// Defines the default proxy port. This field is constant.
        /// </summary>
        public const int DefaultPort = 50192;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="ProxyMessageReader"/> class and specifies
        /// the proxy <paramref name="endpoint"/>.
        /// </summary>
        /// <param name="endpoint">The proxy endpoint.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="endpoint"/> is <see langword="null"/>.
        /// </exception>        
        public ProxyMessageReader(IPEndPoint endpoint)
        {
            Guard.NotNull(endpoint, "endpoint");

            _endpoint = endpoint;
        }

        #endregion

        #region Protected Interface.

        /// <inheritdoc/>        
        protected override Message ReadImpl()
        {
            if(_reader == null)
            {
                Initialise();
            }
            return _reader.Read<Message>();
        }

        /// <inheritdoc/>
        protected override void DisposeOfManagedResources()
        {
            DisposeOf(_socket);
            DisposeOf(_reader);
        }

        #endregion

        #region Private Impl.

        private void Initialise()
        {
            InitialiseSocket();
            InitialiseMessageReader();
        }

        private void InitialiseSocket()
        {
            try
            {
                Log.Info("connecting");
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.Connect(_endpoint);
            }
            catch(SocketException exc)
            {
                DisposeOf(_socket);
                throw Guard.ProxyMessageReader_FailedToConnect(exc);
            }
        }

        private void InitialiseMessageReader()
        {
            _reader = new DecoratedObjectReader(new BufferedStream(new NetworkStream(_socket, ownsSocket: false)));
        }

        #endregion
    }
}