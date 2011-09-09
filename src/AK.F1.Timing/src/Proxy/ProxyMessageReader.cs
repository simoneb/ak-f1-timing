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
using AK.F1.Timing.Proxy.Messages;
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
        private readonly string _username;
        private readonly string _password;

        /// <summary>
        /// Defines the default proxy port. This field is constant.
        /// </summary>
        public const int DefaultPort = 50192;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="ProxyMessageReader"/> class and specifies
        /// the proxy <paramref name="endpoint"/> and the user's authentication token.
        /// </summary>
        /// <param name="endpoint">The proxy endpoint.</param>
        /// <param name="username">The user's F1 live-timing username.</param>
        /// <param name="password">The user's F1 live-timing password.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="endpoint"/>, <paramref name="username"/> or
        /// <paramref name="password"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <paramref name="username"/> or <paramref name="password"/> is empty.
        /// </exception>
        public ProxyMessageReader(IPEndPoint endpoint, string username, string password)
        {
            Guard.NotNull(endpoint, "endpoint");
            Guard.NotNullOrEmpty(username, "username");
            Guard.NotNullOrEmpty(password, "password");

            _endpoint = endpoint;
            _username = username;
            _password = password;
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
            var message = _reader.Read<Message>();
            var exceptionMessage = message as ServerExceptionMessage;
            if(exceptionMessage != null)
            {
                exceptionMessage.ThrowException();
            }
            return message;
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
            WriterServerLoginMessage();
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
            _reader = new DecoratedObjectReader(CreateBufferedStream());
        }

        private void WriterServerLoginMessage()
        {
            Log.InfoFormat("authenticating: {0}", _username);
            using(var buffer = CreateBufferedStream(128))
            using(var writer = new DecoratedObjectWriter(buffer))
            {
                writer.WriteMessage(new ServerLoginMessage(_username, _password));
            }
        }

        private BufferedStream CreateBufferedStream(int bufferSize = 4096)
        {
            return new BufferedStream(new NetworkStream(_socket, ownsSocket: false), bufferSize);
        }

        #endregion
    }
}