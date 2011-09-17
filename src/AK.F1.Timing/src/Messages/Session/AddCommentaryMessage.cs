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
    /// A message which adds a block commentary related to the current session. This class cannot
    /// be inherited.
    /// </summary>
    [Serializable]
    [TypeId(27)]
    public sealed class AddCommentaryMessage : Message
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="AddCommentaryMessage"/> class and specifies
        /// the <paramref name="commentary"/> text.
        /// </summary>
        /// <param name="commentary">The commentary text.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="commentary"/> is <see langword="null"/>.
        /// </exception>
        public AddCommentaryMessage(string commentary)
        {
            Guard.NotNull(commentary, "commentary");

            Commentary = commentary;
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
            return Repr("Commentary='{0}'", Commentary);
        }

        /// <summary>
        /// Gets the commentary text.
        /// </summary>        
        [PropertyId(0)]
        public string Commentary { get; private set; }

        #endregion
    }
}