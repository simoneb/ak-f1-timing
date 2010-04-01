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
using Xunit;

namespace AK.F1.Timing.Messages.Session
{
    public class EndOfSessionMessageTest : MessageTestBase<EndOfSessionMessage>
    {
        [Fact]
        public override void can_create() {

            Assert.NotNull(EndOfSessionMessage.Instance);
        }

        [Fact]
        public void maintains_identity_when_serialized() {

            Assert.Same(EndOfSessionMessage.Instance, EndOfSessionMessage.Instance.DeepClone());
        }

        [Fact]
        public override void can_visit() {

            var message = CreateMessage();
            var visitor = CreateMockMessageVisitor();

            visitor.Setup(x => x.Visit(message));
            message.Accept(visitor.Object);
            visitor.VerifyAll();
        }

        protected override EndOfSessionMessage CreateMessage() {

            return EndOfSessionMessage.Instance;
        }
    }
}
