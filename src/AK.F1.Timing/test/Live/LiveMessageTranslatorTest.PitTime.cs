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

using AK.F1.Timing.Messages.Driver;
using AK.F1.Timing.Messages.Session;

namespace AK.F1.Timing.Live
{
    public partial class LiveMessageTranslatorTest
    {
        [Fact]
        public void when_a_driver_is_pitted_and_the_pit_count_column_is_updated_we_expect_pit_time_updates() {

            In(SessionType.Race).Assert(translator => {
                var driver = translator.GetDriver(1);

                driver.ChangeStatus(DriverStatus.InPits);
                translator.Translate(new SetGridColumnValueMessage(1, GridColumn.PitCount, GridColumnColour.White, "2"));
                Assert.True(driver.IsExpectingPitTimes);
            });
        }

        [Fact]
        public void when_a_driver_is_not_pitted_and_the_pit_count_column_is_updated_we_do_not_expect_pit_time_updates() {

            In(SessionType.Race).Assert(translator => {
                var driver = translator.GetDriver(1);
                var message = new SetGridColumnValueMessage(1, GridColumn.PitCount, GridColumnColour.White, "2");
                // On track.
                driver.ChangeStatus(DriverStatus.OnTrack);
                translator.Translate(message);
                Assert.False(driver.IsExpectingPitTimes);
                // Out.
                driver.ChangeStatus(DriverStatus.Out);
                translator.Translate(message);
                Assert.False(driver.IsExpectingPitTimes);
                // Retired.
                driver.ChangeStatus(DriverStatus.Retired);
                translator.Translate(message);
                Assert.False(driver.IsExpectingPitTimes);
                // Stopped.
                driver.ChangeStatus(DriverStatus.Stopped);
                translator.Translate(message);
                Assert.False(driver.IsExpectingPitTimes);
            });
        }

        [Theory]
        [ClassData(typeof(AllSessionTypesExceptRace))]
        public void when_a_driver_is_pitted_and_the_pit_count_column_is_updated_we_do_not_expect_pit_time_updates(SessionType session) {

            In(session).Assert(translator => {
                var driver = translator.GetDriver(1);

                driver.ChangeStatus(DriverStatus.InPits);
                translator.Translate(new SetGridColumnValueMessage(1, GridColumn.PitCount, GridColumnColour.White, "2"));
                Assert.False(driver.IsExpectingPitTimes);
            });
        }

        [Fact]
        public void when_a_driver_is_expecting_pit_times_sector_3_column_values_are_translated_into_set_pit_time_messages() {

            In(SessionType.Race).Assert(translator => {
                var driver = translator.GetDriver(1);

                driver.IsExpectingPitTimes = true;
                driver.LapNumber = 5;
                Assert.MessagesAreEqual(
                    // Note that we expect the time for lap 4, not 5, as the lap number reflects the current lap.
                    new SetDriverPitTimeMessage(1, new PostedTime(TimeSpan.FromSeconds(23.8), PostedTimeType.Normal, 4)),
                    translator.Translate(new SetGridColumnValueMessage(1, GridColumn.S3, GridColumnColour.White, "23.8"))
                );
                Assert.False(driver.IsExpectingPitTimes);
            });
        }

        [Fact]
        public void when_a_driver_is_expecting_pit_times_sector_1_and_2_column_updates_are_not_translated() {

            In(SessionType.Race).Assert(translator => {
                translator.GetDriver(1).IsExpectingPitTimes = true;
                Assert.Null(translator.Translate(new SetGridColumnValueMessage(1, GridColumn.S1, GridColumnColour.White, "23.8")));
                Assert.Null(translator.Translate(new SetGridColumnValueMessage(1, GridColumn.S2, GridColumnColour.White, "23.8")));
            });
        }
    }
}