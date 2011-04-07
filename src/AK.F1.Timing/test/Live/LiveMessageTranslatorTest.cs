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
using AK.F1.Timing.Messages.Driver;
using AK.F1.Timing.Messages.Session;
using Xunit;
using Xunit.Extensions;

namespace AK.F1.Timing.Live
{
    public partial class LiveMessageTranslatorTest : TestBase
    {
        [Fact]
        public void can_create()
        {
            var translator = new LiveMessageTranslator();

            Assert.Equal(0, translator.RaceLapNumber);
            Assert.Equal(SessionType.None, translator.SessionType);
            Assert.True(translator.IsSessionStarted);
        }

        [Fact]
        public void can_change_the_session_type()
        {
            var translator = new LiveMessageTranslator();

            translator.ChangeSessionType(SessionType.Practice);
            Assert.Equal(SessionType.Practice, translator.SessionType);
        }

        [Fact]
        public void changing_the_session_type_resets_the_translator()
        {
            var translator = new LiveMessageTranslator();

            translator.RaceLapNumber = 5;
            translator.ChangeSessionType(SessionType.Qually);
            Assert.Equal(0, translator.RaceLapNumber);
        }

        [Fact]
        public void changing_to_the_same_session_type_does_not_reset_the_translator()
        {
            var translator = new LiveMessageTranslator();

            translator.ChangeSessionType(SessionType.Practice);
            translator.RaceLapNumber = 5;
            translator.ChangeSessionType(SessionType.Practice);
            Assert.Equal(5, translator.RaceLapNumber);
        }

        [Fact]
        public void can_determine_if_the_session_has_started()
        {
            var translator = new LiveMessageTranslator();

            translator.ChangeSessionType(SessionType.None);
            Assert.True(translator.IsSessionStarted);

            translator.ChangeSessionType(SessionType.Practice);
            Assert.True(translator.IsSessionStarted);

            translator.ChangeSessionType(SessionType.Qually);
            Assert.True(translator.IsSessionStarted);

            translator.ChangeSessionType(SessionType.Race);
            Assert.False(translator.IsSessionStarted);
            translator.RaceLapNumber = 1;
            Assert.True(translator.IsSessionStarted);
        }

        [Fact]
        public void can_determine_if_it_a_race_session()
        {
            var translator = new LiveMessageTranslator();

            translator.ChangeSessionType(SessionType.Race);
            Assert.True(translator.IsRaceSession);

            translator.ChangeSessionType(SessionType.None);
            Assert.False(translator.IsRaceSession);
            translator.ChangeSessionType(SessionType.Practice);
            Assert.False(translator.IsRaceSession);
            translator.ChangeSessionType(SessionType.Qually);
            Assert.False(translator.IsRaceSession);
        }

        [Fact]
        public void can_get_a_driver_by_id()
        {
            var translator = new LiveMessageTranslator();

            Assert.NotNull(translator.GetDriver(1));
            Assert.NotNull(translator.GetDriver(new SetDriverPositionMessage(1, 1)));
        }

        [Fact]
        public void get_driver_returns_the_same_driver_given_the_same_id_or_message()
        {
            var translator = new LiveMessageTranslator();
            var message = new SetDriverPositionMessage(1, 1);

            Assert.Same(translator.GetDriver(1), translator.GetDriver(1));
            Assert.Same(translator.GetDriver(message), translator.GetDriver(message));
            Assert.Same(translator.GetDriver(1), translator.GetDriver(message));
        }

        [Theory]
        [InlineData("J. D'AMBROSIO", "J. D'AMBROSIO")]
        [InlineData("J. D'AMBROSIO", "DAM")]
        [InlineData("J. D'AMBROSIO", "JDA")]
        public void can_get_a_driver_by_name(string driverName, string searchName)
        {
            var translator = new LiveMessageTranslator();
            var expected = translator.GetDriver(1);

            expected.Name = driverName;
            Assert.Same(expected, translator.GetDriver(searchName));
        }

        [Fact]
        public void cannot_get_a_driver_by_an_ambiguous_name()
        {
            var translator = new LiveMessageTranslator();

            translator.GetDriver(1).Name = "M. SCHUMACHER";
            translator.GetDriver(2).Name = "R. SCHUMACHER";

            Assert.Null(translator.GetDriver("SCHUMACHER"));
            Assert.Null(translator.GetDriver("SCH"));

            Assert.NotNull(translator.GetDriver("MSC"));
            Assert.NotNull(translator.GetDriver("RSC"));
        }

        [Fact]
        public void translate_throws_if_message_is_null()
        {
            var translator = new LiveMessageTranslator();

            Assert.Throws<ArgumentNullException>(() => translator.Translate(null));
        }

        [Fact]
        public void when_a_set_session_type_message_is_processed_the_session_type_is_updated()
        {
            var translator = new LiveMessageTranslator();
            var message = new SetSessionTypeMessage(SessionType.Practice, "SessionId");

            Assert.Null(translator.Translate(message));
            Assert.Equal(message.SessionType, translator.SessionType);
        }

        [Theory]
        [ClassData(typeof(AllSessionTypes))]
        public void colour_updates_to_a_column_with_no_value_are_not_translated(SessionType session)
        {
            var combinations = from column in Enum.GetValues(typeof(GridColumn)).Cast<GridColumn>()
                               from colour in Enum.GetValues(typeof(GridColumnColour)).Cast<GridColumnColour>()
                               select new { Column = column, Colour = colour };

            foreach(var combination in combinations)
            {
                In(session).Assert(translator => { Assert.Null(translator.Translate(new SetGridColumnColourMessage(1, combination.Column, combination.Colour))); });
            }
        }

        [Theory]
        [ClassData(typeof(AllSessionTypes))]
        public void unknown_column_values_are_not_translated(SessionType session)
        {
            foreach(GridColumnColour colour in Enum.GetValues(typeof(GridColumnColour)))
            {
                In(session).Assert(translator =>
                {
                    Assert.Null(translator.Translate(new SetGridColumnValueMessage(1, GridColumn.Unknown, colour, null)));
                    Assert.Null(translator.Translate(new SetGridColumnValueMessage(1, GridColumn.Unknown, colour, "Unknown")));
                });
            }
        }

        [Theory]
        [ClassData(typeof(AllSessionTypes))]
        public void unknown_column_colours_are_not_translated(SessionType session)
        {
            foreach(GridColumnColour colour in Enum.GetValues(typeof(GridColumnColour)))
            {
                In(session).Assert(translator => { Assert.Null(translator.Translate(new SetGridColumnColourMessage(1, GridColumn.Unknown, colour))); });
            }
        }

        [Theory]
        [ClassData(typeof(AllSessionTypes))]
        public void driver_name_column_values_are_translated_into_set_driver_name_messages(SessionType session)
        {
            In(session).Assert(translator =>
            {
                Assert.MessagesAreEqual(
                    new SetDriverNameMessage(1, "A. DRIVER"),
                    translator.Translate(new SetGridColumnValueMessage(1, GridColumn.DriverName, GridColumnColour.White, "A. DRIVER"))
                );
            });
        }

        [Theory]
        [ClassData(typeof(AllSessionTypes))]
        public void position_column_values_are_not_translated_as_positions_are_provided_by_the_feed(SessionType session)
        {
            In(session).Assert(translator => { Assert.Null(translator.Translate(new SetGridColumnValueMessage(1, GridColumn.Position, GridColumnColour.White, "10"))); });
        }

        private static PostedTime PT(double seconds, PostedTimeType type, int lapNumber)
        {
            return new PostedTime(TimeSpan.FromSeconds(seconds), type, lapNumber);
        }

        private static Scenario In(SessionType session)
        {
            return new Scenario(session);
        }

        private sealed class Scenario
        {
            private readonly LiveMessageTranslator _translator;

            public Scenario(SessionType inSession)
            {
                _translator = new LiveMessageTranslator();
                _translator.ChangeSessionType(inSession);
            }

            public Scenario OnLap(int raceLapNumber)
            {
                _translator.RaceLapNumber = raceLapNumber;

                return this;
            }

            public void Assert(Action<LiveMessageTranslator> assertions)
            {
                assertions(_translator);
            }
        }

        private sealed class AllSessionTypes : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                foreach(SessionType type in Enum.GetValues(typeof(SessionType)))
                {
                    yield return new object[] { type };
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private sealed class AllSessionTypesExceptRace : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                foreach(SessionType type in Enum.GetValues(typeof(SessionType)))
                {
                    if(type != SessionType.Race)
                    {
                        yield return new object[] { type };
                    }
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private sealed class AllSectorGridColumns_AllSessionTypes : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                return (from sector in A(GridColumn.S1, GridColumn.S2, GridColumn.S3)
                        from session in Enum.GetValues(typeof(SessionType)).Cast<object>()
                        select A(sector, session)).GetEnumerator();
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