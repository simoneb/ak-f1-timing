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
using AK.F1.Timing.Live.Encryption;
using AK.F1.Timing.Live.IO;
using Moq;
using Xunit;

namespace AK.F1.Timing.Live
{
    public class LiveMessageReaderTest
    {
        [Fact]
        public void ctor_throws_if_message_stream_endpoint_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => { new LiveMessageReader(null, new Mock<IDecrypterFactory>().Object); });
        }

        [Fact]
        public void ctor_throws_if_decrypter_factory_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => { new LiveMessageReader(new Mock<IMessageStreamEndpoint>().Object, null); });
        }
    }
}