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
    /// A message which sets a value indicating whether the message stream is valid. This class
    /// cannot be inherited.
    /// </summary>
    [Serializable]
    [TypeId(25)]
    public sealed class SetStreamValidityMessage : Message
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="SetStreamValidityMessage"/> class and
        /// specifies if the application is valid.
        /// </summary>
        /// <param name="isValid"><see langword="true"/> if the stream is valid, otherwise;
        /// <see langword="false"/>.</param>
        public SetStreamValidityMessage(bool isValid)
        {
            IsValid = isValid;
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
            return Repr("IsValid='{0}'", IsValid);
        }

        /// <summary>
        /// Gets a valid indicating if the feed is valid.
        /// </summary>
        [PropertyId(0)]
        public bool IsValid { get; private set; }

        #endregion
    }
}