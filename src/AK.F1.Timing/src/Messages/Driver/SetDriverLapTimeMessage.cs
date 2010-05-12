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
    /// A message which sets a driver's lap time. This class cannot be inherited.
    /// </summary>
    [Serializable]
    [TypeId(-68954098)]
    public sealed class SetDriverLapTimeMessage : DriverMessageBase
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="SetDriverLapTimeMessage"/> class and
        /// specifies if the Id of the driver and the lap time set by the driver.
        /// </summary>
        /// <param name="driverId">The Id of the driver.</param>
        /// <param name="lapTime">The lap time.</param>        
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="driverId"/> is not positive.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="lapTime"/> is <see langword="null"/>.
        /// </exception>
        public SetDriverLapTimeMessage(int driverId, PostedTime lapTime)
            : base(driverId)
        {
            Guard.NotNull(lapTime, "lapTime");

            LapTime = lapTime;
        }

        /// <inheritdoc />
        public override void Accept(IMessageVisitor visitor)
        {
            Guard.NotNull(visitor, "visitor");

            visitor.Visit(this);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Repr("DriverId={0}, LapTime={1}", DriverId, LapTime);
        }

        /// <summary>
        /// Gets the posted lap time.
        /// </summary>
        [PropertyId(1)]
        public PostedTime LapTime { get; private set; }

        #endregion
    }
}