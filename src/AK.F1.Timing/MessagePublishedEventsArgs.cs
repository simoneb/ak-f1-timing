// Copyright 2009 Andy Kernahan
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

namespace AK.F1.Timing
{
    /// <summary>
    /// Contains the <see cref="AK.F1.Timing.Message"/> published by a source.
    /// </summary>
    [Serializable]
    public class MessagePublishedEventArgs : EventArgs
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="MessagePublishedEventArgs"/>
        /// class and specifies the published <see cref="AK.F1.Timing.Message"/>.
        /// </summary>
        /// <param name="message">The published <see cref="AK.F1.Timing.Message"/>.</param>
        public MessagePublishedEventArgs(Message message) {

            Guard.NotNull(message, "message");

            this.Message = message;
        }

        /// <summary>
        /// Gets the published <see cref="AK.F1.Timing.Message"/>.
        /// </summary>
        public Message Message { get; private set; }

        #endregion
    }
}
