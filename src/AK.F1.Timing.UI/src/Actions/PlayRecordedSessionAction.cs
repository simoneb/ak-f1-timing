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
    /// An action which plays back a recorded live-timing session. This class cannot be inherited.
    /// </summary>
    public sealed class PlayRecordedSessionAction : PlaySessionActionBase
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="PlayRecordedSessionAction"/> class.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// <param name="tmsPath">The path of the timing message store to play.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="container"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <paramref name="tmsPath"/> is of zero length.
        /// </exception>
        public PlayRecordedSessionAction(string tmsPath) {

            Guard.NotNullOrEmpty(tmsPath, "tmsPath");

            this.TmsPath = tmsPath;
        }

        #endregion

        #region Protected Interface.

        /// <inheritdoc/>
        protected override ISessionPlayer CreateSessionPlayer() {

            var reader = F1Timing.Playback.Read(this.TmsPath);

            reader.PlaybackSpeed = 15d;

            return new DefaultSessionPlayer(reader);
        }

        #endregion

        #region Private Impl.

        private string TmsPath { get; set; }

        #endregion
    }
}
