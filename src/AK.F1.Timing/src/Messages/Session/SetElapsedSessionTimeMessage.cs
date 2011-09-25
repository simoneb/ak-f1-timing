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
    /// A message which sets the elapsed session session time. This class cannot be inherited.
    /// </summary>
    [Serializable, TypeId(29)]
    public sealed class SetElapsedSessionTimeMessage : Message
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="SetElapsedSessionTimeMessage"/> class
        /// and specifies the elapsed session time.
        /// </summary>
        /// <param name="elapsed">The elapsed session time..</param>        
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="elapsed"/> is negative.
        /// </exception>
        public SetElapsedSessionTimeMessage(TimeSpan elapsed)
        {
            Guard.InRange(elapsed >= TimeSpan.Zero, "elapsed");

            Elapsed = elapsed;
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
            return Repr("Elapsed='{0}'", Elapsed);
        }

        /// <summary>
        /// Gets the elapsed session time.
        /// </summary>
        [PropertyId(0)]
        public TimeSpan Elapsed { get; private set; }

        #endregion
    }
}