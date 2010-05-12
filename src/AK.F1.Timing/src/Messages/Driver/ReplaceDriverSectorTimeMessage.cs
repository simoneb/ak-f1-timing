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
    /// A message which replaces a driver's previous sector time. This class cannot be inherited.
    /// </summary>
    [Serializable]
    [TypeId(-91873925)]
    public sealed class ReplaceDriverSectorTimeMessage : DriverMessageBase
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="ReplaceDriverSectorTimeMessage"/> class and
        /// specifies if the Id of the driver, the sector number, and the previous sector time
        /// replacement.
        /// </summary>
        /// <param name="driverId">The Id of the driver.</param>
        /// <param name="sectorNumber">The sector number.</param>
        /// <param name="replacement">The previous sector time replacement.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="driverId"/> or <paramref name="sectorNumber"/> is not positive.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="replacement"/> is <see langword="null"/>.
        /// </exception>
        public ReplaceDriverSectorTimeMessage(int driverId, int sectorNumber, PostedTime replacement)
            : base(driverId)
        {
            Guard.InRange(sectorNumber > 0, "sectorNumber");
            Guard.NotNull(replacement, "replacement");

            SectorNumber = sectorNumber;
            Replacement = replacement;
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
            return Repr("DriverId={0}, SectorNumber={1}, Replacement={2}",
                DriverId, SectorNumber, Replacement);
        }

        /// <summary>
        /// Gets the sector number (one-based) of the posted time.
        /// </summary>
        [PropertyId(1)]
        public int SectorNumber { get; private set; }

        /// <summary>
        /// Gets the previous sector time replacement.
        /// </summary>
        [PropertyId(2)]
        public PostedTime Replacement { get; private set; }

        #endregion
    }
}