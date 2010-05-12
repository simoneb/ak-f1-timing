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

namespace AK.F1.Timing.Live.Encryption
{
    /// <summary>
    /// Defines a decrypter.
    /// </summary>
    public interface IDecrypter
    {
        /// <summary>
        /// Decrypts the specified buffer in place.
        /// </summary>
        /// <param name="buffer">The buffer from which the encrypted data is read and the decrypted
        /// data is written.</param>
        /// <param name="offset">The offset in <paramref name="buffer"/> at which decryption
        /// begins.</param>
        /// <param name="count">The exact number of bytes to decrypt.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="buffer"/> is <see langword="null"/>.
        /// </exception>
        void Decrypt(byte[] buffer, int offset, int count);

        /// <summary>
        /// Resets the state of the decrypter.
        /// </summary>
        void Reset();
    }
}