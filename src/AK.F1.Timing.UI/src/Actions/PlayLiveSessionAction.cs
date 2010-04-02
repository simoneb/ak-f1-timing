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
using Microsoft.Practices.ServiceLocation;

using AK.F1.Timing.UI.Services.Session;

namespace AK.F1.Timing.UI.Actions
{
    /// <summary>
    /// An action which plays back the current live-timing session. This class cannot be inherited.
    /// </summary>
    public sealed class PlayLiveSessionAction : PlaySessionActionBase
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="PlayLiveSessionAction"/> class.
        /// </summary>
        /// <param name="tmsPath">The user's <see cref="AuthenticationToken"/>.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="token"/> is <see langword="null"/>.
        /// </exception>
        public PlayLiveSessionAction(AuthenticationToken token) {

            Guard.NotNull(token, "token");

            this.Token = token;
        }

        #endregion

        #region Protected Interface.

        /// <inheritdoc/>
        protected override ISessionPlayer CreateSessionPlayer() {

            return new DefaultSessionPlayer(F1Timing.Live.Read(this.Token));
        }

        #endregion

        #region Private Impl.

        private AuthenticationToken Token { get; set; }

        #endregion
    }
}
