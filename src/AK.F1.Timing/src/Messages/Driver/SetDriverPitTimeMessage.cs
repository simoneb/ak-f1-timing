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

namespace AK.F1.Timing.Messages.Driver
{
    /// <summary>
    /// A message which sets the length of a driver's pit stop. This class cannot be inherited.
    /// </summary>
    [Serializable]
    [TypeId(24683217)]
    public sealed class SetDriverPitTimeMessage : DriverMessageBase
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="SetDriverPitTimeMessage"/> class and
        /// specifies if the Id of the driver, the pit time and the lap number on which the
        /// driver pitted.
        /// </summary>
        /// <param name="driverId">The Id of the driver.</param>
        /// <param name="time">The pit time, inclusive of the time taken to travel the pit lane.</param>
        /// <param name="lapNumber">The lap number on which the driver pitted.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="driverId"/> or <paramref name="lapNumber"/> is not positive or
        /// <paramref name="time"/> is negative.
        /// </exception>
        public SetDriverPitTimeMessage(int driverId, TimeSpan time, int lapNumber)
            : base(driverId) {

            Guard.InRange(time >= TimeSpan.Zero, "time");
            Guard.InRange(lapNumber >= 0, "lapNumber");

            Time = time;
            LapNumber = lapNumber;
        }

        /// <inheritdoc />
        public override void Accept(IMessageVisitor visitor) {

            Guard.NotNull(visitor, "visitor");

            visitor.Visit(this);
        }

        /// <inheritdoc />
        public override string ToString() {

            return Repr("DriverId={0}, Time='{1}', LapNumber={2}", DriverId,
                Time, LapNumber);
        }

        /// <summary>
        /// Gets the pit time. This time includes the time taken to travel the length of the pit.
        /// </summary>
        [PropertyId(1)]
        public TimeSpan Time { get; private set; }

        /// <summary>
        /// Gets the lap number on which the driver pitted.
        /// </summary>
        [PropertyId(2)]
        public int LapNumber { get; private set; }

        #endregion
    }
}
