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
    /// A message which sets the current air temperature, in degrees celsius. This class cannot be
    /// inherited.
    /// </summary>
    [Serializable]
    [TypeId(16613064)]
    public sealed class SetAirTemperatureMessage : Message
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="SetAirTemperatureMessage"/> class and
        /// specifies the new air temperature, in degrees celsius.
        /// </summary>
        /// <param name="temperature">The air temperature, in degrees celsius.</param>
        public SetAirTemperatureMessage(double temperature)
        {
            Temperature = temperature;
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
            return Repr("Temperature={0}", Temperature);
        }

        /// <summary>
        /// Gets the current air temperature, in degrees celsius.
        /// </summary>
        [PropertyId(0)]
        public double Temperature { get; private set; }

        #endregion
    }
}