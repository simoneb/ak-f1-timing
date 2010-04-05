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

namespace AK.F1.Timing.Messages.Feed
{
    /// <summary>
    /// A message which sets the interval at which the live message stream should be pinged with a
    /// magic packet. This class cannot be inherited.
    /// </summary>
    [Serializable]
    [TypeId(91507218)]
    public class SetPingIntervalMessage : Message
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="SetPingIntervalMessage"/> class and
        /// specifies the refresh rate.
        /// </summary>
        /// <param name="pingInterval">The refresh rate.</param>        
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="pingInterval"/> is negative.
        /// </exception>
        public SetPingIntervalMessage(TimeSpan pingInterval) {

            Guard.InRange(pingInterval >= TimeSpan.Zero, "pingInterval");

            PingInterval = pingInterval;            
        }

        /// <inheritdoc />
        public override void Accept(IMessageVisitor visitor) {

            Guard.NotNull(visitor, "visitor");

            visitor.Visit(this);
        }

        /// <inheritdoc />
        public override string ToString() {

            return Repr("PingInterval='{0}'", PingInterval);
        }

        /// <summary>
        /// Gets the ping interval.
        /// </summary>
        [PropertyId(0)]
        public TimeSpan PingInterval { get; private set; }

        #endregion
    }
}
