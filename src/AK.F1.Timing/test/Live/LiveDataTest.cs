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
using System.Runtime.Serialization;
using Xunit;
using Xunit.Extensions;

using AK.F1.Timing.Messages.Driver;
using AK.F1.Timing.Messages.Session;

namespace AK.F1.Timing.Live
{
    public class LiveDataTest
    {
        [Theory]
        [InlineData("0", 0, 0)]
        [InlineData("0.0", 0, 0)]
        [InlineData("00.0", 0, 0)]
        [InlineData("00.0", 0, 0)]
        [InlineData("00.00", 0, 0)]
        [InlineData("0.000", 0, 0)]
        [InlineData("00.000", 0, 0)]
        [InlineData("70", 70, 0)]
        [InlineData("0.7", 0, 700)]
        [InlineData("7.0", 7, 0)]
        [InlineData("7.4", 7, 400)]
        [InlineData("17.0", 17, 0)]
        [InlineData("17.4", 17, 400)]
        [InlineData("70.4", 70, 400)]
        [InlineData("7.00", 7, 0)]
        [InlineData("7.44", 7, 440)]
        [InlineData("17.44", 17, 440)]
        [InlineData("70.44", 70, 440)]
        [InlineData("7.000", 7, 0)]
        [InlineData("7.444", 7, 444)]
        [InlineData("17.444", 17, 444)]
        [InlineData("70.444", 70, 444)]
        public void can_parse_sector_time(string s, int seconds, int milliseconds) {

            Assert.Equal(TimeSpan.FromMilliseconds(seconds * 1000 + milliseconds), LiveData.ParseTime(s));
        }

        [Theory]
        [InlineData("0:00.000", 0, 0, 0)]
        [InlineData("00:00.000", 0, 0, 0)]
        [InlineData("0:00:00.000", 0, 0, 0)]
        [InlineData("00:00:00.000", 0, 0, 0)]
        [InlineData("1:00.000", 1, 0, 0)]
        [InlineData("1:15.000", 1, 15, 0)]
        [InlineData("1:15.444", 1, 15, 444)]
        [InlineData("10:00.000", 10, 0, 0)]
        [InlineData("10:15.000", 10, 15, 0)]
        [InlineData("10:15.444", 10, 15, 444)]
        public void can_parse_lap_time(string s, int minutes, int seconds, int milliseconds) {

            Assert.Equal(TimeSpan.FromMilliseconds((minutes * 60 + seconds) * 1000 + milliseconds), LiveData.ParseTime(s));
        }

        [Theory]
        [InlineData("0:00", 0, 0, 0)]
        [InlineData("00:00", 0, 0, 0)]
        [InlineData("0:00:00", 0, 0, 0)]
        [InlineData("7:00", 0, 7, 0)]
        [InlineData("7:44", 0, 7, 44)]
        [InlineData("17:00", 0, 17, 0)]
        [InlineData("17:44", 0, 17, 44)]
        [InlineData("7:00:00", 7, 0, 0)]
        [InlineData("7:17:00", 7, 17, 0)]
        [InlineData("7:17:17", 7, 17, 17)]
        public void can_parse_remaining_session_time(string s, int hours, int minutes, int seconds) {

            Assert.Equal(TimeSpan.FromMilliseconds(((hours * 60 + minutes) * 60 + seconds) * 1000), LiveData.ParseTime(s));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void parse_time_throws_a_serialization_exception_if_time_cannot_be_parsed(string s) {

            Assert.Throws<SerializationException>(() => LiveData.ParseTime(s));
        }

        [Theory]
        [InlineData("0", 0)]
        [InlineData("0.0", 0.0)]
        [InlineData("123.45", 123.45)]
        public void can_parse_a_double(string s, double expected) {

            Assert.Equal(expected, LiveData.ParseDouble(s));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void parse_double_throws_a_serialization_exception_if_double_cannot_be_parsed(string s) {

            Assert.Throws<SerializationException>(() => LiveData.ParseDouble(s));
        }

        [Theory]
        [InlineData("0", 0)]
        [InlineData("123", 123)]
        public void can_parse_a_int32(string s, int expected) {

            Assert.Equal(expected, LiveData.ParseInt32(s));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("1.0")]
        [InlineData("9999999999999999999")]
        public void parse_double_throws_a_serialization_exception_if_int32_cannot_be_parsed(string s) {

            Assert.Throws<SerializationException>(() => LiveData.ParseInt32(s));
        }

        [Theory]
        [InlineData(GridColumnColour.White, DriverStatus.OnTrack)]
        [InlineData(GridColumnColour.Magenta, DriverStatus.OnTrack)]
        [InlineData(GridColumnColour.Yellow, DriverStatus.OnTrack)]
        [InlineData(GridColumnColour.Red, DriverStatus.InPits)]
        public void can_parse_a_driver_status(GridColumnColour colour, DriverStatus expected) {

            Assert.Equal(expected, LiveData.ToDriverStatus(colour));
        }

        [Theory]
        [InlineData(GridColumnColour.Black)]
        [InlineData(GridColumnColour.Blue)]
        [InlineData(GridColumnColour.Green)]
        [InlineData(GridColumnColour.Grey)]
        public void to_driver_status_throws_a_serialization_exception_if_colour_cannot_be_converted(GridColumnColour colour) {

            Assert.Throws<SerializationException>(() => LiveData.ToDriverStatus(colour));
        }

        [Theory]
        [InlineData(0, SessionType.None)]
        [InlineData(1, SessionType.Race)]
        [InlineData(2, SessionType.Practice)]
        [InlineData(3, SessionType.Qually)]
        [InlineData(4, SessionType.Qually)]
        [InlineData(5, SessionType.Qually)]
        public void can_parse_a_session_type(int value, SessionType expected) {

            Assert.Equal(expected, LiveData.ToSessionType(value));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(6)]
        public void to_session_type_throws_a_serialization_exception_if_value_cannot_be_converted(int value) {

            Assert.Throws<SerializationException>(() => LiveData.ToSessionType(value));
        }

        [Theory]
        [InlineData(GridColumnColour.White, PostedTimeType.Normal)]
        // TODO why is yellow here?
        [InlineData(GridColumnColour.Yellow, PostedTimeType.Normal)]
        [InlineData(GridColumnColour.Green, PostedTimeType.PersonalBest)]
        [InlineData(GridColumnColour.Magenta, PostedTimeType.SessionBest)]
        public void can_parse_a_posted_type_type(GridColumnColour colour, PostedTimeType expected) {

            Assert.Equal(expected, LiveData.ToPostedTimeType(colour));
        }

        [Theory]
        [InlineData(GridColumnColour.Black)]
        [InlineData(GridColumnColour.Blue)]
        [InlineData(GridColumnColour.Grey)]
        [InlineData(GridColumnColour.Red)]
        public void to_posted_time_type_throws_a_serialization_exception_if_colour_cannot_be_converted(GridColumnColour colour) {

            Assert.Throws<SerializationException>(() => LiveData.ToPostedTimeType(colour));
        }

        [Theory]
        [InlineData("1", SessionStatus.Green)]
        [InlineData("2", SessionStatus.Yellow)]
        [InlineData("3", SessionStatus.SafetyCarOnStandBy)]
        [InlineData("4", SessionStatus.SafetyCarDeployed)]
        [InlineData("5", SessionStatus.Red)]
        public void can_parse_a_session_status(string s, SessionStatus expected) {

            Assert.Equal(expected, LiveData.ToSessionStatus(s));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("0")]
        [InlineData("6")]
        [InlineData("01")]
        public void to_session_status_throws_a_serialization_exception_if_value_cannot_be_converted(string value) {

            Assert.Throws<SerializationException>(() => LiveData.ToSessionStatus(value));
        }

        [Theory]
        [InlineData(1, GridColumn.Position)]
        [InlineData(2, GridColumn.CarNumber)]
        [InlineData(3, GridColumn.DriverName)]
        [InlineData(4, GridColumn.LapTime)]
        [InlineData(5, GridColumn.Gap)]
        [InlineData(6, GridColumn.S1)]
        [InlineData(7, GridColumn.S2)]
        [InlineData(8, GridColumn.S3)]
        [InlineData(9, GridColumn.Laps)]
        [InlineData(10, GridColumn.Unknown)]
        public void can_parse_a_grid_column_in_practice_session(int column, GridColumn expected) {

            Assert.Equal(expected, LiveData.ToGridColumn(checked((byte)column), SessionType.Practice));
        }

        [Theory]
        [InlineData(1, GridColumn.Position)]
        [InlineData(2, GridColumn.CarNumber)]
        [InlineData(3, GridColumn.DriverName)]
        [InlineData(4, GridColumn.Q1)]
        [InlineData(5, GridColumn.Q2)]
        [InlineData(6, GridColumn.Q3)]
        [InlineData(7, GridColumn.S1)]
        [InlineData(8, GridColumn.S2)]
        [InlineData(9, GridColumn.S3)]
        [InlineData(10, GridColumn.Laps)]
        public void can_parse_a_grid_column_in_qually_session(int column, GridColumn expected) {

            Assert.Equal(expected, LiveData.ToGridColumn(checked((byte)column), SessionType.Qually));
        }

        [Theory]
        [InlineData(1, GridColumn.Position)]
        [InlineData(2, GridColumn.CarNumber)]
        [InlineData(3, GridColumn.DriverName)]
        [InlineData(4, GridColumn.Gap)]
        [InlineData(5, GridColumn.Interval)]
        [InlineData(6, GridColumn.LapTime)]
        [InlineData(7, GridColumn.S1)]
        [InlineData(8, GridColumn.PitLap1)]
        [InlineData(9, GridColumn.S2)]
        [InlineData(10, GridColumn.PitLap2)]
        [InlineData(11, GridColumn.S3)]
        [InlineData(12, GridColumn.PitLap3)]
        [InlineData(13, GridColumn.PitCount)]        
        public void can_parse_a_grid_column_in_race_session(int column, GridColumn expected) {

            Assert.Equal(expected, LiveData.ToGridColumn(checked((byte)column), SessionType.Race));
        }

        [Fact]
        public void to_grid_column_throws_a_serialization_exception_when_current_session_type_is_none() {

            // Go through the entire column range, including out of bound data.
            for(byte column = 0; column < 15; ++column) {
                Assert.Throws<SerializationException>(() => LiveData.ToGridColumn(column, SessionType.None));
            }
        }

        [Theory]
        [InlineData(0, SessionType.Practice)]
        [InlineData(11, SessionType.Practice)]
        [InlineData(0, SessionType.Qually)]
        [InlineData(11, SessionType.Qually)]
        [InlineData(0, SessionType.Race)]
        [InlineData(14, SessionType.Race)]
        public void to_grid_column_throws_a_serialization_exception_if_column_cannot_be_converted(int column, SessionType currentSessionType) {

            Assert.Throws<SerializationException>(() => LiveData.ToGridColumn(checked((byte)column), currentSessionType));
        }

        [Theory]
        [InlineData(0, GridColumnColour.Black)]
        [InlineData(1, GridColumnColour.White)]
        [InlineData(2, GridColumnColour.Red)]
        [InlineData(3, GridColumnColour.Green)]
        [InlineData(4, GridColumnColour.Magenta)]
        [InlineData(5, GridColumnColour.Blue)]
        [InlineData(6, GridColumnColour.Yellow)]
        [InlineData(7, GridColumnColour.Grey)]
        public void can_parse_a_grid_column_colour(int colour, GridColumnColour expected) {

            Assert.Equal(expected, LiveData.ToGridColumnColour(checked((byte)colour)));
        }

        [Theory]        
        [InlineData(8)]
        public void to_grid_column_colour_throws_a_serialization_exception_if_colour_cannot_be_converted(int colour) {

            Assert.Throws<SerializationException>(() => LiveData.ToGridColumnColour(checked((byte)colour)));
        }
    }
}
