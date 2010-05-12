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
using System.Globalization;
using AK.F1.Timing.Messages.Driver;
using AK.F1.Timing.Messages.Session;

namespace AK.F1.Timing.Live
{
    /// <summary>
    /// Provides statis utility methods for parsing F1 timing primitives. This class is
    /// <see langword="static"/>.
    /// </summary>    
    internal static class LiveData
    {
        #region Private Fields.

        private static readonly string[] TimeFormats = new[]
        {
            "ss.f", // Sector or qually time.
            "m:ss.fff", // Lap time.
            "mm:ss.fff", // Lap time.
            "s.fff", // Gap time.
            "ss.fff", // Gap time.
            "h:mm:ss.fff", // Huge lap time.
            "h:mm:ss", // Remaining session time.
            "mm:ss", // Remaining session time.
            "m:ss", // Remaining session time.
        };

        private static readonly GridColumn[][] GridColumnMap = {
            // Practice.
            new[]
            {
                GridColumn.Position,
                GridColumn.CarNumber,
                GridColumn.DriverName,
                GridColumn.LapTime,
                GridColumn.Gap,
                GridColumn.S1,
                GridColumn.S2,
                GridColumn.S3,
                GridColumn.Laps,
                GridColumn.Unknown
            },
            // Qually.
            new[]
            {
                GridColumn.Position,
                GridColumn.CarNumber,
                GridColumn.DriverName,
                GridColumn.Q1,
                GridColumn.Q2,
                GridColumn.Q3,
                GridColumn.S1,
                GridColumn.S2,
                GridColumn.S3,
                GridColumn.Laps
            },
            // Race
            new[]
            {
                GridColumn.Position,
                GridColumn.CarNumber,
                GridColumn.DriverName,
                GridColumn.Gap,
                GridColumn.Interval,
                GridColumn.LapTime,
                GridColumn.S1,
                GridColumn.PitLap1,
                GridColumn.S2,
                GridColumn.PitLap2,
                GridColumn.S3,
                GridColumn.PitLap3,
                GridColumn.PitCount
            }
        };

        #endregion

        #region Public Interface.

        /// <summary>
        /// Parses a <see cref="System.TimeSpan"/> from the specified string.
        /// </summary>
        /// <param name="s">The string to parse.</param>
        /// <returns>The parsed <see cref="System.TimeSpan"/>.</returns>
        /// <exception cref="System.Runtime.Serialization.SerializationException">
        /// Thrown when <paramref name="s"/> is incorrectly formatted.
        /// </exception>
        /// <remarks>
        /// This method is capabable of parsing all known grid time formats, including: lap, sector
        /// interval gap, elapsed session and remaining session times. It will not parse interval
        /// or gaps times in the ##L format.
        /// </remarks>
        public static TimeSpan ParseTime(string s)
        {
            DateTime date;

            if(DateTime.TryParseExact(s, TimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                return date.TimeOfDay;
            }

            double seconds;
            // Sector times may be greater than 60 seconds in which case they are formatted as 73.7
            // This format cannot be expressed as a time format so revert to parsing it as a double.
            if(Double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out seconds))
            {
                return TimeSpan.FromSeconds(seconds);
            }

            throw Guard.LiveData_UnableToParseTime(s);
        }

        /// <summary>
        /// Parses a <see cref="System.Int32"/> from the specified string.
        /// </summary>
        /// <param name="s">The string to parse.</param>
        /// <returns>The parsed <see cref="System.Int32"/>.</returns>
        /// <exception cref="System.Runtime.Serialization.SerializationException">
        /// Thrown when <paramref name="s"/> is incorrectly formatted.
        /// </exception>
        public static int ParseInt32(string s)
        {
            int value;

            if(Int32.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out value))
            {
                return value;
            }

            throw Guard.LiveData_UnableToParseInt32(s);
        }

        /// <summary>
        /// Parses a <see cref="System.Double"/> from the specified string.
        /// </summary>
        /// <param name="s">The string to parse.</param>
        /// <returns>The parsed <see cref="System.Double"/>.</returns>
        /// <exception cref="System.Runtime.Serialization.SerializationException">
        /// Thrown when <paramref name="s"/> is incorrectly formatted.
        /// </exception>
        public static double ParseDouble(string s)
        {
            double value;

            if(Double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
            {
                return value;
            }

            throw Guard.LiveData_UnableToParseDouble(s);
        }

        /// <summary>
        /// Converts the specified timing colour into a
        /// <see cref="AK.F1.Timing.Messages.Driver.PostedTimeType"/>.
        /// </summary>
        /// <param name="colour">The value to convert.</param>
        /// <returns>The converted value.</returns>
        /// <exception cref="System.Runtime.Serialization.SerializationException">
        /// Thrown when <paramref name="colour"/> could not be converted.
        /// </exception>
        public static PostedTimeType ToPostedTimeType(GridColumnColour colour)
        {
            switch(colour)
            {
                case GridColumnColour.Green:
                    return PostedTimeType.PersonalBest;
                case GridColumnColour.Magenta:
                    return PostedTimeType.SessionBest;
                case GridColumnColour.Yellow:
                case GridColumnColour.White:
                    return PostedTimeType.Normal;
                default:
                    throw Guard.LiveData_UnableToConvertToPostedTimeType((int)colour);
            }
        }

        /// <summary>
        /// Converts the specified timing session value into a
        /// <see cref="AK.F1.Timing.Messages.Session.SessionType"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        /// <exception cref="System.Runtime.Serialization.SerializationException">
        /// Thrown when <paramref name="value"/> could not be converted.
        /// </exception>
        public static SessionType ToSessionType(int value)
        {
            switch(value)
            {
                case 0:
                    return SessionType.None;
                case 1:
                    return SessionType.Race;
                case 2:
                    return SessionType.Practice;
                case 3:
                case 4:
                case 5:
                    return SessionType.Qually;
                default:
                    throw Guard.LiveData_UnableToConvertToSessionType(value);
            }
        }

        /// <summary>
        /// Converts the specified timing status value to a
        /// <see cref="AK.F1.Timing.Messages.Session.SessionStatus"/>.
        /// </summary>
        /// <param name="s">The value to convert.</param>
        /// <returns>The converted value.</returns>
        /// <exception cref="System.Runtime.Serialization.SerializationException">
        /// Thrown when <paramref name="s"/> could not be converted.
        /// </exception>
        public static SessionStatus ToSessionStatus(string s)
        {
            if(s != null && s.Length == 1)
            {
                switch(s[0])
                {
                    case '1':
                        return SessionStatus.Green;
                    case '2':
                        return SessionStatus.Yellow;
                    case '3':
                        return SessionStatus.SafetyCarOnStandBy;
                    case '4':
                        return SessionStatus.SafetyCarDeployed;
                    case '5':
                        return SessionStatus.Red;
                }
            }

            throw Guard.LiveData_UnableToConvertToSessionStatus(s);
        }

        /// <summary>
        /// Converts the specified timing colour into a
        /// <see cref="AK.F1.Timing.Messages.Driver.DriverStatus"/>.
        /// </summary>
        /// <param name="colour">The value to convert.</param>
        /// <returns>The converted value.</returns>
        /// <exception cref="System.Runtime.Serialization.SerializationException">
        /// Thrown when <paramref name="colour"/> could not be converted.
        /// </exception>
        public static DriverStatus ToDriverStatus(GridColumnColour colour)
        {
            switch(colour)
            {
                case GridColumnColour.White:
                case GridColumnColour.Magenta:
                case GridColumnColour.Yellow:
                    return DriverStatus.OnTrack;
                case GridColumnColour.Red:
                    return DriverStatus.InPits;
                default:
                    throw Guard.LiveData_UnableToConvertToDriverStatus((int)colour);
            }
        }

        /// <summary>
        /// Converts the specified timing coloum index to a 
        /// <see cref="AK.F1.Timing.Messages.Driver.GridColumn"/>.
        /// </summary>
        /// <param name="column">The value to convert.</param>
        /// <param name="currentSessionType">The current session.</param>
        /// <returns>The converted value.</returns>
        public static GridColumn ToGridColumn(byte column, SessionType currentSessionType)
        {
            try
            {
                return GridColumnMap[(int)currentSessionType - 1][column - 1];
            }
            catch(IndexOutOfRangeException)
            {
                throw Guard.LiveData_UnableToConvertToGridColumn(column, currentSessionType);
            }
        }

        /// <summary>
        /// Converts the specified timing colour into a
        /// <see cref="AK.F1.Timing.Messages.Driver.GridColumnColour"/>.
        /// </summary>
        /// <param name="colour">The value to convert.</param>
        /// <returns>The converted value.</returns>
        /// <exception cref="System.Runtime.Serialization.SerializationException">
        /// Thrown when <paramref name="colour"/> could not be converted.
        /// </exception>
        public static GridColumnColour ToGridColumnColour(byte colour)
        {
            switch(colour)
            {
                case 0:
                    return GridColumnColour.Black;
                case 1:
                    return GridColumnColour.White;
                case 2:
                    return GridColumnColour.Red;
                case 3:
                    return GridColumnColour.Green;
                case 4:
                    return GridColumnColour.Magenta;
                case 5:
                    return GridColumnColour.Blue;
                case 6:
                    return GridColumnColour.Yellow;
                case 7:
                    return GridColumnColour.Grey;
                default:
                    throw Guard.LiveData_UnableToConvertToGridColumnColour(colour);
            }
        }

        #endregion
    }
}