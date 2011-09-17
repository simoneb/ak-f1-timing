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

using System;
using AK.F1.Timing.Messages.Session;
using AK.F1.Timing.Serialization;

namespace AK.F1.Timing.Messages.Driver
{
    /// <summary>
    /// A message which sets a driver's speed at a particualr location on the track.
    /// This class cannot be inherited.
    /// </summary>
    [Serializable]
    [TypeId(15)]
    public sealed class SetDriverSpeedMessage : DriverMessageBase
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="SetDriverSpeedMessage"/> class and
        /// specifies if the Id of the driver, the speed capture location and the speed.
        /// </summary>
        /// <param name="driverId">The Id of the driver.</param>
        /// <param name="location">The speed capture location.</param>
        /// <param name="speed">The driver's speed.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="driverId"/> or <paramref name="speed"/> is negative.
        /// </exception>  
        public SetDriverSpeedMessage(int driverId, SpeedCaptureLocation location, int speed)
            : base(driverId)
        {
            Guard.InRange(speed >= 0, "speed");

            Location = location;
            Speed = speed;
        }

        /// <inheritdoc/>
        public override void Accept(IMessageVisitor visitor)
        {
            Guard.NotNull(visitor, "visitor");

            visitor.Visit(this);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Repr("DriverId={0}, Location='{1}', Speed={2}", DriverId, Location, Speed);
        }

        /// <summary>
        /// Gets the capture location
        /// </summary>
        [PropertyId(1)]
        public SpeedCaptureLocation Location { get; private set; }

        /// <summary>
        /// Gets the captured speed.
        /// </summary>
        [PropertyId(2)]
        public int Speed { get; private set; }

        #endregion
    }
}