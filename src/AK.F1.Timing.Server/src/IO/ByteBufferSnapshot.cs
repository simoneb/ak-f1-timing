// Copyright 2011 Andy Kernahan
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

namespace AK.F1.Timing.Server.IO
{
    /// <summary>
    /// Defines a snapshot of a <see cref="AK.F1.Timing.Server.IO.ByteBuffer"/>.
    /// </summary>
    /// <threadsafety static="true" instance="true"/>
    public struct ByteBufferSnapshot
    {
        #region Fields.

        private readonly int _offset;
        private readonly int _count;
        private readonly byte[] _buffer;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialsies a new instance of the <see cref="AK.F1.Timing.Server.IO.ByteBufferSnapshot"/>
        /// class and specifies the <paramref name="buffer"/>, <paramref name="offset"/> and
        /// <paramref name="count"/>.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The buffer offset.</param>
        /// <param name="count">The buffer length.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="buffer"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when
        /// <list type="bullet">
        /// <item>
        /// <paramref name="offset"/> is negative or greater than the length of <paramref name="buffer"/>
        /// </item>
        /// <item>
        /// <paramref name="count"/> is negative or greater than the length of <paramref name="buffer"/>
        /// given <paramref name="offset"/>
        /// </item>
        /// </list>
        /// </exception>
        public ByteBufferSnapshot(byte[] buffer, int offset, int count)
        {
            Guard.NotNull(buffer, "buffer");
            Guard.CheckBufferArgs(buffer, offset, count);

            _buffer = buffer;
            _offset = offset;
            _count = count;
        }

        /// <summary>
        /// Copies a specified number of bytes from this snapshot starting at a particular offset to a
        /// destination array starting at a particular offset.
        /// </summary>
        /// <param name="srcOffset">The zero-based byte offset.</param>
        /// <param name="dst">The destination buffer.</param>
        /// <param name="dstOffset">The zero-based offset into <paramref name="dst"/>.</param>
        /// <param name="count">The number of bytes to copy.</param>        
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="dst"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <paramref name="srcOffset"/>, <paramref name="dstOffset"/> or <paramref name="count"/>
        /// is not valid.        
        /// </exception>
        public void CopyTo(int srcOffset, byte[] dst, int dstOffset, int count)
        {
            Buffer.BlockCopy(_buffer, _offset + srcOffset, dst, dstOffset, count);
        }

        /// <summary>
        /// Gets the number of bytes in this snapshot.
        /// </summary>
        public int Count
        {
            get { return _count; }
        }

        #endregion
    }
}
