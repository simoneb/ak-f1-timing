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

namespace AK.F1.Timing.Live.IO
{
    public class LiveMessageStreamEndpointTest
    {
        [Fact]
        public void can_open_stream()
        {
            var endpoint = new LiveMessageStreamEndpoint();

            using(var stream = endpoint.Open())
            {
                Assert.NotNull(stream);
            }
        }

        [Fact]
        public void can_read_from_stream()
        {
            var buffer = new byte[1];
            var endpoint = new LiveMessageStreamEndpoint();

            using(var stream = endpoint.Open())
            {
                Assert.True(stream.Fill(buffer, 0, buffer.Length));
            }
        }

        [Fact]
        public void can_open_zero_keyframe()
        {
            var endpoint = new LiveMessageStreamEndpoint();

            using(var stream = endpoint.OpenKeyframe(0))
            {
                Assert.NotNull(stream);
            }
        }

        [Fact]
        public void can_open_first_keyframe()
        {
            var endpoint = new LiveMessageStreamEndpoint();

            using(var stream = endpoint.OpenKeyframe(1))
            {
                Assert.NotNull(stream);
            }
        }

        [Fact]
        public void can_read_from_keyframe_stream()
        {
            var buffer = new byte[1];
            var endpoint = new LiveMessageStreamEndpoint();

            using(var stream = endpoint.OpenKeyframe(0))
            {
                Assert.True(stream.Fill(buffer, 0, buffer.Length));
            }
        }

        [Fact]
        public void open_keyframe_throws_if_keyframe_does_not_exist()
        {
            var endpoint = new LiveMessageStreamEndpoint();

            Assert.Throws<IOException>(() => endpoint.OpenKeyframe(int.MaxValue).Dispose());
        }

        [Fact]
        public void open_keyframe_throws_if_keyframe_is_negative()
        {
            var endpoint = new LiveMessageStreamEndpoint();

            Assert.Throws<ArgumentOutOfRangeException>(() => endpoint.OpenKeyframe(-1).Dispose());
        }
    }
}