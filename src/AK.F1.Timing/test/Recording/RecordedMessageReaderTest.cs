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

namespace AK.F1.Timing.Recording
{
    public class RecordedMessageReaderTest
    {
        [Fact]
        public void ctor_throws_if_a_input_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => new RecordedMessageReader(null, false));
        }

        [Fact]
        public void reader_closes_path_when_disposed()
        {
            var path = Path.GetTempFileName();

            using(var reader = new RecordedMessageReader(path)) {}

            Assert.DoesNotThrow(() => File.Delete(path));
        }

        [Fact]
        public void path_be_opened_whilst_reader_has_it_open()
        {
            var path = Path.GetTempFileName();

            using(var reader = new RecordedMessageReader(path))
            {
                Assert.DoesNotThrow(() => File.OpenRead(path).Dispose());
            }

            File.Delete(path);
        }

        [Fact]
        public void path_cannot_be_modified_whilst_reader_has_it_open()
        {
            var path = Path.GetTempFileName();

            using(var reader = new RecordedMessageReader(path))
            {
                Assert.Throws<IOException>(() => File.Delete(path));
                Assert.Throws<IOException>(() => File.OpenWrite(path).Dispose());
            }

            File.Delete(path);
        }

        [Fact]
        public void reader_closes_input_when_disposed_if_it_owns_it()
        {
            using(var input = new MemoryStream())
            {
                using(var reader = new RecordedMessageReader(input, true)) {}
                Assert.Throws<ObjectDisposedException>(() => input.WriteByte(0));
            }
        }

        [Fact]
        public void reader_does_not_close_input_when_disposed_if_it_does_not_own_it()
        {
            using(var input = new MemoryStream())
            {
                using(var reader = new RecordedMessageReader(input, false)) {}
                Assert.DoesNotThrow(() => input.WriteByte(0));
            }
        }
    }
}