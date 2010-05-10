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
    /// A message which sets a driver's current pit count. This class cannot be inherited.
    /// </summary>
    [Serializable]
    [TypeId(31968328)]
    public sealed class SetDriverPitCountMessage : DriverMessageBase
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="SetDriverPitCountMessage"/> class and
        /// specifies if the Id of the driver and the pit count.
        /// </summary>
        /// <param name="driverId">The Id of the driver.</param>
        /// <param name="pitCount">The driver's new pit count.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="driverId"/> is not positive.
        /// </exception>  
        public SetDriverPitCountMessage(int driverId, int pitCount)
            : base(driverId) {

            Guard.InRange(pitCount >= 0, "pitCount");

            PitCount = pitCount;
        }

        /// <inheritdoc />
        public override void Accept(IMessageVisitor visitor) {

            Guard.NotNull(visitor, "visitor");

            visitor.Visit(this);
        }

        /// <inheritdoc />
        public override string ToString() {

            return Repr("DriverId={0}, PitCount={1}", DriverId, PitCount);
        }

        /// <summary>
        /// Gets the pit count.
        /// </summary>
        [PropertyId(1)]
        public int PitCount { get; private set; }

        #endregion
    }
}
