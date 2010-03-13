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
    public class DriverMessageBaseTest : TestBase
    {
        [Fact]
        public void can_create() {

            var message = new DriverMessageStub(1);

            Assert.Equal(1, message.DriverId);
        }

        [Fact]
        public void ctor_throws_if_driver_id_is_not_positive() {

            Assert.Throws<ArgumentOutOfRangeException>(() => {
                new DriverMessageStub(0);
            });
            Assert.Throws<ArgumentOutOfRangeException>(() => {
                new DriverMessageStub(-1);
            });
        }

        private sealed class DriverMessageStub : DriverMessageBase
        {
            public DriverMessageStub(int driverId) : base(driverId) { }

            public override void Accept(IMessageVisitor visitor) {

                throw new NotImplementedException();
            }
        }
    }
}
