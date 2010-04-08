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

namespace AK.F1.Timing.Live
{
    /// <summary>
    /// Represents the header of an F1 live-timing message. This class cannot be inherited.
    /// </summary>
    /// <remarks>
    /// The properties of this class are interpreted as a whole rather than individualy as a
    /// property can legally have a completely different meaning based on the value of another
    /// property.
    /// </remarks>
    [Serializable]
    internal sealed class LiveMessageHeader
    {
        #region Public Interface.

        /// <inheritdoc />
        public override string ToString() {

            return string.Format(
                "{0}(MessageType='{1}', DriverId='{2}', Colour='{3}', DataLength='{4}', Value='{5}')",
                GetType().Name, MessageType, DriverId, Colour, DataLength, Value);
        }

        /// <summary>
        /// Returns a value indicating if the message is a system message.
        /// </summary>
        public bool IsSystemMessage {

            get { return DriverId == 0; }
        }

        /// <summary>
        /// Returns a value indicating if the message is a driver message.
        /// </summary>
        public bool IsDriverMessage {

            get { return DriverId > 0; }
        }

        /// <summary>
        /// Gets or sets the type of this message.
        /// </summary>
        public byte MessageType { get; set; }

        /// <summary>
        /// Gets or sets the driver Id to which this message related.
        /// </summary>
        public byte DriverId { get; set; }

        /// <summary>
        /// Gets or sets the grid column colour to apply.
        /// </summary>
        public byte Colour { get; set; }

        /// <summary>
        /// Gets or sets the extended data length of this message.
        /// </summary>
        public byte DataLength { get; set; }

        /// <summary>
        /// Gets the "value" of this message.
        /// </summary>
        public byte Value { get; set; }

        #endregion
    }
}
