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
using System.Diagnostics;
using System.Linq;
using Xunit;
using Xunit.Extensions;

using AK.F1.Timing.Messages;
using AK.F1.Timing.Messages.Driver;
using AK.F1.Timing.Messages.Session;

namespace AK.F1.Timing.Live
{
    public abstract class LiveMessageTranslatorTest : TestBase
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
        public void colours_updates_to_a_columns_with_no_value_are_not_translated() {

            foreach(GridColumn column in Enum.GetValues(typeof(GridColumn))) {
                foreach(GridColumnColour colour in Enum.GetValues(typeof(GridColumnColour))) {
                    Translate(
                        new SetGridColumnColourMessage(1, column, colour)
                    ).ExpectNull()
                    .InAllSessions()
                    .Assert();
                }
            }
        }

        [Fact]
        public void unknown_column_values_are_not_translated() {

            foreach(GridColumnColour colour in Enum.GetValues(typeof(GridColumnColour))) {
                Translate(
                    new SetGridColumnValueMessage(1, GridColumn.Unknown, colour, "Unknown")
                ).ExpectNull()
                .InAllSessions()
                .Assert();
                // Assert that clear columns are not translated.
                Translate(
                    new SetGridColumnValueMessage(1, GridColumn.Unknown, colour, null)
                ).ExpectNull()
                .InAllSessions()
                .Assert();
            }
        }

        [Fact]
        public void unknown_column_colours_are_not_translated() {

            foreach(GridColumnColour colour in Enum.GetValues(typeof(GridColumnColour))) {
                Translate(
                    new SetGridColumnColourMessage(1, GridColumn.Unknown, colour)
                ).ExpectNull()
                .InAllSessions()
                .Assert();
            }
        }

        [Fact]
        public void lap_time_column_values_are_translated() {

            Translate(
                new SetSessionTypeMessage(SessionType.Practice, "SessionId"),
                new SetGridColumnValueMessage(1, GridColumn.Laps, GridColumnColour.White, "5"),
                new SetGridColumnValueMessage(1, GridColumn.LapTime, GridColumnColour.White, "1:35.571")
            ).Expect(
                new SetDriverLapTimeMessage(1, new PostedTime(TimeSpan.FromMilliseconds(95571D), PostedTimeType.Normal, 5))
            ).InAllSessions()
            .Assert();

            Translate(
                new SetSessionTypeMessage(SessionType.Practice, "SessionId"),
                new SetGridColumnValueMessage(1, GridColumn.Laps, GridColumnColour.White, "10"),
                new SetGridColumnValueMessage(1, GridColumn.LapTime, GridColumnColour.Green, "1:35.571")
            ).Expect(
                new SetDriverLapTimeMessage(1, new PostedTime(TimeSpan.FromMilliseconds(95571D), PostedTimeType.PersonalBest, 10))
            ).InAllSessions()
            .Assert();

            Translate(
                new SetSessionTypeMessage(SessionType.Practice, "SessionId"),
                new SetGridColumnValueMessage(1, GridColumn.Laps, GridColumnColour.White, "15"),
                new SetGridColumnValueMessage(1, GridColumn.LapTime, GridColumnColour.Magenta, "1:35.571")
            ).Expect(
                new SetDriverLapTimeMessage(1, new PostedTime(TimeSpan.FromMilliseconds(95571D), PostedTimeType.SessionBest, 15))
            ).InAllSessions()
            .Assert();
        }

        [Fact]
        public void session_type_is_updated_when_a_set_session_type_message_is_processed() {

            var translator = new LiveMessageTranslator();
            var message = new SetSessionTypeMessage(SessionType.Practice, "SessionId");

            Assert.Null(translator.Translate(message));
            Assert.Equal(message.SessionType, translator.SessionType);
        }

        [Fact]
        public void car_number_column_values_are_translated() {

            Translate(
                new SetGridColumnValueMessage(1, GridColumn.CarNumber, GridColumnColour.White, "1")
            ).Expect(
                new SetDriverCarNumberMessage(1, 1),
                new SetDriverStatusMessage(1, DriverStatus.OnTrack)
            ).InAllSessions(
            ).Assert();
        }

        [Fact]
        public void position_column_is_not_translated() {

            Translate(
                new SetGridColumnValueMessage(1, GridColumn.Position, GridColumnColour.Yellow, "10")
            ).ExpectNull()
            .InAllSessions()
            .Assert();

            Translate(
                new SetGridColumnValueMessage(1, GridColumn.Position, GridColumnColour.Yellow, "10"),
                new SetGridColumnColourMessage(1, GridColumn.Position, GridColumnColour.White)
            ).ExpectNull()
            .InAllSessions()
            .Assert();
        }

        [Fact]
        public void sector_1_column_colours_are_translated() {

            sector_column_colours_are_translated(GridColumn.S1, 1);
        }

        [Fact]
        public void sector_2_column_colours_are_translated() {

            sector_column_colours_are_translated(GridColumn.S2, 2);
        }

        [Fact]
        public void sector_3_column_colours_are_translated() {

            sector_column_colours_are_translated(GridColumn.S3, 3);
        }

        private void sector_column_colours_are_translated(GridColumn sectorColumn, int sectorNumber) {

            Translate(
                new SetGridColumnValueMessage(1, GridColumn.CarNumber, GridColumnColour.White, "1"),
                new SetGridColumnValueMessage(1, GridColumn.Laps, GridColumnColour.White, "1"),
                new SetGridColumnValueMessage(1, sectorColumn, GridColumnColour.White, "35.5"),
                new SetGridColumnColourMessage(1, sectorColumn, GridColumnColour.White)
            ).Expect(
                new ReplaceDriverSectorTimeMessage(1, sectorNumber, new PostedTime(TimeSpan.FromSeconds(35.5D), PostedTimeType.Normal, 1))
            ).InAllSessions()
            .Assert();

            Translate(
                new SetGridColumnValueMessage(1, GridColumn.CarNumber, GridColumnColour.White, "1"),
                new SetGridColumnValueMessage(1, GridColumn.Laps, GridColumnColour.White, "1"),
                new SetGridColumnValueMessage(1, sectorColumn, GridColumnColour.White, "35.5"),
                new SetGridColumnColourMessage(1, sectorColumn, GridColumnColour.Green)
            ).Expect(
                new ReplaceDriverSectorTimeMessage(1, sectorNumber, new PostedTime(TimeSpan.FromSeconds(35.5D), PostedTimeType.PersonalBest, 1))
            ).InAllSessions()
            .Assert();

            Translate(
                new SetGridColumnValueMessage(1, GridColumn.CarNumber, GridColumnColour.White, "1"),
                new SetGridColumnValueMessage(1, GridColumn.Laps, GridColumnColour.White, "1"),
                new SetGridColumnValueMessage(1, sectorColumn, GridColumnColour.White, "35.5"),
                new SetGridColumnColourMessage(1, sectorColumn, GridColumnColour.Magenta)
            ).Expect(
                new ReplaceDriverSectorTimeMessage(1, sectorNumber, new PostedTime(TimeSpan.FromSeconds(35.5D), PostedTimeType.SessionBest, 1))
            ).InAllSessions()
            .Assert();
        }

        [Fact(Skip = "Need test fixing")]
        public void sector_1_column_values_are_translated() {

            sector_column_values_are_translated(GridColumn.S1, 1);
        }

        [Fact(Skip = "Need test fixing")]
        public void sector_2_column_values_are_translated() {

            sector_column_values_are_translated(GridColumn.S2, 2);
        }

        [Fact(Skip = "Need test fixing")]
        public void sector_3_column_values_are_translated() {

            sector_column_values_are_translated(GridColumn.S3, 3);
        }

        private void sector_column_values_are_translated(GridColumn sectorColumn, int sectorNumber) {

            Translate(
                new SetGridColumnValueMessage(1, GridColumn.Laps, GridColumnColour.White, "5"),
                new SetGridColumnValueMessage(1, sectorColumn, GridColumnColour.White, "31.1")
            ).Expect(
                new SetDriverSectorTimeMessage(1, sectorNumber, new PostedTime(TimeSpan.FromSeconds(31.1D), PostedTimeType.Normal, 5))
            ).InAllSessions()
            .Assert();

            Translate(
                new SetGridColumnValueMessage(1, GridColumn.Laps, GridColumnColour.White, "5"),
                new SetGridColumnValueMessage(1, sectorColumn, GridColumnColour.Green, "31.1")
            ).Expect(
                new SetDriverSectorTimeMessage(1, sectorNumber, new PostedTime(TimeSpan.FromSeconds(31.1D), PostedTimeType.PersonalBest, 5))
            ).InAllSessions()
            .Assert();

            Translate(
                new SetGridColumnValueMessage(1, GridColumn.Laps, GridColumnColour.White, "5"),
                new SetGridColumnValueMessage(1, sectorColumn, GridColumnColour.Magenta, "31.1")
            ).Expect(
                new SetDriverSectorTimeMessage(1, sectorNumber, new PostedTime(TimeSpan.FromSeconds(31.1D), PostedTimeType.SessionBest, 5))
            ).InAllSessions()
            .Assert();
        }

        private Translation Translate(params Message[] messages) {

            if(messages == null || messages.Length == 0) {
                throw new ArgumentNullException("messages");
            }

            return new Translation(Assert, messages);
        }

        private sealed class Translation
        {
            private Action<Message> _validator;
            private readonly Message[] _messages;
            private readonly Assertions _assert;
            private readonly ICollection<SessionType> _sessionTypes = new HashSet<SessionType>();            

            public Translation(Assertions assert, params Message[] messages) {

                _messages = messages;
                _assert = assert;
            }

            public Translation Expect(Message expectation) {

                _validator = actual => _assert.PropertiesAreEqual(expectation, actual);

                return this;
            }

            public Translation Expect(params Message[] expectations) {

                _validator = actual => {
                    _assert.IsType<CompositeMessage>(actual);
                    var actuals = ((CompositeMessage)actual).Messages;
                    _assert.Equal(expectations.Length, actuals.Count);
                    for(int i = 0; i < expectations.Length; ++i) {
                        _assert.PropertiesAreEqual(expectations[i], actuals[i]);
                    }
                };

                return this;
            }

            public Translation ExpectNull() {

                _validator = actual => _assert.Null(actual);

                return this;
            }

            public Translation InAllSessions() {

                foreach(SessionType sessionType in Enum.GetValues(typeof(SessionType))) {
                    _sessionTypes.Add(sessionType);
                }

                return this;
            }

            public Translation InPracticeSession() {

                _sessionTypes.Add(SessionType.Practice);

                return this;
            }

            public Translation InQuallySession() {

                _sessionTypes.Add(SessionType.Qually);

                return this;
            }

            public Translation InRaceSession() {

                _sessionTypes.Add(SessionType.Race);

                return this;
            }

            public void Assert() {

                Debug.Assert(_validator != null);
                Debug.Assert(_sessionTypes.Count > 0);

                ValidateTranslationInEachSessionType(_validator);
            }

            private void ValidateTranslationInEachSessionType(Action<Message> validator) {

                Message translation = null;
                LiveMessageTranslator translator;

                foreach(var sessionType in _sessionTypes) {
                    translator = new LiveMessageTranslator();
                    translator.SessionType = sessionType;
                    foreach(var message in _messages) {
                        translation = translator.Translate(message);
                    }
                    validator(translation);
                }
            }
        }
    }
}
