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

namespace AK.F1.Timing.Utility.Tms.Operations
{
    /// <summary>
    /// Contains methods for formatting timing primitives. This class is <see langword="static"/>.
    /// </summary>
    internal static class Format
    {
        #region Public Interface.

        public static string SectorTime(PostedTime value)
        {
            if(value == null)
            {
                return String.Empty;
            }

            TimeSpan time = value.Time;

            return F("{0}.{1:0}", time.Minutes * 60 + time.Seconds, time.Milliseconds / 100);
        }

        public static string LapTime(PostedTime value)
        {
            return value != null ? LapTime(value.Time) : String.Empty;
        }

        public static string LapTime(TimeSpan? value)
        {
            if(value == null)
            {
                return String.Empty;
            }

            TimeSpan time = value.Value;

            return F("{0}:{1:00}.{2:000}", time.Minutes, time.Seconds, time.Milliseconds);
        }

        public static string Temperature(double? value)
        {
            if(value == null)
            {
                return String.Empty;
            }

            return F("{0:0.0}°C", value.Value);
        }

        public static string Pressure(double? value)
        {
            if(value == null)
            {
                return String.Empty;
            }

            return F("{0:0.0}mBar", value.Value);
        }

        public static string WindSpeed(double? value)
        {
            if(value == null)
            {
                return String.Empty;
            }

            return F("{0:0.00}m/s", value.Value);
        }

        public static string WindDirection(double? value)
        {
            if(value == null)
            {
                return String.Empty;
            }

            return F("{0:#}°", value.Value);
        }

        public static string Laps(int value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public static string PitCount(int value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public static object Humidity(double? value)
        {
            if(value == null)
            {
                return String.Empty;
            }

            return F("{0:0.00}%", value.Value);
        }

        #endregion

        #region Private Impl.

        private static string F(string format, params object[] args)
        {
            return string.Format(CultureInfo.InvariantCulture, format, args);
        }

        #endregion
    }
}