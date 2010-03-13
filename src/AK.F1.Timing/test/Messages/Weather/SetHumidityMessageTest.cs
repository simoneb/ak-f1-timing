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

namespace AK.F1.Timing.Messages.Weather
{
    public class SetHumidityMessageTest : MessageTestBase<SetHumidityMessage>
    {
        [Fact]
        public override void can_create() {

            var message = CreateMessage();

            Assert.Equal(1D, message.Humidity);
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
        public void ctor_throws_if_humidity_is_negative_or_greater_than_100() {

            Assert.DoesNotThrow(() => {
                new SetHumidityMessage(0D);
            });
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                new SetHumidityMessage(-1D);
            });
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                new SetHumidityMessage(100 + Double.Epsilon);
            });
        }

        protected override SetHumidityMessage CreateMessage() {

            return new SetHumidityMessage(1D);
        } 
    }
}
