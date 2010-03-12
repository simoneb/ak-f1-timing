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
using Xunit;

using AK.F1.Timing.Extensions;

namespace AK.F1.Timing.Extensions
{
    public class StreamExtensionsTest
    {
        [Fact]
        public void fully_read_returns_true_if_count_is_zero() {

            var buffer = new byte[1];
            var stream = CreateStream(buffer.Length);

            Assert.True(StreamExtensions.FullyRead(stream, buffer, 0, 0));
        }

        [Fact]
        public void fully_read_returns_true_if_all_data_was_written_to_buffer() {

            var buffer = new byte[10];
            var stream = CreateStream(buffer.Length);

            Assert.True(StreamExtensions.FullyRead(stream, buffer, 0, buffer.Length));
        }

        [Fact]
        public void fully_read_copies_data_to_buffer_and_leaves_stream_in_correct_position() {

            var buffer = new byte[10];
            var stream = CreateStream(buffer.Length);

            Assert.True(StreamExtensions.FullyRead(stream, buffer, 0, buffer.Length));

            for(int i = 0; i < buffer.Length; ++i) {
                Assert.Equal(i, buffer[i]);
            }

            Assert.Equal(stream.Position, buffer.Length);
        }

        [Fact]
        public void fully_read_returns_false_if_end_of_stream_was_reached_before_count_was_written_to_buffer() {

            var buffer = new byte[10];
            var stream = CreateStream(buffer.Length / 2);

            Assert.False(StreamExtensions.FullyRead(stream, buffer, 0, buffer.Length));
        }        

        private static Stream CreateStream(int length) {

            if(length > byte.MaxValue) {
                throw new ArgumentOutOfRangeException("length");
            }

            var stream = new MemoryStream(length);

            for(int i = 0; i < length; ++i) {
                stream.WriteByte((byte)i);
            }

            stream.Position = 0L;

            return stream;
        }
    }
}
