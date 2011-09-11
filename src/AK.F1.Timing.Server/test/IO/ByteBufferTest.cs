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

namespace AK.F1.Timing.Server.IO
{
    public class ByteBufferTest
    {
        [Fact]
        public void ctor_throws_if_capacity_is_not_positive()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ByteBuffer(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ByteBuffer(-1));
        }

        [Fact]
        public void append_throws_if_buffer_is_null()
        {
            var buffer = new ByteBuffer(1);

            Assert.Throws<ArgumentNullException>(() => buffer.Append(null));
        }

        [Fact]
        public void can_append_buffer()
        {
            var buffer = new ByteBuffer(1);

            buffer.Append(CreateArray(0));
            Assert.Equal(buffer.Count, 0);
            AssertDataIsContinuous(buffer);

            buffer.Append(CreateArray(1));
            Assert.Equal(buffer.Count, 1);
            AssertDataIsContinuous(buffer);

            buffer.Append(CreateArray(16, dataStart: buffer.Count));
            Assert.Equal(buffer.Count, 17);
            AssertDataIsContinuous(buffer);
        }

        [Fact]
        public void append_doubles_capacity_until_it_can_satisfy_the_buffer()
        {
            var buffer = new ByteBuffer(1);

            buffer.Append(CreateArray(1));
            Assert.Equal(1, buffer.Capacity);

            buffer.Append(CreateArray(1));
            Assert.Equal(2, buffer.Capacity);

            buffer.Append(CreateArray(10));
            Assert.Equal(16, buffer.Capacity);
        }

        [Fact]
        public void can_create_snapshot()
        {
            var buffer = new ByteBuffer(1);

            var snapshot = buffer.CreateSnapshot();
            Assert.Equal(0, snapshot.Count);

            buffer.Append(CreateArray(1));
            snapshot = buffer.CreateSnapshot();
            Assert.Equal(1, snapshot.Count);
            AssertDataIsContinuous(buffer);
        }

        [Fact]
        public void snapshots_are_not_affected_by_changes_buffer_changes()
        {
            var buffer = new ByteBuffer(1);

            var snapshot = buffer.CreateSnapshot();
            Assert.Equal(0, snapshot.Count);

            buffer.Append(CreateArray(1));
            Assert.Equal(0, snapshot.Count);
        }

        [Fact]
        public void count_property_reflects_the_number_of_buffered_bytes()
        {
            var buffer = new ByteBuffer(1);
            Assert.Equal(0, buffer.Count);

            buffer.Append(CreateArray(10));
            Assert.Equal(10, buffer.Count);
        }

        [Fact]
        public void capacity_property_reflects_the_buffer_capacity()
        {
            var buffer = new ByteBuffer(8);
            Assert.Equal(8, buffer.Capacity);
        }

        private void AssertDataIsContinuous(ByteBuffer buffer)
        {
            var snapshot = buffer.CreateSnapshot();
            Assert.Equal(buffer.Count, snapshot.Count);

            var actual = new byte[snapshot.Count];
            snapshot.CopyTo(0, actual, 0, actual.Length);
            Assert.Equal(CreateArray(snapshot.Count), actual);
        }

        private static byte[] CreateArray(int size, int dataStart = 0)
        {
            var array = new byte[size];
            for(int i = 0; i < size; ++i)
            {
                array[i] = checked((byte)(dataStart + i));
            }
            return array;
        }
    }
}
