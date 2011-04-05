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

using Xunit;

namespace AK.F1.Timing.Messages.Session
{
    public class StopSessionTimeCountdownMessageTest : MessageTestBase<StopSessionTimeCountdownMessage>
    {
        [Fact]
        public void maintains_identity_when_serialized()
        {
            Assert.Same(StopSessionTimeCountdownMessage.Instance,
                StopSessionTimeCountdownMessage.Instance.DeepClone());
        }

        [Fact]
        public override void can_visit()
        {
            var message = CreateTestMessage();
            var visitor = CreateMockMessageVisitor();

            visitor.Setup(x => x.Visit(message));
            message.Accept(visitor.Object);
            visitor.VerifyAll();
        }

        protected override void AssertEqualsTestMessage(StopSessionTimeCountdownMessage message)
        {
            Assert.Same(StopSessionTimeCountdownMessage.Instance, message);
        }

        protected override StopSessionTimeCountdownMessage CreateTestMessage()
        {
            return StopSessionTimeCountdownMessage.Instance;
        }
    }
}