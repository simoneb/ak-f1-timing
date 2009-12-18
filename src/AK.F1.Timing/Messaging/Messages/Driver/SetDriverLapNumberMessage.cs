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
using AK.F1.Timing.Serialization;

namespace AK.F1.Timing.Messaging.Messages.Driver
{
    /// <summary>
    /// A message which sets a driver's current lap number. This class is
    /// <see langword="sealed"/>.
    /// </summary>
    [Serializable]
    [TypeId(3549681)]
    public sealed class SetDriverLapNumberMessage : DriverMessageBase
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="SetDriverLapNumberMessage"/> class and
        /// specifies if the Id of the driver and the lap number.
        /// </summary>
        /// <param name="driverId">The Id of the driver.</param>
        /// <param name="lapNumber">The driver's new lap number.</param>
        public SetDriverLapNumberMessage(int driverId, int lapNumber)
            : base(driverId) {            

            this.LapNumber = lapNumber;
        }

        /// <inheritdoc />
        public override void Accept(IMessageVisitor visitor) {

            Guard.NotNull(visitor, "visitor");

            visitor.Visit(this);
        }

        /// <inheritdoc />
        public override string ToString() {

            return Repr("DriverId={0}, LapNumber={1}", this.DriverId, this.LapNumber);
        }

        /// <summary>
        /// Gets the lap number.
        /// </summary>
        [PropertyId(1)]
        public int LapNumber { get; private set; }

        #endregion
    }
}
