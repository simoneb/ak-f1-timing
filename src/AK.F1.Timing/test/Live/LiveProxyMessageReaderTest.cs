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
using System.Net;
using Xunit;

namespace AK.F1.Timing.Live
{
    public class LiveProxyMessageReaderTest
    {
        private static readonly IPEndPoint TestEndpoint = new IPEndPoint(IPAddress.Any, 50192);
        private static readonly AuthenticationToken TestToken = new AuthenticationToken("token");

        [Fact]
        public void ctor_throws_if_endpoint_if_null()
        {
            Assert.Throws<ArgumentNullException>(() => new LiveProxyMessageReader(null, TestToken));
        }

        [Fact]
        public void ctor_throws_if_token_if_null()
        {
            Assert.Throws<ArgumentNullException>(() => new LiveProxyMessageReader(TestEndpoint, null));
        }

        [Fact]
        public void read_throws_if_cannot_connect_to_endpoint()
        {
            using(var reader = CreateTestReader())
            {
                Assert.Throws<IOException>(() => reader.Read());
            }
        }

        private static LiveProxyMessageReader CreateTestReader()
        {
            return new LiveProxyMessageReader(TestEndpoint, TestToken);
        }
    }
}