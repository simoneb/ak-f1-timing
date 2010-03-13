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
    public class SetPingIntervalMessageTest : MessageTestBase<SetPingIntervalMessage>
    {
        [Fact]
        public override void can_create() {

            var message = CreateMessage();

            Assert.Equal(TimeSpan.FromMilliseconds(1), message.PingInterval);
        }

        [Fact]
        public override void can_visit() {

            var message = CreateMessage();
            var visitor = CreateMockMessageVisitor();

            visitor.Setup(x => x.Visit(message));
            message.Accept(visitor.Object);
            visitor.VerifyAll();
        }

        [Fact]
        public void ctor_throws_if_ping_interval_is_negative() {

            Assert.DoesNotThrow(() => {
                new SetPingIntervalMessage(TimeSpan.Zero);
            });
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                new SetPingIntervalMessage(TimeSpan.FromMilliseconds(-1));
            });
        }

        protected override SetPingIntervalMessage CreateMessage() {

            return new SetPingIntervalMessage(TimeSpan.FromMilliseconds(1));
        }
    }
}
