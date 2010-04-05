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
    /// A message which sets the <see cref="AK.F1.Timing.Messages.Session.SessionStatus"/>
    /// of the current session. This class cannot be inherited.
    /// </summary>
    [Serializable]
    [TypeId(58867632)]
    public sealed class SetSessionStatusMessage : Message
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="SetSessionStatusMessage"/> class and
        /// specifies the <see cref="AK.F1.Timing.Messages.Session.SessionStatus"/>.
        /// </summary>
        /// <param name="sessionStatus">The current 
        /// <see cref="AK.F1.Timing.Messages.Session.SessionStatus"/>.</param>        
        public SetSessionStatusMessage(SessionStatus sessionStatus) {

            SessionStatus = sessionStatus;            
        }

        /// <inheritdoc />
        public override void Accept(IMessageVisitor visitor) {

            Guard.NotNull(visitor, "visitor");

            visitor.Visit(this);
        }

        /// <inheritdoc />
        public override string ToString() {

            return Repr("SessionStatus='{0}'", SessionStatus);
        }

        /// <summary>
        /// Gets the current <see cref="AK.F1.Timing.Messages.Session.SessionStatus"/>.
        /// </summary>
        [PropertyId(0)]
        public SessionStatus SessionStatus { get; private set; }

        #endregion
    }
}
