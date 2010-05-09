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

using AK.F1.Timing.Messages.Driver;

namespace AK.F1.Timing.Live
{
    public class LiveDriverTest : TestBase
    {
        [Fact]
        public void can_create() {

            var driver = new LiveDriver(1);

            Assert.Equal(1, driver.Id);
            assert_properties_have_default_values(driver);
        }

        [Fact]
        public void can_change_the_drivers_status() {

            var driver = new LiveDriver(1);

            driver.ChangeStatus(DriverStatus.OnTrack);

            Assert.Equal(DriverStatus.OnTrack, driver.Status);
        }

        [Fact]
        public void pitting_the_driver_sets_the_current_sector_number_to_one() {

            var driver = new LiveDriver(1);

            driver.ChangeStatus(DriverStatus.OnTrack);
            driver.CurrentSectorNumber = 2;
            driver.ChangeStatus(DriverStatus.InPits);

            Assert.Equal(1, driver.CurrentSectorNumber);
        }

        [Fact]
        public void can_compute_the_drivers_lap_number() {

            var driver = new LiveDriver(1);

            Assert.Equal(10, driver.ComputeLapNumber(10));
            driver.LastGapMessage = new SetDriverGapMessage(1, LapGap.Zero);
            Assert.Equal(10, driver.ComputeLapNumber(10));
            driver.LastGapMessage = new SetDriverGapMessage(1, new LapGap(2));
            Assert.Equal(8, driver.ComputeLapNumber(10));
            // I think this is sensible if the gap is greater than the race lap number.
            driver.LastGapMessage = new SetDriverGapMessage(1, new LapGap(20));
            Assert.Equal(0, driver.ComputeLapNumber(10));

            // TimeGaps should be ignored.
            driver.LastGapMessage = new SetDriverGapMessage(1, TimeGap.Zero);
            Assert.Equal(10, driver.ComputeLapNumber(10));
            driver.LastGapMessage = new SetDriverGapMessage(1, new TimeGap(TimeSpan.FromDays(1D)));
            Assert.Equal(10, driver.ComputeLapNumber(10));
        }

        [Fact]
        public void compute_race_lap_number_throws_if_race_lap_number_is_negative() {

            var driver = new LiveDriver(1);

            Assert.DoesNotThrow(() => driver.ComputeLapNumber(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => driver.ComputeLapNumber(-1));
        }

        [Fact]
        public void can_get_and_set_which_column_has_a_value() {

            var driver = new LiveDriver(1);

            foreach(GridColumn column in Enum.GetValues(typeof(GridColumn))) {
                Assert.False(driver.ColumnHasValue(column));
                driver.SetColumnHasValue(column, true);
                Assert.True(driver.ColumnHasValue(column));
                driver.SetColumnHasValue(column, false);
                Assert.False(driver.ColumnHasValue(column));
                driver.SetColumnHasValue(column, true);
            }
        }

        [Fact]
        public void can_determine_if_the_driver_is_the_race_leader() {

            var driver = new LiveDriver(1);

            Assert.False(driver.IsRaceLeader);
            driver.Position = 1;
            Assert.True(driver.IsRaceLeader);
            driver.Position = 2;
            Assert.False(driver.IsRaceLeader);
        }

        [Fact]
        public void can_determine_if_the_driver_is_expecting_pit_time_updates() {

            var driver = new LiveDriver(1);

            Assert.False(driver.IsExpectingPitTimes);
            driver.ExpectedPitTimeCount = 1;
            Assert.True(driver.IsExpectingPitTimes);
            driver.ExpectedPitTimeCount = 0;
            Assert.False(driver.IsExpectingPitTimes);
        }

        [Fact]
        public void can_determine_if_the_driver_is_on_the_track() {

            var driver = new LiveDriver(1);

            driver.ChangeStatus(DriverStatus.OnTrack);
            Assert.True(driver.IsOnTrack);

            driver.ChangeStatus(DriverStatus.InPits);
            Assert.False(driver.IsOnTrack);            
            driver.ChangeStatus(DriverStatus.Out);
            Assert.False(driver.IsOnTrack);
            driver.ChangeStatus(DriverStatus.Retired);
            Assert.False(driver.IsOnTrack);
            driver.ChangeStatus(DriverStatus.Stopped);
            Assert.False(driver.IsOnTrack);
        }

        [Fact]
        public void can_determine_if_the_driver_is_in_the_pits() {

            var driver = new LiveDriver(1);

            driver.ChangeStatus(DriverStatus.InPits);
            Assert.True(driver.IsInPits);

            driver.ChangeStatus(DriverStatus.OnTrack);
            Assert.False(driver.IsInPits);            
            driver.ChangeStatus(DriverStatus.Out);
            Assert.False(driver.IsInPits);
            driver.ChangeStatus(DriverStatus.Retired);
            Assert.False(driver.IsInPits);
            driver.ChangeStatus(DriverStatus.Stopped);
            Assert.False(driver.IsInPits);
        }

        [Fact]
        public void can_determine_if_a_sector_number_is_the_one_currently_being_completed() {

            var driver = new LiveDriver(1);

            driver.CurrentSectorNumber = 1;
            Assert.True(driver.IsCurrentSectorNumber(1));
            driver.CurrentSectorNumber = 2;
            Assert.True(driver.IsCurrentSectorNumber(2));
            driver.CurrentSectorNumber = 3;
            Assert.True(driver.IsCurrentSectorNumber(3));
        }

        [Fact]
        public void is_current_sector_number_returns_false_if_the_current_sector_has_not_been_set() {

            var driver = new LiveDriver(1);

            Assert.False(driver.IsCurrentSectorNumber(1));
            Assert.False(driver.IsCurrentSectorNumber(2));
            Assert.False(driver.IsCurrentSectorNumber(3));
        }

        [Fact]
        public void is_current_sector_number_returns_false_if_sector_number_is_out_of_range() {

            var driver = new LiveDriver(1);

            for(int i = 1; i <= 3; ++i) {
                driver.CurrentSectorNumber = i;
                Assert.False(driver.IsCurrentSectorNumber(0));
                Assert.False(driver.IsCurrentSectorNumber(4));
            }
        }

        [Fact]
        public void can_determine_if_a_sector_number_is_the_one_previously_completed() {

            var driver = new LiveDriver(1);

            driver.CurrentSectorNumber = 1;
            Assert.True(driver.IsPreviousSectorNumber(3));
            driver.CurrentSectorNumber = 2;
            Assert.True(driver.IsPreviousSectorNumber(1));
            driver.CurrentSectorNumber = 3;
            Assert.True(driver.IsPreviousSectorNumber(2));
        }

        [Fact]
        public void is_previous_sector_number_returns_false_if_the_current_sector_has_not_been_set() {

            var driver = new LiveDriver(1);

            Assert.False(driver.IsPreviousSectorNumber(1));
            Assert.False(driver.IsPreviousSectorNumber(2));
            Assert.False(driver.IsPreviousSectorNumber(3));
        }

        [Fact]
        public void is_previous_sector_number_returns_false_if_sector_number_is_out_of_range() {

            var driver = new LiveDriver(1);

            for(int i = 1; i <= 3; ++i) {
                driver.CurrentSectorNumber = i;
                Assert.False(driver.IsPreviousSectorNumber(0));
                Assert.False(driver.IsPreviousSectorNumber(4));
            }
        }

        [Fact]
        public void can_get_the_drivers_previous_sector_number() {

            var driver = new LiveDriver(1);

            driver.CurrentSectorNumber = 1;
            Assert.Equal(3, driver.PreviousSectorNumber);

            driver.CurrentSectorNumber = 2;
            Assert.Equal(1, driver.PreviousSectorNumber);

            driver.CurrentSectorNumber = 3;
            Assert.Equal(2, driver.PreviousSectorNumber);
        }

        [Fact]
        public void can_reset_the_driver_state() {

            var driver = new LiveDriver(1);

            driver.CarNumber = 21;
            driver.ExpectedPitTimeCount = 3;
            driver.LapNumber = 4;            
            driver.LastGapMessage = new SetDriverGapMessage(1, LapGap.Zero);
            driver.LastIntervalMessage = new SetDriverIntervalMessage(1, LapGap.Zero);
            driver.LastLapTime = new PostedTime(TimeSpan.FromSeconds(90), PostedTimeType.Normal, 3);
            for(int i = 0; i < driver.LastSectors.Length; ++i) {
                driver.LastSectors[i] = driver.LastLapTime;
            }
            driver.Name = "Name";
            driver.CurrentSectorNumber = 2;
            driver.ExpectedPitTimeCount = 1;
            driver.Position = 5;
            driver.ChangeStatus(DriverStatus.OnTrack);
            driver.SetColumnHasValue(GridColumn.DriverName, true);

            driver.Reset();

            Assert.Equal(1, driver.Id);

            assert_properties_have_default_values(driver);
        }

        private void assert_properties_have_default_values(LiveDriver driver) {

            Assert.Equal(0, driver.CarNumber);
            Assert.Equal(0, driver.CurrentSectorNumber);
            Assert.Equal(0, driver.ExpectedPitTimeCount);
            Assert.False(driver.IsExpectingPitTimes);
            Assert.False(driver.IsRaceLeader);
            Assert.Equal(0, driver.LapNumber);
            Assert.Null(driver.LastGapMessage);
            Assert.Null(driver.LastIntervalMessage);
            Assert.Null(driver.LastLapTime);
            foreach(var sector in driver.LastSectors) {
                Assert.Null(sector);
            }
            Assert.Null(driver.Name);            
            Assert.Equal(0, driver.PreviousSectorNumber);
            Assert.Equal(0, driver.ExpectedPitTimeCount);
            Assert.Equal(0, driver.Position);
            Assert.Equal(DriverStatus.InPits, driver.Status);
            foreach(GridColumn column in Enum.GetValues(typeof(GridColumn))) {
                Assert.False(driver.ColumnHasValue(column));
            }
        }
    }
}