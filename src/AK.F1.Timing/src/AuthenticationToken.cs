// Copyright 2010 Andy Kernahan
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

namespace AK.F1.Timing
{
    /// <summary>
    /// An opaque authentication token. This class cannot be inherited.
    /// </summary>    
    public sealed class AuthenticationToken : IEquatable<AuthenticationToken>
    {
        #region Fields.

        private readonly string _token;
        private static readonly StringComparer _comparer = StringComparer.Ordinal;

        #endregion

        #region Public Interface.

        /// <inheritdoc/>
        public override bool Equals(object obj) {

            return Equals(obj as AuthenticationToken);
        }

        /// <inheritdoc/>
        public bool Equals(AuthenticationToken other) {

            if(other == this) {
                return true;
            }
            if(other == null) {
                return false;
            }
            return _comparer.Equals(other.Token, Token);
        }

        /// <inheritdoc/>
        public override int GetHashCode() {

            return _comparer.GetHashCode(Token);
        }

        #endregion

        #region Internal Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="AuthenticationToken"/> class and
        /// specifies the authentication <paramref name="token"/>.
        /// </summary>
        /// <param name="token">The authentication token.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="token"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <paramref name="token"/> is of zero length.
        /// </exception>
        internal AuthenticationToken(string token) {

            Guard.NotNullOrEmpty(token, "token");

            _token = token;
        }

        /// <summary>
        /// Gets the authentication token.
        /// </summary>
        internal string Token {

            get { return _token; }
        }

        #endregion
    }
}
