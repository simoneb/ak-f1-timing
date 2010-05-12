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
    /// A message which sets the current race lap number. This class cannot be inherited.
    /// </summary>
    [Serializable]
    [TypeId(-57818482)]
    public sealed class SetRaceLapNumberMessage : Message
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="SetRaceLapNumberMessage"/> class and
        /// specifies the lap number.
        /// </summary>
        /// <param name="lapNumber">The lap number.</param>        
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="lapNumber"/> is negative.
        /// </exception>
        public SetRaceLapNumberMessage(int lapNumber)
        {
            Guard.InRange(lapNumber >= 0, "lapNumber");

            LapNumber = lapNumber;
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
            return Repr("LapNumber={0}", LapNumber);
        }

        /// <summary>
        /// Gets the current race lap number.
        /// </summary>
        [PropertyId(0)]
        public int LapNumber { get; private set; }

        #endregion
    }
}