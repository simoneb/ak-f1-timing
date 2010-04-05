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
    /// A message which sets a driver's car number. This class cannot be inherited.
    /// </summary>
    [Serializable]
    [TypeId(-47573943)]
    public sealed class SetDriverCarNumberMessage : DriverMessageBase
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="SetDriverCarNumberMessage"/> class and
        /// specifies if the Id of the driver and the driver's car number.
        /// </summary>
        /// <param name="driverId">The Id of the driver.</param>
        /// <param name="carNumber">The driver's new car number.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="driverId"/> or <paramref name="carNumber"/> is not positive.
        /// </exception>
        public SetDriverCarNumberMessage(int driverId, int carNumber)
            : base(driverId) {

            Guard.InRange(carNumber > 0, "carNumber");

            CarNumber = carNumber;
        }

        /// <inheritdoc />
        public override void Accept(IMessageVisitor visitor) {

            Guard.NotNull(visitor, "visitor");

            visitor.Visit(this);
        }

        /// <inheritdoc />
        public override string ToString() {

            return Repr("DriverId={0}, CarNumber={1}", DriverId, CarNumber);
        }

        /// <summary>
        /// Gets the driver's new car number.
        /// </summary>
        [PropertyId(1)]
        public int CarNumber { get; private set; }

        #endregion
    }
}
