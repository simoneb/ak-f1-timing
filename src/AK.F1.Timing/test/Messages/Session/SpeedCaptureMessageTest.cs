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
using System.Collections.Generic;
using Xunit;

namespace AK.F1.Timing.Messages.Session
{
    public class SpeedCaptureMessageTest : MessageTestBase<SpeedCaptureMessage>
    {
        [Fact]
        public void ctor_throws_if_speeds_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => new SpeedCaptureMessage(SpeedCaptureLocation.S1, null));
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

        [Fact]
        public void speeds_is_readonly()
        {
            Assert.True(CreateTestMessage().Speeds.IsReadOnly);
        }

        protected override void AssertEqualsTestMessage(SpeedCaptureMessage message)
        {
            Assert.Equal(SpeedCaptureLocation.S2, message.Location);
            Assert.Equal(1, message.Speeds.Count);
            Assert.Equal("VET", message.Speeds[0].Key);
            Assert.Equal(123, message.Speeds[0].Value);
        }

        protected override SpeedCaptureMessage CreateTestMessage()
        {
            return new SpeedCaptureMessage(SpeedCaptureLocation.S2, new[] { new KeyValuePair<string, int>("VET", 123) });
        }
    }
}