﻿// Copyright 2009 Andy Kernahan
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

namespace AK.F1.Timing.Messaging.Messages.Driver
{
    /// <summary>
    /// A message which sets a driver's track position. This class is <see langword="sealed"/>.
    /// </summary>
    [Serializable]
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
            : base(driverId) {

            Guard.InRange(position > 0, "position");

            this.Position = position;
        }

        /// <inheritdoc />
        public override void Accept(IMessageVisitor visitor) {

            Guard.NotNull(visitor, "visitor");

            visitor.Visit(this);
        }

        /// <inheritdoc />
        public override string ToString() {

            return Repr("DriverId='{0}', Position='{1}'", this.DriverId, this.Position);
        }

        /// <summary>
        /// Gets the driver's new position.
        /// </summary>
        public int Position { get; private set; }

        #endregion
    }
}
