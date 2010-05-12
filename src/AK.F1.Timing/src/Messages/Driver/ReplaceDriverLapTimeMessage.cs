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
    /// A message which replaces a driver's previous lap time. This class cannot be inherited.
    /// </summary>
    [Serializable]
    [TypeId(-73336549)]
    public sealed class ReplaceDriverLapTimeMessage : DriverMessageBase
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="ReplaceDriverLapTimeMessage"/> class and
        /// specifies if the Id of the driver and the previous lap time replacement.
        /// </summary>
        /// <param name="driverId">The Id of the driver.</param>
        /// <param name="replacement">The previous lap time replacement.</param>        
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="driverId"/> is not positive.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="replacement"/> is <see langword="null"/>.
        /// </exception>
        public ReplaceDriverLapTimeMessage(int driverId, PostedTime replacement)
            : base(driverId)
        {
            Guard.NotNull(replacement, "replacement");

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
            return Repr("DriverId={0}, Replacement={1}", DriverId, Replacement);
        }

        /// <summary>
        /// Gets the lap time replacement.
        /// </summary>
        [PropertyId(1)]
        public PostedTime Replacement { get; private set; }

        #endregion
    }
}