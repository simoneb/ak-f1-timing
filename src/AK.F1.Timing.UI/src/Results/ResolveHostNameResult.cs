// Copyright 2010 Andy Kernahan
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
using Caliburn.PresentationFramework.Invocation;
using Caliburn.PresentationFramework.RoutedMessaging;

namespace AK.F1.Timing.UI.Results
{
    /// <summary>
    /// A result which resolves a host name. This class cannot be inherited.
    /// </summary>
    public sealed class ResolveHostNameResult : IResult
    {
        #region Fields.

        private readonly string _hostname;
        private IPAddress _address;

        #endregion

        #region Public Interface.

        /// <inheritdoc/>
        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

        /// <summary>
        /// Initialises a new instance of the <see cref="ResolveHostNameResult"/> class.
        /// </summary>
        /// <param name="hostname">The hostname to resolve.</param>        
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="hostname"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <paramref name="hostname"/> is empty.
        /// </exception>
        public ResolveHostNameResult(string hostname)
        {
            Guard.NotNullOrEmpty(hostname, "hostnameOrAddress");

            _hostname = hostname;
        }

        /// <inheritdoc/>
        public void Execute(ResultExecutionContext context)
        {
            Guard.NotNull(context, "context");

            var dispatcher = context.ServiceLocator.GetInstance<IDispatcher>();

            dispatcher.ExecuteOnBackgroundThread(() =>
            {
                try
                {
                    var entry = Dns.GetHostEntry(_hostname);
                    if(entry.AddressList == null || entry.AddressList.Length == 0)
                    {
                        throw new SocketException((int)SocketError.HostNotFound);
                    }
                    _address = entry.AddressList[0];
                }
                catch(Exception exc)
                {
                    Exception = exc;
                }
            }, delegate { Completed(this, new ResultCompletionEventArgs()); }, null);
        }

        /// <summary>
        /// Returns a value indicating if the action completed successfully.
        /// </summary>
        public bool Success
        {
            get { return Exception == null; }
        }

        /// <summary>
        /// Gets the <see cref="System.Net.IPAddress"/>.
        /// </summary>
        public IPAddress Address
        {
            get { return _address; }
        }

        /// <summary>
        /// Gets the <see cref="System.Exception"/> that was thrown.
        /// </summary>
        public Exception Exception { get; private set; }

        #endregion
    }
}