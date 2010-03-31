// Copyright 2010 Andy Kernahan
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
using Moq;
using Xunit;

namespace AK.F1.Timing.Live.Encryption
{
    public class AuthenticationTokenTest : TestBase
    {
        [Fact]
        public void can_create() {

            var token = new AuthenticationToken("token");

            Assert.Equal("token", token.Token);
        }

        [Fact]
        public void cannot_serialize_token() {

            Assert.False(typeof(AuthenticationToken).IsSerializable);
        }

        [Fact]
        public void implements_equality_contract() {

            Assert.EqualityContract(
                new[] { new AuthenticationToken("token"), new AuthenticationToken("token") },
                new[] { new AuthenticationToken("token"), new AuthenticationToken("Token") }
            );
        }

        [Fact]
        public void ctor_throws_if_token_is_null() {

            Assert.Throws<ArgumentNullException>(() => new AuthenticationToken(null));
        }

        [Fact]
        public void ctor_throws_if_token_is_empty() {

            Assert.Throws<ArgumentException>(() => new AuthenticationToken(string.Empty));
        }
    }
}
