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
using System.Runtime.Serialization;
using Xunit;

namespace AK.F1.Timing.Serialization
{
    public class DecoratedObjectReaderTest
    {
        [Fact]
        public void ctor_throws_if_input_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => new DecoratedObjectReader(null));
        }

        [Fact]
        public void input_is_not_closed_when_reader_is_disposed()
        {
            var input = new MemoryStream();

            using(var reader = new DecoratedObjectReader(input)) {}

            Assert.DoesNotThrow(() => input.Position = 0);
            input.Dispose();
            Assert.Throws<ObjectDisposedException>(() => input.Position = 0);
        }

        [Fact]
        public void read_throws_when_reader_has_been_disposed()
        {
            var reader = new DecoratedObjectReader(new MemoryStream());

            ((IDisposable)reader).Dispose();

            Assert.Throws<ObjectDisposedException>(() => reader.Read());
        }

        [Fact]
        public void read_throws_when_end_of_input_has_been_reached()
        {
            using(var input = new MemoryStream())
            {
                using(var reader = new DecoratedObjectReader(input))
                {
                    Assert.Throws<SerializationException>(() => reader.Read());
                }
            }
        }
    }
}