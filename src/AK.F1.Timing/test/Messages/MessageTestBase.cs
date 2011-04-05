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
using System.IO;
using AK.F1.Timing.Messages.Driver;
using AK.F1.Timing.Serialization;
using Moq;
using Xunit;

namespace AK.F1.Timing.Messages
{
    public abstract class MessageTestBase<TMessage> : TestBase where TMessage : Message
    {
        public static readonly PostedTime PostedTime = new PostedTime(
            new TimeSpan(0, 0, 1, 55, 350),
            PostedTimeType.PersonalBest,
            12);

        public static readonly Gap Gap = new LapGap(1);

        [Fact]
        public abstract void can_visit();

        [Fact]
        public void accept_throws_if_visitor_is_null()
        {
            var message = CreateTestMessage();

            Assert.Throws<ArgumentNullException>(() => { message.Accept(null); });
        }

        [Fact]
        public void can_create()
        {
            AssertEqualsTestMessage(CreateTestMessage());
        }

        [Fact]
        public virtual void can_serialize()
        {
            using(var stream = new MemoryStream())
            {
                using(var writer = new DecoratedObjectWriter(stream))
                {
                    writer.Write(CreateTestMessage());
                }
                stream.Position = 0L;
                using(var reader = new DecoratedObjectReader(stream))
                {
                    AssertEqualsTestMessage(reader.Read<TMessage>());
                }
            }
        }

        protected abstract TMessage CreateTestMessage();

        protected abstract void AssertEqualsTestMessage(TMessage message);

        protected Mock<IMessageVisitor> CreateMockMessageVisitor()
        {
            return new Mock<IMessageVisitor>(MockBehavior.Strict);
        }
    }
}