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
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Xunit;

namespace AK.F1.Timing.Live.IO
{
    public class LiveSocketMessageStreamTest
    {
        [Fact]
        public void ctor_throws_if_socket_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => new LiveSocketMessageStream(null));
        }

        [Fact]
        public void ping_interval_throws_if_value_is_negative()
        {
            using(var context = CreateTestContext())
            {
                Assert.DoesNotThrow(() => context.Stream.PingInterval = TimeSpan.Zero);
                Assert.Throws<ArgumentOutOfRangeException>(() => context.Stream.PingInterval = TimeSpan.FromMilliseconds(-1));
            }
        }

        [Fact]
        public void socket_is_disposed_when_delegate_is_disposed()
        {
            using(var context = CreateTestContext())
            {
                ((IDisposable)context.Stream).Dispose();
                Assert.Throws<ObjectDisposedException>(() => context.Local.Poll(1, SelectMode.SelectRead));
            }
        }

        [Fact]
        public void fully_read_throws_if_stream_has_been_disposed()
        {
            using(var context = CreateTestContext())
            {
                ((IDisposable)context.Stream).Dispose();
                Assert.Throws<ObjectDisposedException>(() => context.Stream.FullyRead(new byte[1], 0, 1));
            }
        }

        [Fact]
        public void can_determine_if_stream_has_been_disposed()
        {
            using(var context = CreateTestContext())
            {
                Assert.False(context.Stream.IsDisposed);
                ((IDisposable)context.Stream).Dispose();
                Assert.True(context.Stream.IsDisposed);
            }
        }

        [Fact]
        public void socket_is_pinged_when_no_data_has_been_read_during_interval()
        {
            var buffer = new byte[1];
            var pingWindow = TimeSpan.FromMilliseconds(5);            

            using(var context = CreateTestContext())
            {
                Action read = () =>
                {
                    context.Stream.FullyRead(buffer, 0, buffer.Length);
                };
                read.BeginInvoke(null, null);
                for(int i = 0; i < 10; ++i)
                {
                    Thread.Sleep(context.Stream.PingInterval);
                    Assert.Equal(1, context.Remote.Receive(buffer));
                    Assert.Equal(0, context.Remote.Available);
                }
            }
        }

        [Fact]
        public void socket_is_not_pinged_when_data_has_been_read_during_interval()
        {
            var buffer = new byte[1];
            var pingWindow = TimeSpan.FromMilliseconds(5);

            using(var context = CreateTestContext())
            {
                for(int i = 0; i < 10; ++i)
                {
                    context.Remote.Send(buffer);
                    Assert.True(context.Stream.FullyRead(buffer, 0, buffer.Length));
                    Thread.Sleep(context.Stream.PingInterval - pingWindow);
                    Assert.Equal(0, context.Remote.Available);
                }
            }
        }

        [Fact]
        public void fully_read_returns_true_when_count_has_been_read()
        {
            var buffer = new byte[10];

            using(var context = CreateTestContext())
            {
                Assert.Equal(buffer.Length, context.Remote.Send(buffer));
                Assert.True(context.Stream.FullyRead(buffer, 0, buffer.Length));
            }
        }

        [Fact(Skip = "This never returns as the socket is not closed.")]
        public void fully_read_returns_false_when_count_has_not_been_read()
        {
            var buffer = new byte[10];

            using(var context = CreateTestContext())
            {
                Assert.Equal(5, context.Remote.Send(buffer, 0, 5, SocketFlags.None));
                Assert.False(context.Stream.FullyRead(buffer, 0, buffer.Length));
            }
        }

        [Fact]
        public void fully_read_copies_data_to_buffer()
        {
            var actual = new byte[10];
            var expected = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            using(var context = CreateTestContext())
            {
                Assert.Equal(expected.Length, context.Remote.Send(expected));
                Assert.True(context.Stream.FullyRead(actual, 0, actual.Length));
                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void fully_read_copies_data_to_buffer_repeatedly()
        {
            int repeat = 5;
            var actual = new byte[25];
            var data = Enumerable.Range(0, actual.Length * 100).Select(x => (byte)x).ToArray();

            using(var context = CreateTestContext())
            {
                while(repeat-- > 0)
                {
                    Assert.Equal(data.Length, context.Remote.Send(data));
                    for(int i = 0; i < data.Length; i += actual.Length)
                    {
                        Assert.True(context.Stream.FullyRead(actual, 0, actual.Length));
                        Assert.Equal(Enumerable.Range(i, actual.Length).Select(x => (byte)x).ToArray(), actual);
                    }
                }
            }
        }

        [Fact]
        public void fully_read_copies_count_elements_to_buffer_starting_at_offset()
        {
            var actual = new byte[9];
            var data = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var expected = new byte[] { 0, 1, 2, 3, 4, 5, 6, 0, 0 };

            using(var context = CreateTestContext())
            {
                Assert.Equal(data.Length, context.Remote.Send(data));
                Assert.True(context.Stream.FullyRead(actual, 1, 6));
                Assert.Equal(expected, actual);
            }
        }

        private static TestContext CreateTestContext()
        {
            var endpoint = new IPEndPoint(IPAddress.Loopback, 50192);
            var server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var context = new TestContext
            {
                Local = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            };

            server.Bind(endpoint);
            server.Listen(1);

            var connectAsyncResult = context.Local.BeginConnect(endpoint, null, null);

            context.Remote = server.Accept();
            context.Remote.NoDelay = true;
            ((IDisposable)server).Dispose();
            // Even though accept blocks the connect async op will not have completed.
            connectAsyncResult.AsyncWaitHandle.WaitOne();
            context.Stream = new LiveSocketMessageStream(context.Local)
            {
                PingInterval = TimeSpan.FromMilliseconds(50d)
            };

            return context;
        }

        private sealed class TestContext : IDisposable
        {
            public void Dispose()
            {
                ((IDisposable)Stream).Dispose();
                ((IDisposable)Local).Dispose();
                ((IDisposable)Remote).Dispose();
            }

            public Socket Local { get; set; }

            public Socket Remote { get; set; }

            public LiveSocketMessageStream Stream { get; set; }
        }
    }
}