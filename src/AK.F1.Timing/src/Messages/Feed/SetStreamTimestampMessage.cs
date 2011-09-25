// Copyright 2011 Andy Kernahan
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
    /// A message which sets the feed timestamp. This class cannot be inherited.
    /// </summary>
    [Serializable, TypeId(24)]
    public sealed class SetStreamTimestampMessage : Message
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="SetStreamTimestampMessage"/> class.
        /// </summary>        
        public SetStreamTimestampMessage()
        {
            Timestamp = DateTime.UtcNow;
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
            return Repr("Timestamp='{0:o}'", Timestamp);
        }

        /// <summary>
        /// Gets the feed timestamp.
        /// </summary>
        [PropertyId(0)]
        public DateTime Timestamp { get; private set; }

        #endregion
    }
}