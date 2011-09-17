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
    /// A message which sets a driver's gap. This class cannot be inherited.
    /// </summary>
    [Serializable]
    [TypeId(5)]
    public sealed class SetDriverGapMessage : DriverMessageBase
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="SetDriverGapMessage"/> class and
        /// specifies if the Id of the driver and the gap.
        /// </summary>
        /// <param name="driverId">The Id of the driver.</param>
        /// <param name="gap">The gap.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="driverId"/> is not positive.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="gap"/> is <see langword="null"/>.
        /// </exception>
        public SetDriverGapMessage(int driverId, Gap gap)
            : base(driverId)
        {
            Guard.NotNull(gap, "gap");

            Gap = gap;
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
            return Repr("DriverId={0}, Gap={1}", DriverId, Gap);
        }

        /// <summary>
        /// Gets the gap.
        /// </summary>
        [PropertyId(1)]
        public Gap Gap { get; private set; }

        #endregion
    }
}