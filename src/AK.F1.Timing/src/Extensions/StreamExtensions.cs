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
using System.IO;

namespace AK.F1.Timing.Extensions
{
    /// <summary>
    /// <see cref="System.IO.Stream"/> extension class. This class is <see langword="static"/>.
    /// </summary>
    public static class StreamExtensions
    {
        #region Public Interface.

        /// <summary>
        /// Reads a sequences of bytes the stream and advances the position within the stream by
        /// the number read. If the end of the stream is reached before <paramref name="count"/>
        /// bytes are read, this method will return <see langword="false"/>, otherwise it will
        /// return <see langword="true"/> which indicates that exactly <paramref name="count"/> bytes
        /// have been written to the specified <paramref name="buffer"/>.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="buffer">The buffer into which the read data is written.</param>
        /// <param name="offset">The offset in <paramref name="buffer"/> at which writing begins.</param>
        /// <param name="count">The exact number of bytes to read.</param>
        /// <returns><see langword="true"/> if the specified number of bytes were read into
        /// <paramref name="buffer"/>, otherwise; <see langword="false"/> to indicate that the end
        /// of the stream was reached.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="buffer"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when
        /// <list type="bullet">
        /// <item>
        /// <paramref name="offset"/> is negative or greater than the length
        /// of <paramref name="buffer"/>
        /// </item>
        /// <item>
        /// <paramref name="count"/> is negative or greater than the length of
        /// <paramref name="buffer"/> given <paramref name="offset"/>.
        /// </item>
        /// </list>
        /// </exception>
        public static bool FullyRead(this Stream stream, byte[] buffer, int offset, int count) {

            Guard.NotNull(stream, "stream");

            if(count == 0) {
                return true;
            }

            int read;

            do {
                if((read = stream.Read(buffer, offset, count)) == 0)
                    return false;
                count -= read;
                offset += read;
            } while(count > 0);

            return true;
        }

        #endregion
    }
}
