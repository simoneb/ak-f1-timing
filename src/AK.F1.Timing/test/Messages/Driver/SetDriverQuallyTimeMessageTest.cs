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

namespace AK.F1.Timing.Messages.Driver
{
    public class SetDriverQuallyTimeMessageTest : MessageTestBase<SetDriverQuallyTimeMessage>
    {
        [Fact]
        public override void can_create()
        {
            var message = CreateMessage();

            Assert.Equal(1, message.DriverId);
            Assert.Equal(1, message.QuallyNumber);
            Assert.Equal(PostedTime.Time, message.QuallyTime);
        }

        [Fact]
        public override void can_visit()
        {
            var message = CreateMessage();
            var visitor = CreateMockMessageVisitor();

            visitor.Setup(x => x.Visit(message));
            message.Accept(visitor.Object);
            visitor.VerifyAll();
        }

        [Fact]
        public void ctor_throws_if_qually_time_is_not_positive()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => { new SetDriverQuallyTimeMessage(1, 1, TimeSpan.Zero); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { new SetDriverQuallyTimeMessage(1, 1, TimeSpan.FromMilliseconds(-1)); });
        }

        [Fact]
        public void ctor_throws_if_qually_number_is_not_positive()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => { new SetDriverQuallyTimeMessage(1, 0, PostedTime.Time); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { new SetDriverQuallyTimeMessage(1, -1, PostedTime.Time); });
        }

        protected override SetDriverQuallyTimeMessage CreateMessage()
        {
            return new SetDriverQuallyTimeMessage(1, 1, PostedTime.Time);
        }
    }
}