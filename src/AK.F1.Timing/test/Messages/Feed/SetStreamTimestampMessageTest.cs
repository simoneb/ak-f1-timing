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

namespace AK.F1.Timing.Messages.Feed
{
    public class SetStreamTimestampMessageTest : MessageTestBase<SetStreamTimestampMessage>
    {
        private static readonly SetStreamTimestampMessage TestMessage = new SetStreamTimestampMessage();

        [Fact]
        public override void can_visit()
        {
            var message = CreateTestMessage();
            var visitor = CreateMockMessageVisitor();

            visitor.Setup(x => x.Visit(message));
            message.Accept(visitor.Object);
            visitor.VerifyAll();
        }

        [Fact]
        public void ctor_sets_timestamp_to_utc_now()
        {
            var expected = DateTime.UtcNow;
            var actual = new SetStreamTimestampMessage().Timestamp;

            Assert.Equal(expected.Kind, actual.Kind);
            Assert.InRange(actual, expected, expected.AddMilliseconds(1));
        }

        protected override void AssertEqualsTestMessage(SetStreamTimestampMessage message)
        {
            Assert.Equal(TestMessage.Timestamp, message.Timestamp);
        }

        protected override SetStreamTimestampMessage CreateTestMessage()
        {
            return TestMessage;
        }
    }
}