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
    /// A message which sets a driver's interval. This class cannot be inherited.
    /// </summary>
    [Serializable]
    [TypeId(6)]
    public sealed class SetDriverIntervalMessage : DriverMessageBase
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="SetDriverIntervalMessage"/> class and
        /// specifies if the Id of the driver and the interval.
        /// </summary>
        /// <param name="driverId">The Id of the driver.</param>
        /// <param name="interval">The interval.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="driverId"/> is not positive.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="interval"/> is <see langword="null"/>.
        /// </exception>
        public SetDriverIntervalMessage(int driverId, Gap interval)
            : base(driverId)
        {
            Guard.NotNull(interval, "interval");

            Interval = interval;
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
            return Repr("DriverId={0}, Interval={1}", DriverId, Interval);
        }

        /// <summary>
        /// Gets the interval.
        /// </summary>
        [PropertyId(1)]
        public Gap Interval { get; private set; }

        #endregion
    }
}