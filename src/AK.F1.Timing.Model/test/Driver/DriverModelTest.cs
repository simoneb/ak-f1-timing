// Copyright 2010 Andy Kernahan
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
using System.Collections;
using System.Collections.Generic;
using AK.F1.Timing.Messages.Driver;
using Xunit;
using Xunit.Extensions;

namespace AK.F1.Timing.Model.Driver
{
    public class DriverModelTest : TestClass
    {
        private const int DriverId = 1;

        [Fact]
        public void can_create() {

            var model = CreateModel();

            Assert.Equal(1, model.Id);
            Assert.Empty(model);
        }

        [Fact]
        public void process_throws_if_message_is_null() {

            var model = CreateModel();

            Assert.Throws<ArgumentNullException>(() => model.Process(null));
        }

        [Fact]
        public void processing_a_set_driver_car_number_message_for_the_driver_updates_the_car_number_property() {

            var model = CreateModel(new SetDriverCarNumberMessage(DriverId, 1));

            Assert.Equal(1, model.CarNumber);
        }

        [Fact]
        public void processing_a_set_driver_car_number_message_for_another_driver_does_not_update_the_car_number_property() {

            var model = CreateModel(new SetDriverCarNumberMessage(DriverId + 1, 5));

            Assert.Equal(0, model.CarNumber);
        }

        [Fact]
        public void changes_to_the_car_number_property_raise_the_property_changed_event() {

            var model = CreateModel();
            var observer = new PropertyChangeObserver<DriverModel>(model);

            model.Process(new SetDriverCarNumberMessage(DriverId, 7));

            Assert.True(observer.HasChanged(x => x.CarNumber));
        }

        [Fact]
        public void processing_a_set_driver_gap_message_for_the_driver_updates_the_gap_property() {

            var gap = new LapGap(2);
            var model = CreateModel(new SetDriverGapMessage(DriverId, gap));

            Assert.Equal(gap, model.Gap);
        }

        [Fact]
        public void processing_a_set_driver_gap_message_for_another_driver_does_not_update_the_gap_property() {

            var model = CreateModel(new SetDriverGapMessage(DriverId + 1, new LapGap(2)));

            Assert.Null(model.Gap);
        }

        [Fact]
        public void changes_to_the_gap_property_raise_the_property_changed_event() {

            var model = CreateModel();
            var observer = new PropertyChangeObserver<DriverModel>(model);

            model.Process(new SetDriverGapMessage(DriverId, new LapGap(2)));

            Assert.True(observer.HasChanged(x => x.Gap));
        }

        [Fact]
        public void processing_a_set_driver_interval_message_for_the_driver_updates_the_interval_property() {

            var interval = new LapGap(2);
            var model = CreateModel(new SetDriverIntervalMessage(DriverId, interval));

            Assert.Equal(interval, model.Interval);
        }

        [Fact]
        public void processing_a_set_driver_interval_message_for_another_driver_does_not_update_the_interval_property() {

            var model = CreateModel(new SetDriverIntervalMessage(DriverId + 1, new LapGap(2)));

            Assert.Null(model.Interval);
        }

        [Fact]
        public void changes_to_the_interval_property_raise_the_property_changed_event() {

            var model = CreateModel();
            var observer = new PropertyChangeObserver<DriverModel>(model);

            model.Process(new SetDriverIntervalMessage(DriverId, new LapGap(2)));

            Assert.True(observer.HasChanged(x => x.Interval));
        }

        [Fact]
        public void processing_a_set_driver_lap_number_message_for_the_driver_updates_the_laps_completed_property() {

            var model = CreateModel(new SetDriverLapNumberMessage(DriverId, 1));

            Assert.Equal(1, model.LapsCompleted);
        }

        [Fact]
        public void processing_a_set_driver_lap_number_message_for_another_driver_does_not_update_the_laps_completed_property() {

            var model = CreateModel(new SetDriverLapNumberMessage(DriverId + 1, 5));

            Assert.Equal(0, model.LapsCompleted);
        }

        [Fact]
        public void changes_to_the_laps_completed_property_raise_the_property_changed_event() {

            var model = CreateModel();
            var observer = new PropertyChangeObserver<DriverModel>(model);

            model.Process(new SetDriverLapNumberMessage(DriverId, 7));

            Assert.True(observer.HasChanged(x => x.LapsCompleted));
        }

        [Fact]
        public void processing_a_set_driver_position_message_for_the_driver_updates_the_position_property() {

            var model = CreateModel(new SetDriverPositionMessage(DriverId, 3));

            Assert.Equal(3, model.Position);
        }

        [Fact]
        public void processing_a_set_driver_position_message_for_another_driver_does_not_update_the_position_property() {

            var model = CreateModel(new SetDriverPositionMessage(DriverId + 1, 5));

            Assert.Equal(0, model.Position);
        }

        [Fact]
        public void changes_to_the_position_property_raise_the_property_changed_event() {

            var model = CreateModel();
            var observer = new PropertyChangeObserver<DriverModel>(model);

            model.Process(new SetDriverPositionMessage(DriverId, 7));

            Assert.True(observer.HasChanged(x => x.Position));
        }

        [Fact]
        public void processing_a_set_driver_name_message_for_the_driver_updates_the_name_property() {

            var model = CreateModel(new SetDriverNameMessage(DriverId, "driverName"));

            Assert.Equal("driverName", model.Name);
        }

        [Fact]
        public void processing_a_set_driver_name_message_for_another_driver_does_not_update_the_name_property() {

            var model = CreateModel(new SetDriverNameMessage(DriverId + 1, "anotherDriverName"));

            Assert.Null(model.Name);
        }

        [Fact]
        public void changes_to_the_name_property_raise_the_property_changed_event() {

            var model = CreateModel();
            var observer = new PropertyChangeObserver<DriverModel>(model);

            model.Process(new SetDriverNameMessage(DriverId, "driverName"));

            Assert.True(observer.HasChanged(x => x.Name));
        }

        [Fact]
        public void processing_a_set_driver_pit_count_message_for_the_driver_updates_the_pit_times_model() {

            var model = CreateModel(new SetDriverPitCountMessage(DriverId, 3));

            Assert.Equal(3, model.PitTimes.Count);
        }

        [Fact]
        public void processing_a_set_driver_pit_count_message_for_another_driver_does_not_update_the_pit_times_model() {

            var model = CreateModel(new SetDriverPitCountMessage(DriverId + 1, 5));

            Assert.Equal(0, model.PitTimes.Count);
        }

        [Fact]
        public void processing_a_set_driver_pit_time_message_for_the_driver_adds_it_to_the_pit_times() {

            var lapNumber = 15;
            var time = TimeSpan.FromSeconds(45.5);
            var model = CreateModel(new SetDriverPitTimeMessage(DriverId, time, lapNumber));

            Assert.NotEmpty(model.PitTimes.Items);
            Assert.Equal(lapNumber, model.PitTimes.Items[0].LapNumber);
            Assert.Equal(time, model.PitTimes.Items[0].Time);
        }

        [Fact]
        public void processing_a_set_driver_pit_time_message_for_another_driver_does_not_add_it_to_the_pit_times() {

            var model = CreateModel(new SetDriverPitTimeMessage(DriverId + 1, TimeSpan.FromSeconds(43.7), 12));

            Assert.Empty(model.PitTimes.Items);
        }

        [Fact]
        public void processing_a_set_driver_status_message_for_the_driver_updates_the_status_property() {

            var model = CreateModel(new SetDriverStatusMessage(DriverId, DriverStatus.Retired));

            Assert.Equal(DriverStatus.Retired, model.Status);
        }

        [Fact]
        public void processing_a_set_driver_status_message_for_another_driver_does_not_update_the_status_property() {

            var model = CreateModel(new SetDriverStatusMessage(DriverId + 1, DriverStatus.Retired));

            Assert.Equal(DriverStatus.InPits, model.Status);
        }

        [Fact]
        public void changes_to_the_status_property_raise_the_property_changed_event() {

            var model = CreateModel();
            var observer = new PropertyChangeObserver<DriverModel>(model);

            model.Process(new SetDriverStatusMessage(DriverId, DriverStatus.OnTrack));

            Assert.True(observer.HasChanged(x => x.Status));            
        }

        [Fact]
        public void processing_a_set_driver_lap_time_message_adds_it_to_the_lap_times() {

            var time = new PostedTime(TimeSpan.FromSeconds(62.3), PostedTimeType.Normal, 15);
            var model = CreateModel(new SetDriverLapTimeMessage(DriverId, time));

            Assert.NotEmpty(model.LapTimes.Laps);
            Assert.Same(time, model.LapTimes.Laps.Items[0]);
        }

        [Fact]
        public void processing_a_set_driver_lap_time_message_for_another_driver_does_not_add_it_to_the_lap_times() {

            var time = new PostedTime(TimeSpan.FromSeconds(76.5), PostedTimeType.Normal, 15);
            var model = CreateModel(new SetDriverLapTimeMessage(DriverId + 1, time));

            Assert.Empty(model.LapTimes.Laps);
        }

        [Fact]
        public void processing_a_replace_driver_lap_time_message_replaces_the_previous_lap_time() {

            var replacementTime = new PostedTime(TimeSpan.FromSeconds(62.3), PostedTimeType.PersonalBest, 15);
            var model = CreateModel(
                new SetDriverLapTimeMessage(DriverId, new PostedTime(TimeSpan.FromSeconds(62.3), PostedTimeType.Normal, 15)),
                new ReplaceDriverLapTimeMessage(DriverId, replacementTime)
            );

            Assert.NotEmpty(model.LapTimes.Laps);
            Assert.Same(replacementTime, model.LapTimes.Laps.Items[0]);
        }

        [Fact]
        public void processing_a_replace_driver_lap_time_message_for_another_driver_does_not_replace_the_previous_lap_time() {

            var time = new PostedTime(TimeSpan.FromSeconds(62.3), PostedTimeType.Normal, 15);
            var model = CreateModel(
                new SetDriverLapTimeMessage(DriverId, time),
                new ReplaceDriverLapTimeMessage(DriverId + 1, new PostedTime(TimeSpan.FromSeconds(62.3), PostedTimeType.PersonalBest, 15))
            );

            Assert.NotEmpty(model.LapTimes.Laps);
            Assert.Same(time, model.LapTimes.Laps.Items[0]);
        }

        [Theory]
        [ClassData(typeof(AllSectorNumbers))]
        public void processing_a_set_driver_sector_time_message_adds_it_to_the_sector_times(int sectorNumber) {

            var time = new PostedTime(TimeSpan.FromSeconds(34.5), PostedTimeType.Normal, 15);
            var model = CreateModel(new SetDriverSectorTimeMessage(DriverId, sectorNumber, time));

            Assert.NotEmpty(model.LapTimes.GetSector(sectorNumber));
            Assert.Same(time, model.LapTimes.GetSector(sectorNumber).Items[0]);
        }

        [Theory]
        [ClassData(typeof(AllSectorNumbers))]
        public void processing_a_set_driver_sector_time_message_for_another_driver_does_not_add_it_to_the_sector_times(int sectorNumber) {

            var time = new PostedTime(TimeSpan.FromSeconds(24.6), PostedTimeType.Normal, 15);
            var model = CreateModel(new SetDriverSectorTimeMessage(DriverId + 1, sectorNumber, time));

            Assert.Empty(model.LapTimes.GetSector(sectorNumber));
        }

        [Theory]
        [ClassData(typeof(AllSectorNumbers))]
        public void processing_a_replace_driver_sector_time_message_replaces_the_previous_sector_time(int sectorNumber) {

            var replacementTime = new PostedTime(TimeSpan.FromSeconds(34.5), PostedTimeType.PersonalBest, 15);
            var model = CreateModel(
                new SetDriverSectorTimeMessage(DriverId, sectorNumber, new PostedTime(TimeSpan.FromSeconds(34.5), PostedTimeType.Normal, 15)),
                new ReplaceDriverSectorTimeMessage(DriverId, sectorNumber, replacementTime)
            );

            Assert.NotEmpty(model.LapTimes.GetSector(sectorNumber));
            Assert.Same(replacementTime, model.LapTimes.GetSector(sectorNumber).Items[0]);
        }

        [Theory]
        [ClassData(typeof(AllSectorNumbers))]
        public void processing_a_replace_driver_sector_time_message_for_another_driver_does_not_replace_the_previous_sector_time(int sectorNumber) {

            var time = new PostedTime(TimeSpan.FromSeconds(34.5), PostedTimeType.Normal, 15);
            var model = CreateModel(
                new SetDriverSectorTimeMessage(DriverId, sectorNumber, time),
                new ReplaceDriverSectorTimeMessage(DriverId + 1, sectorNumber, new PostedTime(TimeSpan.FromSeconds(34.5), PostedTimeType.SessionBest, 15))
            );

            Assert.NotEmpty(model.LapTimes.GetSector(sectorNumber));
            Assert.Same(time, model.LapTimes.GetSector(sectorNumber).Items[0]);
        }

        [Theory]
        [ClassData(typeof(AllQuallyNumbers))]
        public void processing_a_set_driver_qually_time_message_adds_it_to_the_qually_times(int quallyNumber) {

            var quallyTime = TimeSpan.FromSeconds(34.5);
            var model = CreateModel(new SetDriverQuallyTimeMessage(DriverId, quallyNumber, quallyTime));

            Assert.Equal(quallyTime, model.QuallyTimes.GetTime(quallyNumber));            
        }

        [Theory]
        [ClassData(typeof(AllQuallyNumbers))]
        public void processing_a_set_driver_qually_time_message_for_another_driver_does_not_add_it_to_the_qually_times(int quallyNumber) {

            var model = CreateModel(new SetDriverQuallyTimeMessage(DriverId + 1, quallyNumber, TimeSpan.FromSeconds(34.5)));

            Assert.Null(model.QuallyTimes.GetTime(quallyNumber));
        }

        private static DriverModel CreateModel(params Message[] messagesToProcess) {

            var model = new DriverModel(DriverId);

            foreach(var message in messagesToProcess) {
                model.Process(message);
            }

            return model;
        }

        public sealed class AllSectorNumbers : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator() {

                yield return A(1);
                yield return A(2);
                yield return A(3);
            }

            IEnumerator IEnumerable.GetEnumerator() {

                return GetEnumerator();
            }

            private static object[] A(params object[] args) {

                return args;
            }
        }

        public sealed class AllQuallyNumbers : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator() {

                yield return A(1);
                yield return A(2);
                yield return A(3);
            }

            IEnumerator IEnumerable.GetEnumerator() {

                return GetEnumerator();
            }

            private static object[] A(params object[] args) {

                return args;
            }
        }
    }
}
