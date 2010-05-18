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
    /// A message which contains a message related to the state of the message stream. This class
    /// cannot be inherited.
    /// </summary>
    [Serializable]
    [TypeId(-2058583)]
    public sealed class SetSystemMessageMessage : Message
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="SetSystemMessageMessage"/> class and
        /// specifies the new system message text.
        /// </summary>
        /// <param name="message">The new system message text.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        public SetSystemMessageMessage(string message)
        {
            Guard.NotNull(message, "message");

            Message = message;
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
            return Repr("Message='{0}'", Message);
        }

        /// <summary>
        /// Gets the new system message text.
        /// </summary>
        [PropertyId(0)]
        public string Message { get; private set; }

        #endregion
    }
}