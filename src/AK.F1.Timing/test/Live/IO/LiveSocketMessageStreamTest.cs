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
using System.Net.Sockets;
using Xunit;

namespace AK.F1.Timing.Live.IO
{
    public class LiveSocketMessageStreamTest
    {
        [Fact]
        public void ctor_throws_if_socket_is_null() {

            Assert.Throws<ArgumentNullException>(() => new LiveSocketMessageStream(null));
        }

        [Fact]
        public void socket_is_disposed_when_delegate_is_disposed() {

            using(var socket = CreateSocket()) {
                var stream = new LiveSocketMessageStream(socket);
                ((IDisposable)stream).Dispose();
                Assert.Throws<ObjectDisposedException>(() => socket.Poll(1, SelectMode.SelectRead));
            }
        }

        [Fact]
        public void fully_read_throws_if_stream_has_been_disposed() {

            using(var socket = CreateSocket()) {
                var stream = new LiveSocketMessageStream(socket);
                ((IDisposable)stream).Dispose();
                Assert.Throws<ObjectDisposedException>(() => stream.FullyRead(new byte[1], 0, 1));
            }
        }

        [Fact]
        public void can_determine_if_stream_has_been_disposed() {

            using(var socket = CreateSocket()) {
                var stream = new LiveSocketMessageStream(socket);
                Assert.False(stream.IsDisposed);
                ((IDisposable)stream).Dispose();
                Assert.True(stream.IsDisposed);
            }
        }

        [Fact]
        public void socket_is_pinged_when_no_data_has_been_received_during_interval() {


        }

        [Fact]
        public void socket_is_not_pinged_when_data_has_been_received_during_interval() {


        }

        private static Socket CreateSocket() {

            return null;
        }
    }
}
