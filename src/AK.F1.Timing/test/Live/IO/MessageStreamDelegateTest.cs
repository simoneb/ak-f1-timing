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
using Moq;
using Xunit;

namespace AK.F1.Timing.Live.IO
{
    public class MessageStreamDelegateTest
    {
        [Fact]
        public void inner_is_disposed_when_delegate_is_disposed()
        {
            using(var inner = new MemoryStream())
            {
                var @delegate = new MessageStreamDelegate(inner);
                ((IDisposable)@delegate).Dispose();
                Assert.Throws<ObjectDisposedException>(() => inner.ReadByte());
            }
        }

        [Fact]
        public void fill_delegates_to_inner()
        {
            var buffer = new byte[10];
            var inner = new Mock<Stream>(MockBehavior.Strict);
            var @delegate = new MessageStreamDelegate(inner.Object);

            inner.Setup(x => x.Read(buffer, 0, buffer.Length))
                .Returns(buffer.Length)
                .Verifiable();
            @delegate.Fill(buffer, 0, buffer.Length);
            inner.VerifyAll();
        }

        [Fact]
        public void fill_returns_true_when_count_has_been_read()
        {
            var buffer = new byte[10];

            using(var inner = new MemoryStream(buffer))
            {
                var @delegate = new MessageStreamDelegate(inner);
                Assert.True(@delegate.Fill(buffer, 0, buffer.Length));
            }
        }

        [Fact]
        public void fill_returns_false_when_count_has_not_been_read()
        {
            var buffer = new byte[10];

            using(var inner = new MemoryStream(new byte[5]))
            {
                var @delegate = new MessageStreamDelegate(inner);
                Assert.False(@delegate.Fill(buffer, 0, buffer.Length));
            }
        }

        [Fact]
        public void fill_copies_data_to_buffer()
        {
            var actual = new byte[10];
            var expected = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            using(var inner = new MemoryStream(expected))
            {
                var @delegate = new MessageStreamDelegate(inner);
                Assert.True(@delegate.Fill(actual, 0, actual.Length));
                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void fill_copies_count_elements_to_buffer_starting_at_offset()
        {
            var actual = new byte[9];
            var data = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var expected = new byte[] { 0, 1, 2, 3, 4, 5, 6, 0, 0 };

            using(var inner = new MemoryStream(data))
            {
                var @delegate = new MessageStreamDelegate(inner);
                Assert.True(@delegate.Fill(actual, 1, 6));
                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void fill_throws_if_delegate_has_been_disposed()
        {
            using(var inner = new MemoryStream())
            {
                var @delegate = new MessageStreamDelegate(inner);
                ((IDisposable)@delegate).Dispose();
                Assert.Throws<ObjectDisposedException>(() => @delegate.Fill(new byte[1], 0, 1));
            }
        }

        [Fact]
        public void can_determine_if_delegate_has_been_disposed()
        {
            using(var inner = new MemoryStream())
            {
                var @delegate = new MessageStreamDelegate(inner);
                Assert.False(@delegate.IsDisposed);
                ((IDisposable)@delegate).Dispose();
                Assert.True(@delegate.IsDisposed);
            }
        }

        [Fact]
        public void ctor_throws_if_inner_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => new MessageStreamDelegate(null));
        }
    }
}