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

namespace AK.F1.Timing.Messages.Session
{
    /// <summary>
    /// A message which sets the minimum required qualification time. This class cannot be
    /// inherited.
    /// </summary>
    /// <remarks>As of the 2011 season the minimum required qualificaion time is computed as 107%
    /// of the fastest time set in Q1.</remarks>
    [Serializable]
    [TypeId(63565184)]
    public sealed class SetMinRequiredQuallyTimeMessage : Message
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="SetMinRequiredQuallyTimeMessage"/> class
        /// and specifies the minimum required qualification time.
        /// </summary>
        /// <param name="time">The minimum required qualification time.</param>        
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="time"/> is negative.
        /// </exception>
        public SetMinRequiredQuallyTimeMessage(TimeSpan time)
        {
            Guard.InRange(time >= TimeSpan.Zero, "time");

            Time = time;
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
            return Repr("Time='{0}'", Time);
        }

        /// <summary>
        /// Gets the minimum required qualification time.
        /// </summary>
        [PropertyId(0)]
        public TimeSpan Time { get; private set; }

        #endregion
    }
}