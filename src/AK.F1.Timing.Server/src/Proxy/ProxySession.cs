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
using System.Runtime.Serialization;
using System.Security.Authentication;
using System.Threading;
using AK.F1.Timing.Extensions;
using AK.F1.Timing.Proxy.Messages;
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
        /// <param name="cancellationToken">The session
        /// <see cref="System.Threading.CancellationToken"/>.</param>
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
                var login = ReadLogin();
                Proxy(login.Username, login.Password);
            }
            catch(Exception exc)
            {
                if(exc.IsFatal())
                {
                    throw;
                }
                if(!IsExpectedException(exc))
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

        private ServerLoginMessage ReadLogin()
        {
            using(var stream = CreateBufferedStream(128))
            using(var reader = new DecoratedObjectReader(stream))
            {
                return reader.Read<ServerLoginMessage>();
            }
        }

        private void Proxy(string username, string password)
        {
            Log.InfoFormat("username={0}", username);
            using(var stream = CreateBufferedStream())
            using(var writer = new DecoratedObjectWriter(stream))
            {
                using(var reader = WriteExceptions(writer, () =>
                    F1Timing.Live.Read(F1Timing.Live.Login(username, password))))
                {
                    while(!IsCancellationRequested)
                    {
                        var message = WriteExceptions(writer, () => reader.Read());
                        writer.WriteMessage(message);
                        stream.Flush();
                        if(message == null)
                        {
                            break;
                        }
                    }
                }
            }
        }

        private static T WriteExceptions<T>(DecoratedObjectWriter writer, Func<T> body)
        {
            try
            {
                return body();
            }
            catch(IOException exc)
            {
                writer.WriteMessage(new ServerExceptionMessage(exc));
                throw;
            }
            catch(SerializationException exc)
            {
                writer.WriteMessage(new ServerExceptionMessage(exc));
                throw;
            }
            catch(AuthenticationException exc)
            {
                writer.WriteMessage(new ServerExceptionMessage(exc));
                throw;
            }
        }

        private BufferedStream CreateBufferedStream(int bufferSize = 4096)
        {
            return new BufferedStream(new NetworkStream(_client, ownsSocket: false), bufferSize);
        }

        private static bool IsExpectedException(Exception exc)
        {
            return exc is IOException || exc is SerializationException || exc is AuthenticationException;
        }

        private bool IsCancellationRequested
        {
            get { return _cancellationToken.IsCancellationRequested; }
        }

        #endregion
    }
}
