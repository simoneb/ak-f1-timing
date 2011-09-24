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
using System.Linq;
using System.Net;
using System.Net.Sockets;
using AK.F1.Timing.Server.IO;
using Xunit;

namespace AK.F1.Timing.Server.Proxy
{
    public class ProxySessionTest
    {
        private const int PollTimeout = 250000;

        [Fact]
        public void proxy_disposes_of_client()
        {
            using(var client = CreateSocket())
            {
                var session = new ProxySession(1, client);
                ((IDisposable)session).Dispose();
                Assert.Throws<ObjectDisposedException>(() => client.Poll(PollTimeout, SelectMode.SelectRead));
            }
        }

        [Fact]
        public void proxy_raises_disposed_event_once_when_disposed()
        {
            using(var client = CreateSocket())
            {
                int disposedCount = 0;
                var session = new ProxySession(1, client);
                session.Disposed += delegate { ++disposedCount; };
                ((IDisposable)session).Dispose();
                ((IDisposable)session).Dispose();
                Assert.Equal(1, disposedCount);
            }
        }

        [Fact]
        public void can_send_a_buffer()
        {
            using(var ctx = new TestContext())
            using(var session = new ProxySession(0, ctx.Output))
            {
                var expected = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

                session.SendAsync(new[] { expected });

                var actual = new byte[expected.Length];
                Assert.Equal(ctx.Input.Receive(actual), expected.Length);
                Assert.Equal(expected, actual);
                Assert.False(ctx.Input.Poll(PollTimeout, SelectMode.SelectRead));
            }
        }

        [Fact]
        public void can_send_a_buffer_snapshot()
        {
            using(var ctx = new TestContext())
            using(var session = new ProxySession(0, ctx.Output))
            {
                var expected = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

                session.SendAsync(new ByteBufferSnapshot(expected, 0, expected.Length));

                var actual = new byte[expected.Length];
                Assert.Equal(ctx.Input.Receive(actual), expected.Length);
                Assert.Equal(expected, actual);
                Assert.False(ctx.Input.Poll(PollTimeout, SelectMode.SelectRead));
            }
        }

        [Fact]
        public void can_send_a_partial_buffer_snapshot()
        {
            using(var ctx = new TestContext())
            using(var session = new ProxySession(0, ctx.Output))
            {
                var buffer = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
                var expected = new byte[] { 6, 7, 8, 9 };

                session.SendAsync(new ByteBufferSnapshot(buffer, 5, 4));

                var actual = new byte[expected.Length];
                Assert.Equal(ctx.Input.Receive(actual), expected.Length);
                Assert.Equal(expected, actual);
                Assert.False(ctx.Input.Poll(PollTimeout, SelectMode.SelectRead));
            }
        }

        [Fact]
        public void can_send_multiple_buffers()
        {
            using(var ctx = new TestContext())
            using(var session = new ProxySession(0, ctx.Output))
            {
                var buffers = new[] {
                    new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
                    new byte[] { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 },
                    new byte[] { 21, 22, 23, 24, 25, 26, 27, 28, 29, 30 },
                    new byte[] { 31, 32, 33, 34, 35, 36, 37, 38, 39, 40 }
                };
                var expected = buffers.SelectMany(x => x).ToArray();

                session.SendAsync(buffers);

                var actual = new byte[expected.Length];
                Assert.Equal(ctx.Input.Receive(actual), expected.Length);
                Assert.Equal(expected, actual);
                Assert.False(ctx.Input.Poll(PollTimeout, SelectMode.SelectRead));
            }
        }

        [Fact]
        public void can_send_a_buffer_larger_than_the_internal_output_buffer()
        {
            using(var ctx = new TestContext())
            using(var session = new ProxySession(0, ctx.Output))
            {
                var expected = new byte[(int)(ProxySession.OutputBufferSize * 2.3)];
                for(int i = 0; i < expected.Length; ++i)
                {
                    expected[i] = (byte)i;
                }

                session.SendAsync(new[] { expected });

                int actualRead = 0;
                var actual = new byte[expected.Length];
                while(actualRead < actual.Length && ctx.Input.Poll(PollTimeout, SelectMode.SelectRead))
                {
                    actualRead += ctx.Input.Receive(actual, actualRead, actual.Length - actualRead, SocketFlags.None);
                }
                Assert.Equal(expected.Length, actualRead);
                Assert.Equal(expected, actual);
                Assert.False(ctx.Input.Poll(PollTimeout, SelectMode.SelectRead));
            }
        }

        [Fact]
        public void ctor_throws_if_client_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => new ProxySession(0, null));
        }

        [Fact]
        public void ctor_sets_id()
        {
            using(var session = new ProxySession(123, CreateSocket()))
            {
                Assert.Equal(123, session.Id);
            }
        }

        [Fact]
        public void send_throws_if_buffer_is_null()
        {
            using(var session = new ProxySession(123, CreateSocket()))
            {
                Assert.Throws<ArgumentNullException>(() => session.SendAsync(null));
            }
        }

        private static Socket CreateSocket(AddressFamily addressFamily = AddressFamily.InterNetwork)
        {
            return new Socket(addressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        private sealed class TestContext : IDisposable
        {
            private static readonly IPEndPoint TestEndpoint = new IPEndPoint(IPAddress.Loopback, 50192);

            public TestContext()
            {
                using(var server = CreateSocket(TestEndpoint.AddressFamily))
                {
                    server.Bind(TestEndpoint);
                    server.Listen(1);
                    Output = CreateSocket(TestEndpoint.AddressFamily);
                    Output.BeginConnect(TestEndpoint, delegate { }, null);
                    Input = server.Accept();
                }
            }

            public void Dispose()
            {
                DisposeOf(Input);
                DisposeOf(Output);
            }

            private static void DisposeOf(Socket socket)
            {
                if(socket != null)
                {
                    try
                    {
                        socket.Dispose();
                    }
                    catch { }
                }
            }

            public Socket Output { get; private set; }

            public Socket Input { get; private set; }
        }
    }
}