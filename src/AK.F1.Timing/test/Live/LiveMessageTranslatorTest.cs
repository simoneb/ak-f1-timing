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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Extensions;

using AK.F1.Timing.Messages;
using AK.F1.Timing.Messages.Driver;
using AK.F1.Timing.Messages.Session;

namespace AK.F1.Timing.Live
{
    public class LiveMessageTranslatorTest : TestBase
    {
        [Fact]
        public void can_create() {

            var translator = new LiveMessageTranslator();

            Assert.False(translator.HasSessionStarted);
            Assert.Equal(0, translator.RaceLapNumber);
            Assert.Equal(SessionType.None, translator.SessionType);
        }

        [Fact]
        public void can_determine_if_the_session_has_started() {

            var translator = new LiveMessageTranslator();

            Assert.False(translator.HasSessionStarted);

            translator.SessionType = SessionType.Practice;
            Assert.True(translator.HasSessionStarted);

            translator.SessionType = SessionType.Qually;
            Assert.True(translator.HasSessionStarted);

            translator.SessionType = SessionType.Race;
            Assert.False(translator.HasSessionStarted);
            translator.RaceLapNumber = 1;
            Assert.True(translator.HasSessionStarted);
        }

        [Fact]
        public void translate_throws_if_message_is_null() {

            var translator = new LiveMessageTranslator();

            Assert.Throws<ArgumentNullException>(() => translator.Translate(null));
        }

        [Fact]
        public void car_number_value_updates_are_translated_into_into_set_car_number_and_set_status_messages() {

            Translate(
                new SetGridColumnValueMessage(1, GridColumn.CarNumber, GridColumnColour.White, "1"))
            .Expect(
                new SetDriverCarNumberMessage(1, 1),
                new SetDriverStatusMessage(1, DriverStatus.OnTrack)
            );
        }

        [Fact]
        public void colour_updates_to_a_column_with_no_value_does_not_result_in_a_translation() {

            foreach(GridColumn column in Enum.GetValues(typeof(GridColumn))) {
                foreach(GridColumnColour colour in Enum.GetValues(typeof(GridColumnColour))) {
                    Translate(new SetGridColumnColourMessage(1, column, colour)).ExpectNull();
                }
            }
        }

        [Fact]
        public void sector_1_colour_updates_to_the_sector_just_set_are_translated_into_replace_driver_sector_time_messages() {

            colour_updates_to_the_sector_just_set_are_translated_into_replace_driver_sector_time_messages(GridColumn.S1, 1);
        }

        [Fact]
        public void sector_2_colour_updates_to_the_sector_just_set_are_translated_into_replace_driver_sector_time_messages() {

            colour_updates_to_the_sector_just_set_are_translated_into_replace_driver_sector_time_messages(GridColumn.S2, 2);
        }

        [Fact]
        public void sector_3_colour_updates_to_the_sector_just_set_are_translated_into_replace_driver_sector_time_messages() {

            colour_updates_to_the_sector_just_set_are_translated_into_replace_driver_sector_time_messages(GridColumn.S3, 3);
        }

        private void colour_updates_to_the_sector_just_set_are_translated_into_replace_driver_sector_time_messages(GridColumn sectorColumn, int sectorNumber) {

            var message = Translate<ReplaceDriverSectorTimeMessage>(
                new SetGridColumnValueMessage(1, GridColumn.Laps, GridColumnColour.White, "1"),
                new SetGridColumnValueMessage(1, sectorColumn, GridColumnColour.White, "35.5"),
                new SetGridColumnColourMessage(1, sectorColumn, GridColumnColour.White));

            Assert.Equal(1, message.DriverId);
            Assert.Equal(sectorNumber, message.SectorNumber);
            Assert.Equal(new PostedTime(TimeSpan.FromSeconds(35.5D), PostedTimeType.Normal, 1), message.Replacement);

            message = Translate<ReplaceDriverSectorTimeMessage>(
                new SetGridColumnValueMessage(1, GridColumn.Laps, GridColumnColour.White, "1"),
                new SetGridColumnValueMessage(1, sectorColumn, GridColumnColour.White, "35.5"),
                new SetGridColumnColourMessage(1, sectorColumn, GridColumnColour.Green));

            Assert.Equal(1, message.DriverId);
            Assert.Equal(sectorNumber, message.SectorNumber);
            Assert.Equal(new PostedTime(TimeSpan.FromSeconds(35.5D), PostedTimeType.PersonalBest, 1), message.Replacement);

            message = Translate<ReplaceDriverSectorTimeMessage>(
                new SetGridColumnValueMessage(1, GridColumn.Laps, GridColumnColour.White, "1"),
                new SetGridColumnValueMessage(1, sectorColumn, GridColumnColour.White, "35.5"),
                new SetGridColumnColourMessage(1, sectorColumn, GridColumnColour.Magenta));

            Assert.Equal(1, message.DriverId);
            Assert.Equal(sectorNumber, message.SectorNumber);
            Assert.Equal(new PostedTime(TimeSpan.FromSeconds(35.5D), PostedTimeType.SessionBest, 1), message.Replacement);
        }

        [Fact]
        public void sector_1_value_updates_are_translated_into_set_driver_sector_time_messages() {

            sector_value_updates_are_translated_into_set_driver_sector_time_messages(GridColumn.S1, 1);
        }

        [Fact]
        public void sector_2_value_updates_are_translated_into_set_driver_sector_time_messages() {

            sector_value_updates_are_translated_into_set_driver_sector_time_messages(GridColumn.S2, 2);
        }

        [Fact]
        public void sector_3_value_updates_are_translated_into_set_driver_sector_time_messages() {

            sector_value_updates_are_translated_into_set_driver_sector_time_messages(GridColumn.S3, 3);
        }

        private void sector_value_updates_are_translated_into_set_driver_sector_time_messages(GridColumn sectorColumn, int sectorNumber) {

            var message = Translate<SetDriverSectorTimeMessage>(
                new SetGridColumnValueMessage(1, GridColumn.Laps, GridColumnColour.White, "5"),
                new SetGridColumnValueMessage(1, sectorColumn, GridColumnColour.White, "31.1"));

            Assert.Equal(1, message.DriverId);
            Assert.Equal(sectorNumber, message.SectorNumber);
            Assert.Equal(new PostedTime(TimeSpan.FromSeconds(31.1D), PostedTimeType.Normal, 5), message.SectorTime);

            message = Translate<SetDriverSectorTimeMessage>(
                new SetGridColumnValueMessage(1, GridColumn.Laps, GridColumnColour.White, "5"),
                new SetGridColumnValueMessage(1, sectorColumn, GridColumnColour.Yellow, "31.1"));

            Assert.Equal(1, message.DriverId);
            Assert.Equal(sectorNumber, message.SectorNumber);
            Assert.Equal(new PostedTime(TimeSpan.FromSeconds(31.1D), PostedTimeType.Normal, 5), message.SectorTime);

            message = Translate<SetDriverSectorTimeMessage>(
                new SetGridColumnValueMessage(1, GridColumn.Laps, GridColumnColour.White, "5"),
                new SetGridColumnValueMessage(1, sectorColumn, GridColumnColour.Green, "31.1"));

            Assert.Equal(1, message.DriverId);
            Assert.Equal(sectorNumber, message.SectorNumber);
            Assert.Equal(new PostedTime(TimeSpan.FromSeconds(31.1D), PostedTimeType.PersonalBest, 5), message.SectorTime);

            message = Translate<SetDriverSectorTimeMessage>(
                new SetGridColumnValueMessage(1, GridColumn.Laps, GridColumnColour.White, "5"),
                new SetGridColumnValueMessage(1, sectorColumn, GridColumnColour.Magenta, "31.1"));

            Assert.Equal(1, message.DriverId);
            Assert.Equal(sectorNumber, message.SectorNumber);
            Assert.Equal(new PostedTime(TimeSpan.FromSeconds(31.1D), PostedTimeType.SessionBest, 5), message.SectorTime);
        }

        private Translation Translate(params Message[] messages) {

            Message translated = null;
            var translator = new LiveMessageTranslator();

            Assert.NotEmpty(messages);
            foreach(var message in messages) {
                translated = translator.Translate(message);
            }

            return new Translation(translated, this.Assert);
        }

        private sealed class Translation
        {
            private readonly Message _result;
            private readonly Assertions _assert;

            public Translation(Message result, Assertions assert) {

                _result = result;
                _assert = assert;
            }

            public void Expect(Message message) {

                _assert.PropertiesAreEqual(message, _result);
            }

            public void Expect(params Message[] messages) {

                _assert.IsType<CompositeMessage>(_result);

                var composite = (CompositeMessage)_result;

                _assert.Equal(messages.Length, composite.Messages.Count);
                foreach(var message in composite.Messages) {
                    _assert.PropertiesAreEqual(message, _result);
                }
            }

            public void ExpectNull() {

                _assert.Null(_result);
            }
        }
    }
}
