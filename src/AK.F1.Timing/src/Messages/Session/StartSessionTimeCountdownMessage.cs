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
using System.Runtime.Serialization;
using AK.F1.Timing.Serialization;

namespace AK.F1.Timing.Messages.Session
{
    /// <summary>
    /// A message which starts the remaining session time countdown. This class cannot be inherited.
    /// </summary>
    [Serializable]
    [TypeId(-63715761)]
    public sealed class StartSessionTimeCountdownMessage : Message, IObjectReference
    {
        #region Public Interface.

        /// <summary>
        /// Defines the only <see cref="StartSessionTimeCountdownMessage"/> instance. This field is
        /// <see langword="readonly"/>.
        /// </summary>
        public static readonly StartSessionTimeCountdownMessage Instance =
            new StartSessionTimeCountdownMessage();

        /// <inheritdoc/>
        public override void Accept(IMessageVisitor visitor)
        {
            Guard.NotNull(visitor, "visitor");

            visitor.Visit(this);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Repr("");
        }

        #endregion

        #region Explicit Interface.

        object IObjectReference.GetRealObject(StreamingContext context)
        {
            return Instance;
        }

        #endregion

        #region Private Impl.

        private StartSessionTimeCountdownMessage() {}

        #endregion
    }
}