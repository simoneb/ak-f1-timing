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
    /// A message which sets the lastest ketframe number associated with the message stream. This
    /// class cannot be inherited.
    /// </summary>
    [Serializable]
    [TypeId(21)]
    public sealed class SetKeyframeMessage : Message
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="SetKeyframeMessage"/> class and specifies
        /// the new keyframe number.
        /// </summary>
        /// <param name="keyframe">The keyframe number.</param> 
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="keyframe"/> is negative.
        /// </exception>
        public SetKeyframeMessage(int keyframe)
        {
            Guard.InRange(keyframe >= 0, "keyframe");

            Keyframe = keyframe;
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
            return Repr("Keyframe={0}", Keyframe);
        }

        /// <summary>
        /// Gets the keyframe.
        /// </summary>
        [PropertyId(0)]
        public int Keyframe { get; private set; }

        #endregion
    }
}