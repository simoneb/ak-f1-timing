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
    /// A message which sets the current atmospheric pressure, in millibars. This class cannot be
    /// inherited.
    /// </summary>
    [Serializable, TypeId(39)]
    public sealed class SetAtmosphericPressureMessage : Message
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="SetAtmosphericPressureMessage"/> class
        /// and specifies the new atmospheric pressure, in millibars.
        /// </summary>
        /// <param name="pressure">The atmospheric pressure, in millibars.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="pressure"/> is negative.
        /// </exception>
        public SetAtmosphericPressureMessage(double pressure)
        {
            Guard.InRange(pressure > 0D, "pressure");

            Pressure = pressure;
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
            return Repr("Pressure={0}", Pressure);
        }

        /// <summary>
        /// Gets the current atmospheric pressure, in millibars.
        /// </summary>
        [PropertyId(0)]
        public double Pressure { get; private set; }

        #endregion
    }
}