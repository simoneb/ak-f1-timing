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
    /// A message which updates the <see cref="AK.F1.Timing.Messages.Session.SessionType"/>
    /// and session identifier. This class cannot be inherited.
    /// </summary>
    [Serializable]
    [TypeId(34)]
    public sealed class SetSessionTypeMessage : Message
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="SetSessionTypeMessage"/> class and
        /// specifies the <see cref="AK.F1.Timing.Messages.Session.SessionType"/> and the session
        /// identifier.
        /// </summary>
        /// <param name="sessionType">The current
        /// <see cref="AK.F1.Timing.Messages.Session.SessionType"/>.</param>
        /// <param name="sessionId">The session identifier.</param>
        public SetSessionTypeMessage(SessionType sessionType, string sessionId)
        {
            Guard.NotNull(sessionId, "sessionId");

            SessionType = sessionType;
            SessionId = sessionId;
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
            return Repr("SessionType='{0}', SessionId='{1}'", SessionType, SessionId);
        }

        /// <summary>
        /// Gets the current <see cref="AK.F1.Timing.Messages.Session.SessionType"/>.
        /// </summary>
        [PropertyId(0)]
        public SessionType SessionType { get; private set; }

        /// <summary>
        /// Gets the current session identifier.
        /// </summary>
        [PropertyId(1)]
        public string SessionId { get; private set; }

        #endregion
    }
}