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

namespace AK.F1.Timing.Messages.Weather
{
    /// <summary>
    /// A message which sets the current wind direction, in degrees.  This class cannot be inherited.
    /// </summary>
    [Serializable]
    [TypeId(91304368)]
    public sealed class SetWindAngleMessage : Message
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="SetWindAngleMessage"/> class and
        /// specifies the wind angle, in degrees.
        /// </summary>
        /// <param name="angle">The wind angle, in degrees.</param>        
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="angle"/> is negative or greater than 360.
        /// </exception>
        public SetWindAngleMessage(int angle)
        {
            Guard.InRange(IsValidAngle(angle), "angle");

            Angle = angle;
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
            return Repr("Angle={0}", Angle);
        }

        /// <summary>
        /// Returns a value indicating if the specified <paramref name="angle"/> is valid.
        /// </summary>
        /// <param name="angle">The angle to test.</param>
        /// <returns><see langword="true"/> if the specified <paramref name="angle"/> is valid,
        /// otherwise; <see langword="false"/>.</returns>
        public static bool IsValidAngle(int angle)
        {
            return angle >= 0 && angle <= 360;
        }

        /// <summary>
        /// Gets the current wind angle, in degrees.
        /// </summary>
        [PropertyId(0)]
        public int Angle { get; private set; }

        #endregion
    }
}