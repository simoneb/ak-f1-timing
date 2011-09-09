
// Copyright 2011 Andy Kernahan
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file exceptionept in compliance with the License.
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

namespace AK.F1.Timing.Proxy.Messages
{
    /// <summary>
    /// This class cannot be inherited.
    /// </summary>
    [Serializable]
    [TypeId(42678832)]
    internal class ServerLoginMessage : Message
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="ServerLoginMessage"/> class and
        /// specifies the user's <paramref name="username"/> and <paramref name="password"/>.
        /// </summary>
        /// <param name="username">The user's username.</param>
        /// <param name="password">The user's password.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="username"/> or <paramref name="password"/> is 
        /// <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <paramref name="username"/> or <paramref name="password"/> is empty.
        /// </exception>
        public ServerLoginMessage(string username, string password)
        {
            Guard.NotNull(username, "username");
            Guard.NotNull(password, "password");

            Username = username;
            Password = password;
        }

        /// <summary>
        /// This method is a no-op.
        /// </summary>
        /// <param name="visitor">The message visitor.</param>
        public override void Accept(IMessageVisitor visitor) { }

        /// <summary>
        /// Gets the user's username.
        /// </summary>
        [PropertyId(0)]
        public string Username { get; private set; }

        /// <summary>
        /// Gets the user's password.
        /// </summary>
        [PropertyId(1)]
        public string Password { get; private set; }

        #endregion
    }
}