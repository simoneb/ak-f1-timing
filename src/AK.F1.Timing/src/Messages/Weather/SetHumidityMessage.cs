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
    /// A message which sets the current humidity, as a percentage. This class cannot be inherited.
    /// </summary>
    [Serializable]
    [TypeId(-81152176)]
    public sealed class SetHumidityMessage : Message
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="SetHumidityMessage"/> class and
        /// specifies the new humidity, as a percentage.
        /// </summary>
        /// <param name="humidity">The humidity, as a percentage.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="humidity"/> is negative or greater than one hundred.
        /// </exception>
        public SetHumidityMessage(double humidity)
        {
            Guard.InRange(humidity >= 0d && humidity < 100d, "humidity");

            Humidity = humidity;
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
            return Repr("Humidity={0}", Humidity);
        }

        /// <summary>
        /// Gets the current humidity, as a percentage.
        /// </summary>
        [PropertyId(0)]
        public double Humidity { get; private set; }

        #endregion
    }
}