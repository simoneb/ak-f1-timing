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

using AK.F1.Timing.Messages.Weather;
using Xunit;

namespace AK.F1.Timing.Messages
{
    public class CompositeMessageTest : MessageTestBase<CompositeMessage>
    {
        [Fact]
        public override void can_create()
        {
            var message = CreateMessage();

            Assert.NotNull(message.Messages);
            Assert.Equal(2, message.Messages.Count);
            Assert.Same(Message.Empty, message.Messages[0]);
            Assert.Same(Message.Empty, message.Messages[1]);
        }

        [Fact]
        public override void can_visit()
        {
            var visitor = CreateMockMessageVisitor();
            var message = new CompositeMessage(
                new SetAirTemperatureMessage(28.3),
                new SetTrackTemperatureMessage(35.4));

            visitor.Setup(x => x.Visit((SetAirTemperatureMessage)message.Messages[0]));
            visitor.Setup(x => x.Visit((SetTrackTemperatureMessage)message.Messages[1]));
            message.Accept(visitor.Object);
            visitor.VerifyAll();
        }

        protected override CompositeMessage CreateMessage()
        {
            return new CompositeMessage(Message.Empty, Message.Empty);
        }
    }
}