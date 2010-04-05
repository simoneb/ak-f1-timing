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
    /// A message which sets a driver's name. This class cannot be inherited.
    /// </summary>
    [Serializable]
    [TypeId(-9752615)]
    public sealed class SetDriverNameMessage : DriverMessageBase
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="SetDriverNameMessage"/> class
        /// and specifies if the Id of the driver and the driver's name.
        /// </summary>
        /// <param name="driverId">The Id of the driver.</param>
        /// <param name="driverName">The driver's name.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="driverId"/> is not positive.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="driverName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <paramref name="driverName"/> is or zero length.
        /// </exception>
        public SetDriverNameMessage(int driverId, string driverName)
            : base(driverId) {

            Guard.NotNullOrEmpty(driverName, "driverName");

            DriverName = driverName;
        }

        /// <inheritdoc />
        public override void Accept(IMessageVisitor visitor) {

            Guard.NotNull(visitor, "visitor");

            visitor.Visit(this);
        }

        /// <inheritdoc />
        public override string ToString() {

            return Repr("DriverId={0}, DriverName='{1}'", DriverId, DriverName);
        }

        /// <summary>
        /// Gets the driver's new name.
        /// </summary>
        [PropertyId(1)]
        public string DriverName { get; private set; }

        #endregion
    }
}
