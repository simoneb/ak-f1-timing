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
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using AK.F1.Timing.Messages.Weather;
using AK.F1.Timing.Serialization;
using Xunit;

namespace AK.F1.Timing.Proxy
{
    public class ProxyMessageReaderTest : TestBase
    {
        private static readonly IPEndPoint TestEndpoint = new IPEndPoint(IPAddress.Loopback, 25000);

        [Fact]
        public void ctor_throws_if_endpoint_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => new ProxyMessageReader(null));
        }

        [Fact]
        public void reader_reads_messages_sent_from_endpoint_until_a_null_mesage_is_read()
        {
            using(var server = new Server())
            using(var reader = new ProxyMessageReader(TestEndpoint))
            {
                foreach(var expected in Server.Messages)
                {
                    Assert.MessagesAreEqual(expected, reader.Read());
                }
                Assert.Null(reader.Read());
            }
        }

        [Fact]
        public void reader_throws_if_unable_to_connect_to_endpoint()
        {
            Assert.Throws<IOException>(() =>
            {
                using(var reader = new ProxyMessageReader(TestEndpoint))
                {
                    reader.Read();
                }
            });
        }

        private sealed class Server : IDisposable
        {
            private readonly TcpListener _lisenter;

            public static readonly IList<Message> Messages = Array.AsReadOnly(new Message[] {
                new SetAirTemperatureMessage(1),
                new SetAtmosphericPressureMessage(2),
                new SetHumidityMessage(3),
                new SetIsWetMessage(true),
                new SetTrackTemperatureMessage(4),
                new SetWindSpeedMessage(5)
            });

            public Server()
            {
                _lisenter = new TcpListener(TestEndpoint);
                _lisenter.Start();
                _lisenter.BeginAcceptTcpClient(Accept, null);
            }

            private void Accept(IAsyncResult iar)
            {
                using(var client = _lisenter.EndAcceptTcpClient(iar))
                using(var writer = new DecoratedObjectWriter(client.GetStream()))
                {
                    foreach(var message in Messages)
                    {
                        writer.WriteMessage(message);
                    }
                    writer.Write(null);
                }
            }

            void IDisposable.Dispose()
            {
                _lisenter.Stop();
            }
        }
    }
}