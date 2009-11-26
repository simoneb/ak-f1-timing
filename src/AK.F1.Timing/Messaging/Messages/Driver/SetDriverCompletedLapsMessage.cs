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
using AK.F1.Timing.Messaging.Serialization;

namespace AK.F1.Timing.Messaging.Messages.Driver
{
    /// <summary>
    /// A message which updates the number of laps completed by a driver. This class is
    /// <see langword="sealed"/>.
    /// </summary>
    [Serializable]
    [TypeId(59519632)]
    public sealed class SetDriverCompletedLapsMessage : DriverMessageBase
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="SetDriverCompletedLapsMessage"/> class
        /// and specifies if the Id of the driver and the number of completed laps.
        /// </summary>
        /// <param name="driverId">The Id of the driver.</param>
        /// <param name="completedLaps">The number of completed laps.</param>
        public SetDriverCompletedLapsMessage(int driverId, int completedLaps)
            : base(driverId) {            

            this.CompletedLaps = completedLaps;
        }

        /// <inheritdoc />
        public override void Accept(IMessageVisitor visitor) {

            Guard.NotNull(visitor, "visitor");

            visitor.Visit(this);
        }

        /// <inheritdoc />
        public override string ToString() {

            return Repr("DriverId={0}, CompletedLaps={1}", this.DriverId,
                this.CompletedLaps);
        }

        /// <summary>
        /// Gets the number of completed laps.
        /// </summary>
        [PropertyId(1)]
        public int CompletedLaps { get; private set; }

        #endregion
    }
}
