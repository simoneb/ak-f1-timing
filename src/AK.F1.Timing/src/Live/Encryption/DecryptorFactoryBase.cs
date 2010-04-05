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
using System.Collections.Generic;

namespace AK.F1.Timing.Live.Encryption
{    
    /// <summary>
    /// Defines a base class for <see cref="AK.F1.Timing.Live.Encryption.IDecryptorFactory"/>
    /// implementations which provides seed caching capabilities. This class is
    /// <see langword="abstract"/>.
    /// </summary>
    /// <remarks>
    /// Sessions identifers are compared using the <see cref="System.StringComparer.Ordinal"/>
    /// comparer.
    /// </remarks>
    [Serializable]
    public abstract class DecryptorFactoryBase : IDecryptorFactory
    {
        #region Private Fields.

        private log4net.ILog _log;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Defines the default seed. This field is constant.
        /// </summary>
        public const int DefaultSeed = 0;

        /// <inheritdoc />
        public virtual IDecryptor Create() {

            Log.Info("creating decryptor with default seed");

            return CreateWithSeed(DecryptorFactoryBase.DefaultSeed);
        }

        /// <inheritdoc />
        public IDecryptor Create(string sessionId) {

            Guard.NotNullOrEmpty(sessionId, "sessionId");

            int seed;

            if(!SeedCache.TryGetValue(sessionId, out seed)) {
                seed = GetSeedForSession(sessionId);
                SeedCache.Add(sessionId, seed);
            } else {
                Log.InfoFormat("cache hit for session {0} with seed {1}", sessionId, seed);
            }

            Log.InfoFormat("creating decryptor for session {0} with seed {1}", sessionId, seed);

            return CreateWithSeed(seed);
        }

        #endregion

        #region Protected Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="DecryptorFactoryBase"/> class.
        /// </summary>
        protected DecryptorFactoryBase() {

            SeedCache = new Dictionary<string, int>(StringComparer.Ordinal);
        }

        /// <summary>
        /// When overriden in a derived class, gets the decryption seed for the this with
        /// the specified identifier.
        /// </summary>
        /// <param name="sessionId">The identifier of the this to return the decryption seed
        /// for.</param>
        /// <returns>The decryption seed for the this with the specified identifier.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="sessionId"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <paramref name="sessionId"/> is empty.
        /// </exception>
        protected abstract int GetSeedForSession(string sessionId);

        /// <summary>
        /// When overriden in a derived class; creates a new
        /// <see cref="AK.F1.Timing.Live.Encryption.IDecryptor"/> using the specified
        /// <paramref name="seed"/>.
        /// </summary>
        /// <param name="seed">The decryption seed.</param>
        /// <returns>A new <see cref="AK.F1.Timing.Live.Encryption.IDecryptor"/> using the specified
        /// <paramref name="seed"/>.</returns>
        protected abstract IDecryptor CreateWithSeed(int seed);        

        /// <summary>
        /// Gets the <see cref="log4net.ILog"/> for this type.
        /// </summary>
        protected log4net.ILog Log {

            get {
                if(_log == null) {
                    _log = log4net.LogManager.GetLogger(GetType());
                }
                return _log;
            }
        }

        #endregion

        #region Private Impl.

        private IDictionary<string, int> SeedCache { get; set; }

        #endregion
    }
}
