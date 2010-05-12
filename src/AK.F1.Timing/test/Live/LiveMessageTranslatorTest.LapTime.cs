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

using AK.F1.Timing.Messages.Driver;
using AK.F1.Timing.Messages.Session;
using Xunit.Extensions;

namespace AK.F1.Timing.Live
{
    public partial class LiveMessageTranslatorTest
    {
        [Theory]
        [ClassData(typeof(AllSessionTypes))]
        public void when_a_driver_is_on_the_track_lap_time_column_values_are_translated_into_set_lap_time_messages(SessionType session)
        {
            In(session).OnLap(5).Assert(translator =>
            {
                SetDriverLapTimeMessage expected;
                LiveDriver driver = translator.GetDriver(1);

                driver.LapNumber = 5;
                driver.ChangeStatus(DriverStatus.OnTrack);
                // Normal lap time.
                expected = new SetDriverLapTimeMessage(1, PT(95.571, PostedTimeType.Normal, 5));
                Assert.MessagesAreEqual(expected,
                    translator.Translate(new SetGridColumnValueMessage(1, GridColumn.LapTime, GridColumnColour.White, "1:35.571"))
                    );
                Assert.Equal(expected.LapTime, driver.LastLapTime);
                // Personal best lap time.
                expected = new SetDriverLapTimeMessage(1, PT(95.571, PostedTimeType.PersonalBest, 5));
                Assert.MessagesAreEqual(expected,
                    translator.Translate(new SetGridColumnValueMessage(1, GridColumn.LapTime, GridColumnColour.Green, "1:35.571"))
                    );
                Assert.Equal(expected.LapTime, driver.LastLapTime);
                // Session best lap time.
                expected = new SetDriverLapTimeMessage(1, PT(95.571, PostedTimeType.SessionBest, 5));
                Assert.MessagesAreEqual(expected,
                    translator.Translate(new SetGridColumnValueMessage(1, GridColumn.LapTime, GridColumnColour.Magenta, "1:35.571"))
                    );
                Assert.Equal(expected.LapTime, driver.LastLapTime);
            });
        }

        [Theory]
        [ClassData(typeof(AllSessionTypes))]
        public void when_a_driver_is_not_on_the_track_lap_time_column_values_are_not_translated_into_set_lap_time_messages(SessionType session)
        {
            In(session).OnLap(5).Assert(translator =>
            {
                var driver = translator.GetDriver(1);
                var message = new SetGridColumnValueMessage(1, GridColumn.LapTime, GridColumnColour.White, "1:35.571");
                // In pits.
                driver.ChangeStatus(DriverStatus.InPits);
                Assert.Null(translator.Translate(message));
                // Out.
                driver.ChangeStatus(DriverStatus.Out);
                Assert.Null(translator.Translate(message));
                // Retired.
                driver.ChangeStatus(DriverStatus.Retired);
                Assert.Null(translator.Translate(message));
                // Stopped.
                driver.ChangeStatus(DriverStatus.Stopped);
                Assert.Null(translator.Translate(message));
            });
        }

        [Theory]
        [ClassData(typeof(AllSessionTypes))]
        public void when_a_driver_is_on_the_track_white_lap_time_column_colours_are_translated_into_set_lap_time_messages(SessionType session)
        {
            In(session).OnLap(5).Assert(translator =>
            {
                var driver = translator.GetDriver(1);
                var expected = new SetDriverLapTimeMessage(1, PT(95.571, PostedTimeType.Normal, 5));

                driver.LapNumber = 5;
                driver.ChangeStatus(DriverStatus.OnTrack);
                driver.SetColumnHasValue(GridColumn.LapTime, true);
                driver.LastLapTime = PT(95.571, PostedTimeType.Normal, 5);
                Assert.MessagesAreEqual(expected,
                    translator.Translate(new SetGridColumnColourMessage(1, GridColumn.LapTime, GridColumnColour.White))
                    );
                Assert.Equal(expected.LapTime, driver.LastLapTime);
            });
        }

        [Theory]
        [ClassData(typeof(AllSessionTypes))]
        public void when_a_driver_is_on_the_track_green_and_megenta_lap_time_column_colours_are_translated_into_replace_lap_time_messages(SessionType session)
        {
            In(session).OnLap(5).Assert(translator =>
            {
                ReplaceDriverLapTimeMessage expected;
                LiveDriver driver = translator.GetDriver(1);

                driver.LapNumber = 5;
                driver.ChangeStatus(DriverStatus.OnTrack);
                driver.SetColumnHasValue(GridColumn.LapTime, true);
                driver.LastLapTime = PT(95.571, PostedTimeType.Normal, 5);
                // Personal best lap time.
                expected = new ReplaceDriverLapTimeMessage(1, PT(95.571, PostedTimeType.PersonalBest, 5));
                Assert.MessagesAreEqual(expected,
                    translator.Translate(new SetGridColumnColourMessage(1, GridColumn.LapTime, GridColumnColour.Green))
                    );
                Assert.Equal(expected.Replacement, driver.LastLapTime);
                // Session best lap time.
                expected = new ReplaceDriverLapTimeMessage(1, PT(95.571, PostedTimeType.SessionBest, 5));
                Assert.MessagesAreEqual(expected,
                    translator.Translate(new SetGridColumnColourMessage(1, GridColumn.LapTime, GridColumnColour.Magenta))
                    );
                Assert.Equal(expected.Replacement, driver.LastLapTime);
            });
        }

        [Theory]
        [ClassData(typeof(AllSessionTypes))]
        public void when_a_driver_is_not_on_the_track_lap_time_column_colours_are_not_translated_into_set_lap_time_messages(SessionType session)
        {
            In(session).OnLap(5).Assert(translator =>
            {
                var driver = translator.GetDriver(1);
                var message = new SetGridColumnColourMessage(1, GridColumn.LapTime, GridColumnColour.White);

                driver.SetColumnHasValue(GridColumn.LapTime, true);
                driver.LastLapTime = PT(120.765, PostedTimeType.Normal, 1);
                // In pits.
                driver.ChangeStatus(DriverStatus.InPits);
                Assert.Null(translator.Translate(message));
                // Out.
                driver.ChangeStatus(DriverStatus.Out);
                Assert.Null(translator.Translate(message));
                // Retired.
                driver.ChangeStatus(DriverStatus.Retired);
                Assert.Null(translator.Translate(message));
                // Stopped.
                driver.ChangeStatus(DriverStatus.Stopped);
                Assert.Null(translator.Translate(message));
            });
        }
    }
}