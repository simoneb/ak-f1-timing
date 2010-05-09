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
using Xunit.Extensions;

using AK.F1.Timing.Messages;
using AK.F1.Timing.Messages.Driver;
using AK.F1.Timing.Messages.Session;

namespace AK.F1.Timing.Live
{
    public partial class LiveMessageTranslatorTest
    {
        [Theory]
        [ClassData(typeof(AllSessionTypes))]
        public void car_number_column_values_are_translated_into_set_car_number_and_or_set_status_messages(SessionType session) {

            In(session).Assert(translator => {
                CompositeMessage composite;
                LiveDriver driver = translator.GetDriver(1);
                Message actual = translator.Translate(new SetGridColumnValueMessage(1, GridColumn.CarNumber, GridColumnColour.White, "1"));

                Assert.IsType<CompositeMessage>(actual);
                composite = (CompositeMessage)actual;
                Assert.Equal(2, composite.Messages.Count);
                Assert.MessagesAreEqual(new SetDriverCarNumberMessage(1, 1), composite.Messages[0]);
                Assert.MessagesAreEqual(new SetDriverStatusMessage(1, DriverStatus.OnTrack), composite.Messages[1]);
                Assert.Equal(DriverStatus.OnTrack, driver.Status);
                // Change the car number.
                Assert.MessagesAreEqual(
                    new SetDriverCarNumberMessage(1, 2),
                    translator.Translate(new SetGridColumnValueMessage(1, GridColumn.CarNumber, GridColumnColour.White, "2"))
                );
                // Change the status.
                Assert.MessagesAreEqual(
                    new SetDriverStatusMessage(1, DriverStatus.InPits),
                    translator.Translate(new SetGridColumnValueMessage(1, GridColumn.CarNumber, GridColumnColour.Red, "2"))
                );
                Assert.Equal(DriverStatus.InPits, driver.Status);
            });
        }

        [Theory]
        [ClassData(typeof(AllSessionTypes))]
        public void car_number_column_values_are_not_translated_into_set_car_number_or_set_status_messages_if_they_have_not_changed(SessionType session) {

            In(session).Assert(translator => {
                var driver = translator.GetDriver(1);

                driver.CarNumber = 1;
                driver.ChangeStatus(DriverStatus.InPits);
                Assert.Null(translator.Translate(new SetGridColumnValueMessage(1, GridColumn.CarNumber, GridColumnColour.Red, "1")));
            });
        }

        [Theory]
        [ClassData(typeof(AllSectorGridColumns_AllSessionTypes))]
        public void sector_column_values_are_translated_into_set_status_messages_if_the_status_has_changed(GridColumn sector, SessionType session) {

            In(session).Assert(translator => {
                SetGridColumnValueMessage message;
                LiveDriver driver = translator.GetDriver(1);                
                // Out.
                message = new SetGridColumnValueMessage(1, sector, GridColumnColour.White, "OUT");
                Assert.MessagesAreEqual(
                    new SetDriverStatusMessage(1, DriverStatus.Out),
                    translator.Translate(message)
                );
                Assert.Equal(DriverStatus.Out, driver.Status);
                Assert.Null(translator.Translate(message));
                // Stopped.
                message = new SetGridColumnValueMessage(1, sector, GridColumnColour.White, "STOP");
                Assert.MessagesAreEqual(
                    new SetDriverStatusMessage(1, DriverStatus.Stopped),
                    translator.Translate(message)
                );
                Assert.Equal(DriverStatus.Stopped, driver.Status);
                Assert.Null(translator.Translate(message));
            });
        }


        [Theory]
        [ClassData(typeof(AllSessionTypes))]
        public void lap_time_column_values_are_translated_into_set_status_messages_if_the_status_has_changed(SessionType session) {

            In(session).Assert(translator => {
                SetGridColumnValueMessage message;
                LiveDriver driver = translator.GetDriver(1);
                // On track. Note that OUT is displayed when a driver exits the pit and is on thier OUT lap.
                message = new SetGridColumnValueMessage(1, GridColumn.LapTime, GridColumnColour.White, "OUT");
                Assert.MessagesAreEqual(
                    new SetDriverStatusMessage(1, DriverStatus.OnTrack),
                    translator.Translate(message)
                );
                Assert.Equal(DriverStatus.OnTrack, driver.Status);
                Assert.Null(translator.Translate(message));
                // In pit.
                message = new SetGridColumnValueMessage(1, GridColumn.LapTime, GridColumnColour.White, "IN PIT");
                Assert.MessagesAreEqual(
                    new SetDriverStatusMessage(1, DriverStatus.InPits),
                    translator.Translate(message)
                );
                Assert.Equal(DriverStatus.InPits, driver.Status);
                Assert.Null(translator.Translate(message));
                // Retired.
                message = new SetGridColumnValueMessage(1, GridColumn.LapTime, GridColumnColour.White, "RETIRED");
                Assert.MessagesAreEqual(
                    new SetDriverStatusMessage(1, DriverStatus.Retired),
                    translator.Translate(message)
                );
                Assert.Equal(DriverStatus.Retired, driver.Status);
                Assert.Null(translator.Translate(message));
            });
        }
    }
}