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
    /// A message which contains the copyright text associated with the message stream. This class
    /// cannot be inherited.
    /// </summary>
    [Serializable]
    [TypeId(40380621)]
    public sealed class SetCopyrightMessage : Message
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="SetCopyrightMessage"/> class and
        /// specifies the new copyright text.
        /// </summary>
        /// <param name="copyright">The new copyright text.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="copyright"/> is <see langword="null"/>.
        /// </exception>
        public SetCopyrightMessage(string copyright) {            

            Guard.NotNull(copyright, "copyright");

            Copyright = copyright;
        }

        /// <inheritdoc />
        public override void Accept(IMessageVisitor visitor) {

            Guard.NotNull(visitor, "visitor");

            visitor.Visit(this);
        }

        /// <inheritdoc />
        public override string ToString() {

            return Repr("Copyright='{0}'", Copyright);
        }

        /// <summary>
        /// Gets the new copyright text.
        /// </summary>
        [PropertyId(0)]
        public string Copyright { get; private set; }

        #endregion
    }
}
