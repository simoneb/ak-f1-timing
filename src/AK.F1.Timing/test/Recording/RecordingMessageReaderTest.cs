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
using System.Threading;
using AK.F1.Timing.Messages.Feed;
using AK.F1.Timing.Messages.Session;
using Xunit;

namespace AK.F1.Timing.Recording
{
    public class RecordingMessageReaderTest
    {
        [Fact]
        public void reader_returns_messages_read_from_inner_reader()
        {
            var inner = CreatePopulatedInnerReader();
            var messages = inner.MessageQueue.ToArray();

            using(var reader = new RecordingMessageReader(inner, Stream.Null, false))
            {
                foreach(var expected in messages)
                {
                    Assert.Same(expected, reader.Read());
                }
                Assert.Null(reader.Read());
            }
        }

        [Fact]
        public void reader_records_messages_to_path()
        {
            var path = Path.GetTempFileName();

            using(var reader = new RecordingMessageReader(CreatePopulatedInnerReader(), path))
            {
                while(reader.Read() != null) {}
            }

            Assert.NotEqual(0L, new FileInfo(path).Length);

            File.Delete(path);
        }

        [Fact]
        public void reader_records_messages_to_output()
        {
            using(var output = new MemoryStream())
            {
                using(var reader = new RecordingMessageReader(CreatePopulatedInnerReader(), output, true))
                {
                    while(reader.Read() != null) {}
                    Assert.NotEqual(0L, output.Length);
                }
            }
        }

        [Fact]
        public void can_record_an_empty_inner_reader()
        {
            using(var output = new MemoryStream())
            {
                using(var reader = new RecordingMessageReader(CreateEmptyInnerReader(), output, true))
                {
                    Assert.Null(reader.Read());
                }
            }
        }

        [Fact]
        public void reader_does_not_return_the_set_next_message_delay_messages()
        {
            Message message;
            var path = Path.GetTempFileName();

            using(var reader = new RecordingMessageReader(CreatePopulatedInnerReader(), path))
            {
                while((message = reader.Read()) != null)
                {
                    Assert.IsNotType<SetNextMessageDelayMessage>(message);
                    Thread.Sleep(50);
                }
            }
        }

        [Fact]
        public void ctor_throws_if_inner_is_null()
        {
            var path = Path.GetTempFileName();

            try
            {
                Assert.Throws<ArgumentNullException>(() => new RecordingMessageReader(null, path));
                Assert.Throws<ArgumentNullException>(() => new RecordingMessageReader(null, Stream.Null, false));
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Fact]
        public void ctor_throws_if_path_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => new RecordingMessageReader(CreateEmptyInnerReader(), null));
        }

        [Fact]
        public void reader_creates_path_if_it_does_not_exist()
        {
            var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            using(new RecordingMessageReader(CreateEmptyInnerReader(), path)) {}

            Assert.True(File.Exists(path));

            File.Delete(path);
        }

        [Fact]
        public void reader_overwrites_and_truncates_path_if_it_exists()
        {
            var path = Path.GetTempFileName();

            using(var output = File.OpenWrite(path))
            {
                output.WriteByte(0);
            }

            Assert.DoesNotThrow(() => { using(new RecordingMessageReader(CreateEmptyInnerReader(), path)) {} });

            Assert.Equal(0L, new FileInfo(path).Length);

            File.Delete(path);
        }

        [Fact]
        public void path_cannot_be_opened_or_modified_while_reader_has_it_open()
        {
            var path = Path.GetTempFileName();

            using(var reader = new RecordingMessageReader(CreateEmptyInnerReader(), path))
            {
                Assert.Throws<IOException>(() => File.Delete(path));
                Assert.Throws<IOException>(() => File.OpenRead(path).Dispose());
                Assert.Throws<IOException>(() => File.OpenWrite(path).Dispose());
            }

            File.Delete(path);
        }

        [Fact]
        public void reader_closes_path_when_disposed()
        {
            var path = Path.GetTempFileName();

            using(new RecordingMessageReader(CreateEmptyInnerReader(), path)) {}

            Assert.DoesNotThrow(() => File.Delete(path));
        }

        [Fact]
        public void reader_disposes_if_inner_when_disposed()
        {
            var inner = CreateEmptyInnerReader();

            using(new RecordingMessageReader(inner, Stream.Null, false)) {}

            Assert.True(inner.IsDisposed);
        }

        [Fact]
        public void reader_disposes_of_input_when_disposed_if_it_owns_it()
        {
            using(var input = new MemoryStream())
            {
                using(new RecordingMessageReader(CreateEmptyInnerReader(), input, true)) {}
                Assert.Throws<ObjectDisposedException>(() => input.WriteByte(0));
            }
        }

        [Fact]
        public void reader_does_not_dispose_of_input_when_disposed_if_it_does_not_own_it()
        {
            using(var input = new MemoryStream())
            {
                using(new RecordingMessageReader(CreateEmptyInnerReader(), input, false)) {}
                Assert.DoesNotThrow(() => input.WriteByte(0));
            }
        }

        private static StubMessageReader CreateEmptyInnerReader()
        {
            return new StubMessageReader();
        }

        private static StubMessageReader CreatePopulatedInnerReader()
        {
            return new StubMessageReader(
                new SetSessionTypeMessage(SessionType.None, "none"),
                new SetSessionStatusMessage(SessionStatus.Green),
                new SetRaceLapNumberMessage(15)
                );
        }
    }
}