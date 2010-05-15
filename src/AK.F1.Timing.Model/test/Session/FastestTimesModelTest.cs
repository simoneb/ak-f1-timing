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
using System.Globalization;
using System.Linq;
using AK.F1.Timing.Messages.Driver;
using AK.F1.Timing.Messages.Session;
using AK.F1.Timing.Model.Driver;
using Xunit;
using Xunit.Extensions;

namespace AK.F1.Timing.Model.Session
{
    public class FastestTimesModelTest
    {
        [Fact]
        public void can_create()
        {
            var model = CreateModel();

            assert_properties_have_default_values(model);
        }

        [Fact]
        public void ctor_throws_if_driver_model_provider_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => new FastestTimesModel(null));
        }

        [Fact]
        public void can_reset()
        {
            var model = CreateModel();
            var postedTime = new PostedTime(TimeSpan.FromSeconds(1D), PostedTimeType.SessionBest, 1);

            for(int sectorNumber = 1; sectorNumber <= 3; ++sectorNumber)
            {
                model.Process(new SetDriverSectorTimeMessage(1, sectorNumber, postedTime));
            }
            model.Process(new SetDriverLapTimeMessage(1, postedTime));

            model.Reset();

            assert_properties_have_default_values(model);
        }

        [Fact]
        public void process_throws_when_message_is_null()
        {
            var model = CreateModel();

            Assert.Throws<ArgumentNullException>(() => model.Process(null));
        }

        [Theory]
        [ClassData(typeof(AllSessionTypes_AllSectors))]
        public void processing_a_session_best_set_driver_sector_time_message_updates_the_sector_property(
            SessionType sessionType, int sectorNumber)
        {
            var driverId = 1;
            var sectorTime = new PostedTime(TimeSpan.FromSeconds(35.4), PostedTimeType.SessionBest, 14);
            var model = CreateModel(
                new SetSessionTypeMessage(sessionType, "sessionId"),
                new SetDriverSectorTimeMessage(driverId, sectorNumber, sectorTime));
            var fastestTime = GetFastestTime(model, sectorNumber);

            Assert.NotNull(fastestTime);
            Assert.Null(fastestTime.Delta);
            Assert.NotNull(fastestTime.Driver);
            Assert.Equal(driverId, fastestTime.Driver.Id);
            Assert.Equal(sectorTime.Time, fastestTime.Time);
            Assert.Equal(sectorTime.LapNumber, fastestTime.LapNumber);
        }

        [Theory]
        [ClassData(typeof(AllSessionTypes_AllSectors))]
        public void processing_another_session_best_set_driver_sector_time_message_updates_the_sector_property_and_computes_the_delta(
            SessionType sessionType, int sectorNumber)
        {
            var driverId = 2;
            var sectorTime = new PostedTime(TimeSpan.FromSeconds(25.4), PostedTimeType.SessionBest, 24);
            var model = CreateModel(
                new SetSessionTypeMessage(sessionType, "sessionId"),
                new SetDriverSectorTimeMessage(1, sectorNumber, new PostedTime(TimeSpan.FromSeconds(35.4), PostedTimeType.SessionBest, 14)),
                new SetDriverSectorTimeMessage(driverId, sectorNumber, sectorTime));
            var fastestTime = GetFastestTime(model, sectorNumber);

            Assert.NotNull(fastestTime);
            Assert.NotNull(fastestTime.Delta);
            Assert.Equal(TimeSpan.FromSeconds(-10D), fastestTime.Delta);
            Assert.NotNull(fastestTime.Driver);
            Assert.Equal(driverId, fastestTime.Driver.Id);
            Assert.Equal(sectorTime.Time, fastestTime.Time);
            Assert.Equal(sectorTime.LapNumber, fastestTime.LapNumber);
        }

        [Theory]
        [ClassData(typeof(AllSessionTypes))]
        public void processing_a_session_best_set_driver_lap_time_message_updates_the_lap_property(SessionType sessionType)
        {
            var driverId = 1;
            var lapTime = new PostedTime(TimeSpan.FromSeconds(125.567), PostedTimeType.SessionBest, 14);
            var model = CreateModel(
                new SetSessionTypeMessage(sessionType, "sessionId"),
                new SetDriverLapTimeMessage(driverId, lapTime));
            var fastestTime = model.Lap;

            Assert.NotNull(fastestTime);
            Assert.Null(fastestTime.Delta);
            Assert.NotNull(fastestTime.Driver);
            Assert.Equal(driverId, fastestTime.Driver.Id);
            Assert.Equal(lapTime.Time, fastestTime.Time);
            Assert.Equal(lapTime.LapNumber, fastestTime.LapNumber);
        }

        [Theory]
        [InlineData(PostedTimeType.Normal, 1)]
        [InlineData(PostedTimeType.Normal, 2)]
        [InlineData(PostedTimeType.Normal, 3)]
        [InlineData(PostedTimeType.PersonalBest, 1)]
        [InlineData(PostedTimeType.PersonalBest, 2)]
        [InlineData(PostedTimeType.PersonalBest, 3)]
        public void in_a_race_session_processing_a_non_session_best_set_driver_sector_time_message_does_not_update_the_sector_property(
            PostedTimeType postedTimeType, int sectorNumber)
        {
            var model = CreateModel(
                new SetSessionTypeMessage(SessionType.Race, "sessionId"),
                new SetDriverSectorTimeMessage(1, sectorNumber, new PostedTime(TimeSpan.FromSeconds(35.4), postedTimeType, 14)));

            Assert.Null(GetFastestTime(model, sectorNumber));
        }

        [Theory]
        [ClassData(typeof(AllNonRaceSessionTypes_AllNonSessionBestPostedTimeTypes))]
        public void in_a_none_race_session_processing_a_non_session_best_set_driver_lap_time_that_is_less_than_the_current_updates_the_lap_property(
            SessionType sessionType, PostedTimeType postedTimeType)
        {
            var sectorTime = new PostedTime(TimeSpan.FromSeconds(145.743), postedTimeType, 4);
            var model = CreateModel(
                new SetSessionTypeMessage(sessionType, "sessionId"),
                new SetDriverLapTimeMessage(1, sectorTime));

            Assert.NotNull(model.Lap);
            // An equal time should not alter the property: first to set the time wins.
            model.Process(new SetDriverLapTimeMessage(2, sectorTime));
            Assert.Equal(1, model.Lap.Driver.Id);
            // A longer time should not alter the property.
            model.Process(new SetDriverLapTimeMessage(2, new PostedTime(sectorTime.Time + TimeSpan.FromSeconds(1D), postedTimeType, 4)));
            Assert.Equal(1, model.Lap.Driver.Id);
            // A shorter time should set the property.
            model.Process(new SetDriverLapTimeMessage(2, new PostedTime(sectorTime.Time - TimeSpan.FromSeconds(1D), postedTimeType, 4)));
            Assert.Equal(2, model.Lap.Driver.Id);
        }

        [Fact]
        public void when_all_sector_times_have_been_set_the_possible_property_is_computed_and_the_property_changed_event_is_raised()
        {
            var model = CreateModel();
            var sectorTime = new PostedTime(TimeSpan.FromSeconds(2D), PostedTimeType.SessionBest, 1);
            var observer = new PropertyChangeObserver<FastestTimesModel>(model);

            model.Process(new SetDriverSectorTimeMessage(1, 1, sectorTime));
            Assert.Null(model.Possible);
            model.Process(new SetDriverSectorTimeMessage(1, 2, sectorTime));
            Assert.Null(model.Possible);
            model.Process(new SetDriverSectorTimeMessage(1, 3, sectorTime));
            Assert.NotNull(model.Possible);
            Assert.Null(model.Possible.Delta);
            Assert.Null(model.Possible.Driver);
            // TODO perhaps the lap number should define the latest change made? 
            Assert.Equal(0, model.Possible.LapNumber);
            Assert.Equal(TimeSpan.FromSeconds(6D), model.Possible.Time);
            Assert.True(observer.HasChanged(x => x.Possible));
            // Post a lap time assert the delta is computed.
            model.Process(new SetDriverLapTimeMessage(1, new PostedTime(TimeSpan.FromSeconds(8D), PostedTimeType.SessionBest, 1)));
            Assert.NotNull(model.Possible.Delta);
            Assert.Equal(TimeSpan.FromSeconds(2D), model.Possible.Delta.Value);
            Assert.Equal(2, observer.GetChangeCount(x => x.Possible));
        }

        [Fact]
        public void when_at_least_one_property_has_a_value_the_is_empty_property_is_updated_and_the_property_changed_event_is_raised()
        {
            var model = CreateModel();
            var postedTime = new PostedTime(TimeSpan.FromSeconds(1D), PostedTimeType.SessionBest, 1);
            var observer = new PropertyChangeObserver<FastestTimesModel>(model);

            for(int sectorNumber = 1; sectorNumber <= 3; ++sectorNumber)
            {
                model.Process(new SetDriverSectorTimeMessage(1, sectorNumber, postedTime));
                Assert.False(model.IsEmpty);
                Assert.True(observer.HasChanged(x => x.IsEmpty));
                model.Reset();
                observer.ClearChanges();
            }

            model.Reset();
            observer.ClearChanges();
            model.Process(new SetDriverLapTimeMessage(1, postedTime));
            Assert.False(model.IsEmpty);
            Assert.True(observer.HasChanged(x => x.IsEmpty));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void changes_to_a_sector_property_raises_the_property_changed_event(int sectorNumber)
        {
            var model = CreateModel();
            var observer = new PropertyChangeObserver<FastestTimesModel>(model);

            model.Process(new SetDriverSectorTimeMessage(1, sectorNumber, new PostedTime(TimeSpan.FromSeconds(32.4), PostedTimeType.SessionBest, 1)));

            Assert.True(observer.HasChanged("S" + sectorNumber.ToString(CultureInfo.InvariantCulture)));
        }

        [Fact]
        public void changes_to_the_lap_property_raises_the_property_changed_event()
        {
            var model = CreateModel();
            var observer = new PropertyChangeObserver<FastestTimesModel>(model);

            model.Process(new SetDriverLapTimeMessage(1, new PostedTime(TimeSpan.FromSeconds(123.624), PostedTimeType.SessionBest, 1)));

            Assert.True(observer.HasChanged(x => x.Lap));
        }

        private void assert_properties_have_default_values(FastestTimesModel model)
        {
            Assert.True(model.IsEmpty);
            Assert.Null(model.Lap);
            Assert.Null(model.Possible);
            Assert.Null(model.S1);
            Assert.Null(model.S2);
            Assert.Null(model.S3);
        }

        private FastestTimeModel GetFastestTime(FastestTimesModel model, int sectorNumber)
        {
            switch(sectorNumber)
            {
                case 1:
                    return model.S1;
                case 2:
                    return model.S2;
                case 3:
                    return model.S3;
                default:
                    throw new ArgumentOutOfRangeException("sectorNumber");
            }
        }

        private FastestTimesModel CreateModel(params Message[] messagesToProcess)
        {
            var model = new FastestTimesModel(new StubDriverModelLocator());

            foreach(var message in messagesToProcess)
            {
                model.Process(message);
            }

            return model;
        }

        private sealed class StubDriverModelLocator : IDriverModelLocator
        {
            public DriverModel GetDriver(int id)
            {
                return new DriverModel(id);
            }
        }

        public sealed class AllNonRaceSessionTypes_AllNonSessionBestPostedTimeTypes : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                return (from sessionType in Enum.GetValues(typeof(SessionType))
                            .Cast<object>()
                            .Except(A(SessionType.Race))
                        from postedTimeType in Enum.GetValues(typeof(PostedTimeType))
                            .Cast<object>()
                            .Except(A(PostedTimeType.SessionBest))
                        select A(sessionType, postedTimeType)).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            private static object[] A(params object[] args)
            {
                return args;
            }
        }

        public sealed class AllSessionTypes_AllSectors : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                return (from sessionType in Enum.GetValues(typeof(SessionType))
                            .Cast<object>()
                        from sectorNumber in Enumerable.Range(1, 3)
                        select A(sessionType, sectorNumber)).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            private static object[] A(params object[] args)
            {
                return args;
            }
        }

        public sealed class AllSessionTypes : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                return Enum.GetValues(typeof(SessionType)).Cast<object>().Select(x => A(x)).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            private static object[] A(params object[] args)
            {
                return args;
            }
        }
    }
}