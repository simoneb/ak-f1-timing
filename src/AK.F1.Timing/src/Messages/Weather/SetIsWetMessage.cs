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
    /// A message which specifies whether the track is dry or wet. This class cannot be inherited.
    /// </summary>
    [Serializable]
    [TypeId(72954222)]
    public sealed class SetIsWetMessage : Message
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="SetIsWetMessage"/> class and specifies
        /// whether the track is wet or dry.
        /// </summary>
        /// <param name="isWet"></param>
        public SetIsWetMessage(bool isWet)
        {
            IsWet = isWet;
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
            return Repr("IsWet='{0}'", IsWet);
        }

        /// <summary>
        /// Gets a value indicating if the track is wet.
        /// </summary>
        [PropertyId(0)]
        public bool IsWet { get; private set; }

        #endregion
    }
}