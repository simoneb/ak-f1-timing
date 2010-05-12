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
using System.Collections.Generic;
using System.IO;
using AK.F1.Timing.Messages.Weather;
using Xunit;

namespace AK.F1.Timing
{
    public class MessageReaderBaseTest
    {
        [Fact]
        public void read_impl_should_not_be_invoked_again_having_previously_thrown_an_exception_and_same_exception_should_be_re_thrown()
        {
            var reader = new ThrowingMessageReader();

            Assert.Throws<IOException>(() => reader.Read());
            Assert.Throws<IOException>(() => reader.Read());
        }

        [Fact]
        public void read_impl_should_not_be_invoked_again_having_previously_returned_null()
        {
            var reader = new NullMessageReader();

            Assert.Null(reader.Read());
            Assert.Null(reader.Read());
        }

        [Fact]
        public void read_impl_should_be_invoked_again_have_previously_returned_an_empty_message()
        {
            var expected = new SetIsWetMessage(true);
            var reader = new StubMessageReader(Message.Empty, Message.Empty, expected);

            Assert.Same(expected, reader.Read());
        }

        [Fact]
        public void read_should_throw_when_it_has_been_reader_has_been_disposed()
        {
            var reader = new StubMessageReader();

            reader.Dispose();

            Assert.Throws<ObjectDisposedException>(() => reader.Read());
        }

        [Fact]
        public void is_disposed_should_reflect_the_disposed_state_of_the_reader()
        {
            var reader = new StubMessageReader();

            Assert.False(reader.IsDisposed);
            reader.Dispose();
            Assert.True(reader.IsDisposed);
        }

        private sealed class StubMessageReader : MessageReaderBase
        {
            private readonly Queue<Message> _messages;

            public StubMessageReader(params Message[] messages)
            {
                _messages = new Queue<Message>(messages);
            }

            public void Dispose()
            {
                ((IDisposable)this).Dispose();
            }

            protected override Message ReadImpl()
            {
                return _messages.Count > 0 ? _messages.Dequeue() : null;
            }
        }

        private sealed class ThrowingMessageReader : MessageReaderBase
        {
            private bool _readImplCalled;

            protected override Message ReadImpl()
            {
                Assert.False(_readImplCalled, "ReadImpl has already been called.");

                _readImplCalled = true;

                throw new IOException();
            }
        }

        private sealed class NullMessageReader : MessageReaderBase
        {
            private bool _readImplCalled;

            protected override Message ReadImpl()
            {
                Assert.False(_readImplCalled, "ReadImpl has already been called.");

                _readImplCalled = true;

                return null;
            }
        }
    }
}