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
    /// A message which sets a driver's qually time. This class cannot be inherited.
    /// </summary>
    [Serializable]
    [TypeId(13)]
    public sealed class SetDriverQuallyTimeMessage : DriverMessageBase
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="SetDriverQuallyTimeMessage"/> class and
        /// specifies if the Id of the driver, the qually number and qually time.
        /// </summary>
        /// <param name="driverId">The Id of the driver.</param>
        /// <param name="quallyNumber">The qually number.</param>
        /// <param name="quallyTime">The qually time.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="driverId"/> or <paramref name="quallyNumber"/> is not positive
        /// or <paramref name="quallyTime"/> is not positive.
        /// </exception>
        public SetDriverQuallyTimeMessage(int driverId, int quallyNumber, TimeSpan quallyTime)
            : base(driverId)
        {
            Guard.InRange(quallyNumber > 0, "quallyNumber");
            Guard.InRange(quallyTime > TimeSpan.Zero, "quallyTime");

            QuallyNumber = quallyNumber;
            QuallyTime = quallyTime;
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
            return Repr("DriverId={0}, QuallyNumber={1}, QuallyTime='{2}'", DriverId,
                QuallyNumber, QuallyTime);
        }

        /// <summary>
        /// Gets the qually time.
        /// </summary>
        [PropertyId(1)]
        public TimeSpan QuallyTime { get; private set; }

        /// <summary>
        /// Gets the qually number (one-based) of the posted time.
        /// </summary>
        [PropertyId(2)]
        public int QuallyNumber { get; private set; }

        #endregion
    }
}