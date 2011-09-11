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
using Xunit;
using Xunit.Extensions;

namespace AK.F1.Timing.Server.IO
{
    public class ByteBufferSnapshotTest
    {
        [Fact]
        public void can_copy_snapshot_data()
        {
            var actual = new byte[10];
            var snapshot = Create(10, 0, 10);

            snapshot.CopyTo(0, actual, 0, actual.Length);
            Assert.Equal(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }, actual);
            // count
            actual = new byte[10];
            snapshot.CopyTo(0, actual, 0, 5);
            Assert.Equal(new byte[] { 1, 2, 3, 4, 5, 0, 0, 0, 0, 0 }, actual);
            // dstOffset
            actual = new byte[10];
            snapshot.CopyTo(0, actual, 5, 5);
            Assert.Equal(new byte[] { 0, 0, 0, 0, 0, 1, 2, 3, 4, 5 }, actual);
            // srcOffset
            actual = new byte[10];
            snapshot.CopyTo(5, actual, 0, 5);
            Assert.Equal(new byte[] { 6, 7, 8, 9, 10, 0, 0, 0, 0, 0 }, actual);
            // internal offset
            actual = new byte[10];
            snapshot = Create(10, 5, 5);
            snapshot.CopyTo(0, actual, 0, 5);
            Assert.Equal(new byte[] { 6, 7, 8, 9, 10, 0, 0, 0, 0, 0 }, actual);
        }

        [Fact]
        public void copy_to_throws_if_args_are_not_valid()
        {
            var snapshot = Create(10, 0, 10);
            var dst = new byte[5];

            Assert.Throws<ArgumentNullException>(() => snapshot.CopyTo(0, null, 0, 0));
            // srcOffset
            Assert.Throws<ArgumentException>(() => snapshot.CopyTo(10, dst, 0, 1));
            // dstOffset
            Assert.Throws<ArgumentException>(() => snapshot.CopyTo(0, dst, 5, 1));
            // count
            Assert.Throws<ArgumentException>(() => snapshot.CopyTo(0, dst, 0, 6));
            // dstOffset + count
            Assert.Throws<ArgumentException>(() => snapshot.CopyTo(0, dst, 4, 2));
        }

        [Fact]
        public void count_property_reflects_the_number_of_buffered_bytes()
        {
            var snapshot = Create(1, 0, 1);
            Assert.Equal(1, snapshot.Count);
        }

        [Fact]
        public void ctor_throws_if_buffer_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => new ByteBufferSnapshot(null, 0, 0));
        }

        [Theory]
        [InlineData(1, 2, 0)]
        [InlineData(1, 0, 2)]
        [InlineData(1, 1, 2)]
        public void ctor_throws_if_offset_or_count_is_not_valid(int bufferSize, int offset, int count)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Create(bufferSize, offset, count));
        }

        [Theory]
        [InlineData(1, 0, 0)]
        [InlineData(1, 1, 0)]
        [InlineData(1, 0, 1)]
        [InlineData(2, 1, 1)]
        public void ctor_does_not_throws_if_offset_and_count_are_valid(int bufferSize, int offset, int count)
        {
            Assert.DoesNotThrow(() => Create(bufferSize, offset, count));
        }

        private static ByteBufferSnapshot Create(int bufferSize, int offset, int count)
        {
            var buffer = new byte[bufferSize];
            for(int i = 0; i < bufferSize; ++i)
            {
                buffer[i] = checked((byte)(i + 1));
            }
            return new ByteBufferSnapshot(buffer, offset, count);
        }
    }
}
