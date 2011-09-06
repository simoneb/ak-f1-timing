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
using System.IO;
using System.Net.Sockets;
using System.Threading;
using AK.F1.Timing.Extensions;
using AK.F1.Timing.Serialization;
using AK.F1.Timing.Utility;
using log4net;

namespace AK.F1.Timing.Server.Proxy
{
    /// <summary>
    /// Proxies the <see cref="AK.F1.Timing.Message"/>s read from the live-timing service to the
    /// connected client. This class cannot be inherited.
    /// </summary>
    public sealed class ProxySession : DisposableBase
    {
        #region Fields.

        private readonly Socket _client;
        private readonly CancellationToken _cancellationToken;

        private const int MaxAuthenticationTokenLength = 4096;
        private static readonly ILog Log = LogManager.GetLogger(typeof(ProxySession));

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="ProxySession"/> class and specifies
        /// the <paramref name="client"/> and the session cancellation token.
        /// </summary>
        /// <param name="client">The client <see cref="System.Net.Sockets.Socket"/>.</param>
        /// <param name="cancellationToken">The session <see cref="System.Threading.CancellationToken"/>.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="client"/> is <see langword="null"/>.
        /// </exception>
        public ProxySession(Socket client, CancellationToken cancellationToken)
        {
            Guard.NotNull(client, "client");

            _client = client;
            _cancellationToken = cancellationToken;
        }

        /// <summary>
        /// Runs the session.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the handler has been disposed of.
        /// </exception>
        public void Run()
        {
            CheckDisposed();
            try
            {
                RunCore();
            }
            catch(Exception exc)
            {
                if(exc.IsFatal())
                {
                    throw;
                }
                if(!(exc is IOException))
                {
                    Log.Error(exc);
                }
            }
            finally
            {
                ((IDisposable)this).Dispose();
            }
        }

        #endregion

        #region Protected Interface.

        /// <inheritdoc/>
        protected override void DisposeOfManagedResources()
        {
            DisposeOf(_client);
        }

        #endregion

        #region Private Impl.

        private void RunCore()
        {
            AuthenticationToken token;
            if(!TryReadAuthenticationToken(out token))
            {
                return;
            }
            using(var messageReader = F1Timing.Live.Read(token))
            using(var networkStream = new NetworkStream(_client))
            using(var bufferedStream = new BufferedStream(networkStream))
            using(var messageWriter = new DecoratedObjectWriter(bufferedStream))
            {
                Message message;
                while((message = messageReader.Read()) != null)
                {
                    if(IsCancellationRequested)
                    {
                        return;
                    }
                    messageWriter.WriteMessage(message);
                    bufferedStream.Flush();
                }
                messageWriter.Write(null);
            }
        }

        private bool TryReadAuthenticationToken(out AuthenticationToken token)
        {
            using(var input = new NetworkStream(_client))
            using(var reader = new BinaryReader(input))
            {
                int length = reader.ReadInt32();
                if(length > MaxAuthenticationTokenLength)
                {
                    Log.WarnFormat("max authentication token length exceeded, max={0}, received={1}",
                        MaxAuthenticationTokenLength, length);
                    token = null;
                    return false;
                }
                token = new AuthenticationToken(new string(reader.ReadChars(length)));
                return true;
            }
        }

        private bool IsCancellationRequested
        {
            get { return _cancellationToken.IsCancellationRequested; }
        }

        #endregion
    }
}
