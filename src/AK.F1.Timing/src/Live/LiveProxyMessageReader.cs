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

namespace AK.F1.Timing.Live
{
    /// <summary>
    /// An <see cref="AK.F1.Timing.IMessageReader"/> which reads <see cref="AK.F1.Timing.Message"/>s
    /// from an enpoint which itself proxies messages read from the live-timing service. This class
    /// cannot be inherited.
    /// </summary>
    public sealed class LiveProxyMessageReader : MessageReaderBase
    {
        #region Fields.

        private Socket _socket;
        private IObjectReader _reader;
        private readonly IPEndPoint _endpoint;
        private readonly AuthenticationToken _authenticationToken;

        /// <summary>
        /// Defines the default proxy port. This field is constant.
        /// </summary>
        public const int DefaultPort = 50192;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="LiveProxyMessageReader"/> class and specifies
        /// the proxy <paramref name="endpoint"/> and the user's authentication token.
        /// </summary>
        /// <param name="endpoint">The proxy endpoint.</param>
        /// <param name="authenticationToken">The user's authentication token.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="endpoint"/> or <paramref name="authenticationToken"/> is
        /// <see langword="null"/>.
        /// </exception>
        public LiveProxyMessageReader(IPEndPoint endpoint, AuthenticationToken authenticationToken)
        {
            Guard.NotNull(endpoint, "endpoint");
            Guard.NotNull(authenticationToken, "token");

            _endpoint = endpoint;
            _authenticationToken = authenticationToken;
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
            WriteAuthenticationToken();
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
                throw Guard.LiveProxyMessageReader_FailedToConnect(exc);
            }
        }

        private void InitialiseMessageReader()
        {
            _reader = new DecoratedObjectReader(new BufferedStream(new NetworkStream(_socket)));
        }

        private void WriteAuthenticationToken()
        {
            Log.InfoFormat("writing authentication token: {0}", _authenticationToken.Token);
            using(var stream = new NetworkStream(_socket, false))
            using(var writer = new BinaryWriter(stream))
            {
                // We do not use WriteString as the server validates the string length is sane.
                writer.Write(_authenticationToken.Token.Length);
                writer.Write(_authenticationToken.Token.ToCharArray());
            }
        }

        #endregion
    }
}