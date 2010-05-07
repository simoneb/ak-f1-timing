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

namespace AK.F1.Timing.UI.Utility
{
    /// <summary>
    /// Contains methods for formatting timing primitives. This class is <see langword="static"/>.
    /// </summary>
    internal static class Format
    {
        #region Fields.

        private static readonly CultureInfo INV_CULTURE = CultureInfo.InvariantCulture;

        #endregion

        #region Public Interface.

        public static string SectorTime(TimeSpan? value) {

            if(value == null) {
                return String.Empty;
            }
            
            TimeSpan time = value.Value;

            return F("{0}.{1:0}", time.Minutes * 60 + time.Seconds, time.Milliseconds / 100);
        }

        public static string SectorTimeDelta(TimeSpan? value) {

            if(value == null) {
                return String.Empty;
            }

            string signSymbol;
            TimeSpan time = value.Value;

            if(time <= TimeSpan.Zero) {
                signSymbol = "-";
                time = TimeSpan.FromTicks(Math.Abs(time.Ticks));
            } else {
                signSymbol = "+";
            }

            return F("{0}{1}.{2:00}", signSymbol, time.Minutes * 60 + time.Seconds, time.Milliseconds / 10);
        }

        public static string GapTime(Gap value) {

            if(value == null) {
                return String.Empty;
            }

            TimeGap time = value as TimeGap;

            if(time != null) {
                return GapTime(time);
            }

            return GapTime((LapGap)value);
        }

        public static string LapTime(TimeSpan? value) {

            if(value == null) {
                return String.Empty;
            }

            TimeSpan time = value.Value;

            return F("{0}:{1:00}.{2:000}", time.Minutes, time.Seconds, time.Milliseconds);
        }

        public static string LapTimeDelta(TimeSpan? value) {

            if(value == null) {
                return String.Empty;
            }

            string signSymbol;
            TimeSpan time = value.Value;

            if(time <= TimeSpan.Zero) {
                signSymbol = "-";
                time = TimeSpan.FromTicks(Math.Abs(time.Ticks));
            } else {
                signSymbol = "+";
            }

            return F("{0}{1}.{2:000}", signSymbol, time.Minutes * 60 + time.Seconds, time.Milliseconds);
        }

        public static string LapTime(PostedTime value) {

            if(value == null) {
                return String.Empty;
            }

            return LapTime(value.Time);
        }

        public static string Temperature(double? value) {

            if(value == null) {
                return String.Empty;
            }

            return F("{0:0.0}°C", value.Value);
        }

        public static string Pressure(double? value) {

            if(value == null) {
                return String.Empty;
            }

            return F("{0:0.0}mBar", value.Value);
        }

        public static string WindSpeed(double? value) {

            if(value == null) {
                return String.Empty;
            }

            return F("{0:0.00}m/s", value.Value);
        }

        public static string WindDirection(double? value) {

            if(value == null) {
                return String.Empty;
            }

            return F("{0:#}°", value.Value);
        }

        public static string Laps(int value) {

            return value.ToString(INV_CULTURE);
        }

        public static string PitCount(int value) {

            return value.ToString(INV_CULTURE);
        }

        public static object Humidity(double? value) {

            if(value == null) {
                return String.Empty;
            }

            return F("{0:0.00}%", value.Value);
        }

        #endregion

        #region Private Impl.

        private static string GapTime(LapGap value) {

            return value.Laps > 0 ? value.Laps.ToString(INV_CULTURE) + "L" : String.Empty;
        }

        private static string GapTime(TimeGap value) {

            return value.Time > TimeSpan.Zero ? SectorTime(value.Time) : String.Empty;
        }

        private static string F(string format, params object[] args) {

            return string.Format(INV_CULTURE, format, args);
        }

        #endregion
    }
}