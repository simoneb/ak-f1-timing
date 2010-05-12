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

namespace AK.F1.Timing.Messages.Driver
{
    public class SetGridColumnValueMessageTest : MessageTestBase<SetGridColumnValueMessage>
    {
        [Fact]
        public override void can_create()
        {
            var message = CreateMessage();

            Assert.Equal(1, message.DriverId);
            Assert.Equal(GridColumnColour.Blue, message.Colour);
            Assert.Equal(GridColumn.DriverName, message.Column);
            Assert.Equal("Value", message.Value);
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
        public void clear_column_is_true_when_value_is_null()
        {
            var message = new SetGridColumnValueMessage(1, GridColumn.CarNumber, GridColumnColour.Black, null);

            Assert.True(message.ClearColumn);
            message = new SetGridColumnValueMessage(1, GridColumn.CarNumber, GridColumnColour.Black, string.Empty);
            Assert.False(message.ClearColumn);
            message = new SetGridColumnValueMessage(1, GridColumn.CarNumber, GridColumnColour.Black, "L. HAMILTON");
            Assert.False(message.ClearColumn);
        }

        protected override SetGridColumnValueMessage CreateMessage()
        {
            return new SetGridColumnValueMessage(1, GridColumn.DriverName, GridColumnColour.Blue, "Value");
        }
    }
}