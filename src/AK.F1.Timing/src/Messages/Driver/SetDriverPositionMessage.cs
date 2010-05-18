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
    /// A message which sets a driver's track position. This class cannot be inherited.
    /// </summary>
    [Serializable]
    [TypeId(13097688)]
    public sealed class SetDriverPositionMessage : DriverMessageBase
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="SetDriverPositionMessage"/> class and
        /// specifies if the Id of the driver and the driver's new position.
        /// </summary>
        /// <param name="driverId">The Id of the driver.</param>
        /// <param name="position">The driver's new position.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="driverId"/> or <paramref name="position"/> is not positive.
        /// </exception>
        public SetDriverPositionMessage(int driverId, int position)
            : base(driverId)
        {
            Guard.InRange(position > 0, "position");

            Position = position;
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
            return Repr("DriverId={0}, Position={1}", DriverId, Position);
        }

        /// <summary>
        /// Gets the driver's new position.
        /// </summary>
        [PropertyId(1)]
        public int Position { get; private set; }

        #endregion
    }
}