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
    public class SetAtmosphericPressureMessageTest : MessageTestBase<SetAtmosphericPressureMessage>
    {
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
        public void ctor_throws_if_pressure_is_not_positive()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => { new SetAtmosphericPressureMessage(0D); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { new SetAtmosphericPressureMessage(-1D); });
        }

        protected override void AssertEqualsTestMessage(SetAtmosphericPressureMessage message)
        {
            Assert.Equal(1D, message.Pressure);
        }

        protected override SetAtmosphericPressureMessage CreateTestMessage()
        {
            return new SetAtmosphericPressureMessage(1D);
        }
    }
}