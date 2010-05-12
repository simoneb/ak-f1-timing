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

using AK.F1.Timing.UI.Services.Session;

namespace AK.F1.Timing.UI.Results
{
    /// <summary>
    /// A result which plays back a recorded live-timing session. This class cannot be inherited.
    /// </summary>
    public sealed class PlayRecordedSessionResult : PlaySessionResultBase
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="PlayRecordedSessionResult"/> class.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// <param name="tmsPath">The path of the timing message store to play.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="container"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <paramref name="tmsPath"/> is of zero length.
        /// </exception>
        public PlayRecordedSessionResult(string tmsPath)
        {
            Guard.NotNullOrEmpty(tmsPath, "tmsPath");

            TmsPath = tmsPath;
        }

        #endregion

        #region Protected Interface.

        /// <inheritdoc/>
        protected override ISessionPlayer CreateSessionPlayer()
        {
            return new DefaultSessionPlayer(CreateReader)
            {
                SupportsPause = true
            };
        }

        #endregion

        #region Private Impl.

        private IMessageReader CreateReader()
        {
            var reader = F1Timing.Playback.Read(TmsPath);

            reader.PlaybackSpeed = 50d;

            return reader;
        }

        private string TmsPath { get; set; }

        #endregion
    }
}