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
    /// Provides an append only byte buffer that supports the efficient creation of snapshots.
    /// This class cannot be inherited.
    /// </summary>
    public sealed class ByteBuffer
    {
        #region Fields.

        private int _count;
        private byte[] _buffer;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises new instance of the <see cref="AK.F1.Timing.Server.IO.ByteBuffer"/> class
        /// and specifies the initial buffer capacity.
        /// </summary>
        /// <param name="initialCapacity">The initial buffer cpacity.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="initialCapacity"/> is not positive.
        /// </exception>
        public ByteBuffer(int initialCapacity)
        {
            Guard.InRange(initialCapacity > 0, "initialCapacity");

            _buffer = new byte[initialCapacity];
        }

        /// <summary>
        /// Appends the specified <paramref name="buffer"/> to this buffer.
        /// </summary>
        /// <param name="buffer">The buffer to append.</param>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when this instance has been disposed of.
        /// </exception>
        /// <exception cref="System.OverflowException">
        /// Thrown when the buffer is unable to be appended.
        /// </exception>
        public void Append(byte[] buffer)
        {
            Guard.NotNull(buffer, "buffer");

            EnsureCapacity(buffer.Length);
            Buffer.BlockCopy(buffer, 0, _buffer, _count, buffer.Length);
            _count += buffer.Length;
        }

        /// <summary>
        /// Creates a snapshot of this buffer.
        /// </summary>
        /// <returns>A snapshot of this buffer.</returns>
        public ByteBufferSnapshot CreateSnapshot()
        {
            return new ByteBufferSnapshot(_buffer, 0, _count);
        }

        /// <summary>
        /// Gets the number of bytes in this buffer.
        /// </summary>
        public int Count
        {
            get { return _count; }
        }

        /// <summary>
        /// Gets the capacity this buffer.
        /// </summary>
        public int Capacity
        {
            get { return _buffer.Length; }
        }

        #endregion

        #region Private Impl.

        private void EnsureCapacity(int count)
        {
            checked
            {
                int requiredCapacity = _count + count;
                if(requiredCapacity > _buffer.Length)
                {
                    int newSize = _buffer.Length * 2;
                    while(newSize < requiredCapacity)
                    {
                        newSize *= 2;
                    }
                    ResizeBuffer(newSize);
                }
            }
        }

        private void ResizeBuffer(int newSize)
        {
            // Even though Array.Resize allocates, copies then assigns we require this behaviour
            // to support snapshots and therefore cannot rely on a private implementation detail.
            var newArray = new byte[newSize];
            Buffer.BlockCopy(_buffer, 0, newArray, 0, _count);
            _buffer = newArray;
        }

        #endregion
    }
}
