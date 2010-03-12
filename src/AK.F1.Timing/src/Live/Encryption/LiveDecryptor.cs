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

namespace AK.F1.Timing.Live.Encryption
{    
    /// <summary>
    /// An <see cref="AK.F1.Timing.Live.Encryption.IDecryptor"/> implementation which implements
    /// the 2009 decryption algorithm. This class cannot be inherited.
    /// </summary>
    public sealed class LiveDecryptor : IDecryptor
    {
        #region Private Fields.

        private const int DEFAULT_MASK = 1431655765;
        
        private int _mask;
        private int _seed;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="LiveDecryptor"/> class and specifies the
        /// initial hash <paramref name="seed"/>
        /// </summary>
        /// <param name="seed">The initial hash seed.</param>
        public LiveDecryptor(int seed) {

            _seed = seed;
            _mask = DEFAULT_MASK;
        }

        /// <inheritdoc />
        public void Decrypt(byte[] buffer, int offset, int count) {

            Guard.CheckBufferArgs(buffer, offset, count);

            if(_seed == 0) {
                return;
            }

            for(int i = offset, end = offset + count; i < end; ++i) {
                _mask = _mask >> 1 & 0x7FFFFFFF ^ ((_mask & 0x1) == 1 ? _seed : 0);
                buffer[i] = (byte)(buffer[i] ^ _mask & 0xFF);
            }
        }

        /// <inheritdoc />
        public void Reset() {

            _mask = DEFAULT_MASK;
        }

        #endregion
    }
}
