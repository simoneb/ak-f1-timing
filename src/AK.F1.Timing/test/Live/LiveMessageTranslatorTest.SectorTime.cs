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
using Xunit;
using Xunit.Extensions;

namespace AK.F1.Timing.Live
{
    public partial class LiveMessageTranslatorTest
    {
        [Theory]
        [ClassData(typeof(AllSectorGridColumns_AllSessionTypes))]
        public void when_a_driver_is_not_on_the_track_then_sector_time_column_values_are_not_translated_into_set_sector_time_messages(
            GridColumn sector, SessionType session)
        {
            In(session).Assert(translator =>
            {
                var driver = translator.GetDriver(1);
                var message = new SetGridColumnValueMessage(1, sector, GridColumnColour.White, "1:35.571");
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
        public void when_a_driver_is_on_the_track_then_sector_1_column_values_are_translated_into_set_sector_time_messages(SessionType session)
        {
            In(session).Assert(translator =>
            {
                SetDriverSectorTimeMessage expected;
                LiveDriver driver = translator.GetDriver(1);

                driver.ChangeStatus(DriverStatus.OnTrack);
                driver.LapNumber = 5;
                // TODO Perhaps this test should be moved into one of its own as we also test here that not setting the
                // CurrentSectorNumber still results in a translation for the given sector
                // Normal lap time.
                expected = new SetDriverSectorTimeMessage(1, 1, PT(23.5, PostedTimeType.Normal, 5));
                Assert.MessagesAreEqual(
                    expected,
                    translator.Translate(new SetGridColumnValueMessage(1, GridColumn.S1, GridColumnColour.White, "23.5"))
                );
                Assert.Equal(expected.SectorTime, driver.LastSectors[0]);
                Assert.Equal(2, driver.CurrentSectorNumber);
                // Personal best lap time.
                driver.CurrentSectorNumber = 1;
                expected = new SetDriverSectorTimeMessage(1, 1, PT(23.5, PostedTimeType.PersonalBest, 5));
                Assert.MessagesAreEqual(
                    expected,
                    translator.Translate(new SetGridColumnValueMessage(1, GridColumn.S1, GridColumnColour.Green, "23.5"))
                );
                Assert.Equal(expected.SectorTime, driver.LastSectors[0]);
                Assert.Equal(2, driver.CurrentSectorNumber);
                // Session best lap time.
                driver.CurrentSectorNumber = 1;
                expected = new SetDriverSectorTimeMessage(1, 1, PT(23.5, PostedTimeType.SessionBest, 5));
                Assert.MessagesAreEqual(
                    expected,
                    translator.Translate(new SetGridColumnValueMessage(1, GridColumn.S1, GridColumnColour.Magenta, "23.5"))
                );
                Assert.Equal(expected.SectorTime, driver.LastSectors[0]);
                Assert.Equal(2, driver.CurrentSectorNumber);
            });
        }

        [Theory]
        [ClassData(typeof(AllSessionTypes))]
        public void when_a_driver_is_on_the_track_then_sector_2_column_values_are_translated_into_set_sector_time_messages(SessionType session)
        {
            In(session).Assert(translator =>
            {
                var driver = translator.GetDriver(1);
                var expected = new SetDriverSectorTimeMessage(1, 2, PT(23.5, PostedTimeType.Normal, 5));

                driver.ChangeStatus(DriverStatus.OnTrack);
                driver.LapNumber = 5;
                Assert.MessagesAreEqual(
                    expected,
                    translator.Translate(new SetGridColumnValueMessage(1, GridColumn.S2, GridColumnColour.White, "23.5"))
                );
                Assert.Equal(expected.SectorTime, driver.LastSectors[1]);
                Assert.Equal(3, driver.CurrentSectorNumber);
                // We don't assert the other types here as it is sufficiently covered by the sector 1 test.
            });
        }

        [Fact(Skip = "TODO")]
        public void sector_3_column_values_in_a_race_session_are_translated_into_set_sector_time_and_set_driver_completed_laps_messages() {}

        [Theory(Skip = "TODO")]
        [ClassData(typeof(AllSectorGridColumns_AllSessionTypes))]
        public void when_a_driver_is_not_on_the_track_then_sector_time_column_colours_are_not_translated_into_set_sector_time_messages(
            GridColumn sector, SessionType session) {}

        [Theory]
        [ClassData(typeof(AllSessionTypes))]
        public void when_a_driver_is_on_the_track_in_sector_1_then_sector_1_column_colours_are_translated_into_a_set_sector_time_message(
            SessionType session)
        {
            In(session).Assert(translator =>
            {
                var driver = translator.GetDriver(1);
                var expected = new SetDriverSectorTimeMessage(1, 1, PT(23.5, PostedTimeType.Normal, 5));

                driver.ChangeStatus(DriverStatus.OnTrack);
                driver.LapNumber = 5;
                driver.CurrentSectorNumber = 1;
                driver.LastSectors[0] = PT(23.5, PostedTimeType.Normal, 4);
                driver.SetColumnHasValue(GridColumn.S1, true);
                Assert.MessagesAreEqual(
                    expected,
                    translator.Translate(new SetGridColumnColourMessage(1, GridColumn.S1, GridColumnColour.White))
                );
                Assert.Equal(expected.SectorTime, driver.LastSectors[0]);
                Assert.Equal(2, driver.CurrentSectorNumber);
            });
        }

        [Theory]
        [ClassData(typeof(AllSessionTypes))]
        public void when_a_driver_is_on_the_track_in_sector_1_then_sector_2_column_colours_are_not_translated_into_a_set_sector_time_message(
            SessionType session)
        {
            In(session).Assert(translator =>
            {
                var driver = translator.GetDriver(1);

                driver.ChangeStatus(DriverStatus.OnTrack);
                driver.LapNumber = 5;
                driver.CurrentSectorNumber = 1;
                driver.SetColumnHasValue(GridColumn.S2, true);
                Assert.Null(translator.Translate(new SetGridColumnColourMessage(1, GridColumn.S2, GridColumnColour.White)));
            });
        }

        [Theory]
        [ClassData(typeof(AllSessionTypes))]
        public void when_a_driver_is_on_the_track_in_sector_1_then_sector_3_column_colours_are_translated_into_a_replace_sector_3_time_message(
            SessionType session)
        {
            In(session).Assert(translator =>
            {
                var driver = translator.GetDriver(1);
                var expected = new ReplaceDriverSectorTimeMessage(1, 3, PT(23.5, PostedTimeType.PersonalBest, 4));

                driver.ChangeStatus(DriverStatus.OnTrack);
                driver.LapNumber = 5;
                driver.CurrentSectorNumber = 1;
                driver.LastSectors[2] = PT(23.5, PostedTimeType.Normal, 4);
                driver.SetColumnHasValue(GridColumn.S3, true);
                Assert.MessagesAreEqual(
                    expected,
                    translator.Translate(new SetGridColumnColourMessage(1, GridColumn.S3, GridColumnColour.Green))
                );
                Assert.Equal(expected.Replacement, driver.LastSectors[2]);
            });
        }

        [Theory]
        [ClassData(typeof(AllSessionTypes))]
        public void when_a_driver_is_on_the_track_in_sector_2_then_sector_1_column_colours_are_translated_into_a_replace_sector_1_time_message(
            SessionType session)
        {
            In(session).Assert(translator =>
            {
                var driver = translator.GetDriver(1);
                var expected = new ReplaceDriverSectorTimeMessage(1, 1, PT(23.5, PostedTimeType.PersonalBest, 4));

                driver.ChangeStatus(DriverStatus.OnTrack);
                driver.LapNumber = 5;
                driver.CurrentSectorNumber = 2;
                driver.LastSectors[0] = PT(23.5, PostedTimeType.Normal, 4);
                driver.SetColumnHasValue(GridColumn.S1, true);
                Assert.MessagesAreEqual(
                    expected,
                    translator.Translate(new SetGridColumnColourMessage(1, GridColumn.S1, GridColumnColour.Green))
                );
                Assert.Equal(expected.Replacement, driver.LastSectors[0]);
            });
        }

        [Theory]
        [ClassData(typeof(AllSessionTypes))]
        public void when_a_driver_is_on_the_track_in_sector_1_then_sector_2_column_clears_are_translated_into_set_sector_time_messages(
            SessionType session)
        {
            In(session).Assert(translator =>
            {
                var driver = translator.GetDriver(1);
                var expected = new SetDriverSectorTimeMessage(1, 1, PT(23.5, PostedTimeType.Normal, 5));

                driver.ChangeStatus(DriverStatus.OnTrack);
                driver.LapNumber = 5;
                driver.CurrentSectorNumber = 1;
                driver.LastSectors[0] = PT(23.5, PostedTimeType.Normal, 4);
                driver.SetColumnHasValue(GridColumn.S1, true);
                Assert.MessagesAreEqual(
                    expected,
                    translator.Translate(new SetGridColumnValueMessage(1, GridColumn.S2, GridColumnColour.White, null))
                );
                Assert.Equal(expected.SectorTime, driver.LastSectors[0]);
                Assert.Equal(2, driver.CurrentSectorNumber);
            });
        }

        [Theory]
        [ClassData(typeof(AllSessionTypes))]
        public void when_a_driver_is_on_the_track_in_sector_1_and_the_sector_1_column_has_no_value_then_sector_2_column_clears_are_not_translated_into_set_sector_time_messages(
            SessionType session)
        {
            In(session).Assert(translator =>
            {
                var driver = translator.GetDriver(1);

                driver.ChangeStatus(DriverStatus.OnTrack);
                driver.LapNumber = 5;
                driver.CurrentSectorNumber = 1;
                driver.LastSectors[0] = PT(23.5, PostedTimeType.Normal, 4);
                driver.SetColumnHasValue(GridColumn.S1, false);
                Assert.Null(translator.Translate(new SetGridColumnValueMessage(1, GridColumn.S2, GridColumnColour.White, null)));
            });
        }
    }
}