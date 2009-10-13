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
using AK.F1.Timing.Messaging.Serialization;

namespace AK.F1.Timing.Messaging.Messages.Weather
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [TypeId(-50574713)]
    public sealed class SetWindSpeedMessage : Message
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="SetWindSpeedMessage"/> class and
        /// specifies the new wind speed, in meters per second.
        /// </summary>
        /// <param name="speed">The wind speed, in meters per second.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="speed"/> is negative.
        /// </exception>
        public SetWindSpeedMessage(double speed) {

            Guard.InRange(speed >= 0d, "speed");

            this.Speed = speed;            
        }

        /// <inheritdoc />
        public override void Accept(IMessageVisitor visitor) {

            Guard.NotNull(visitor, "visitor");

            visitor.Visit(this);
        }

        /// <inheritdoc />
        public override string ToString() {

            return Repr("Speed='{0}'", this.Speed);
        }

        /// <summary>
        /// Gets the current wind speed, in meters per second.
        /// </summary>
        [PropertyId(0)]
        public double Speed { get; private set; }

        #endregion
    }
}
