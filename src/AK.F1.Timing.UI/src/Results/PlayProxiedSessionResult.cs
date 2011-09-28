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

using System.Net;
using AK.F1.Timing.UI.Services.Session;

namespace AK.F1.Timing.UI.Results
{
    /// <summary>
    /// A result which plays back a proxied live-timing session. This class cannot be inherited.
    /// </summary>
    public sealed class PlayProxiedSessionResult : PlaySessionResultBase
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="PlayProxiedSessionResult"/> class.
        /// </summary>
        /// <param name="address">The proxy address>.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="address"/> is <see langword="null"/>.
        /// </exception>
        public PlayProxiedSessionResult(IPAddress address)
        {
            Guard.NotNull(address, "address");

            Address = address;
        }

        #endregion

        #region Protected Interface.

        /// <inheritdoc/>
        protected override ISessionPlayer CreateSessionPlayer()
        {
            return new DefaultSessionPlayer(CreateReader);
        }

        #endregion

        #region Private Impl.

        private IMessageReader CreateReader()
        {
            return F1Timing.Proxy.Read(Address);
        }

        private IPAddress Address { get; set; }

        #endregion
    }
}