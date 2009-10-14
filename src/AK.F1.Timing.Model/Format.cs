﻿// Copyright 2009 Andy Kernahan
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

using AK.F1.Timing.Messaging.Messages.Driver;

namespace AK.F1.Timing.Model
{
    public static class Format
    {
        private static readonly CultureInfo INV_CULTURE = CultureInfo.InvariantCulture;

        public static string SectorTime(TimeSpan? value) {

            if(value == null) {
                return String.Empty;
            }

            TimeSpan time = value.Value;

            return F("{0}.{1}", time.Minutes * 60 + time.Seconds, time.Milliseconds / 100);
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

        public static string GapTime(LapGap value) {

            return value.Laps > 0 ? value.Laps.ToString(INV_CULTURE) + "L" : String.Empty;
        }

        public static string GapTime(TimeGap value) {

            return value.Time > TimeSpan.Zero ? SectorTime(value.Time) : String.Empty;
        }

        public static string LapTime(TimeSpan? value) {

            if(value == null) {
                return String.Empty;
            }

            TimeSpan time = value.Value;

            return F("{0}:{1}.{2:000}", time.Minutes, time.Seconds, time.Milliseconds);
        }

        public static string LapTime(PostedTime value) {

            if(value == null) {
                return String.Empty;
            }

            TimeSpan time = value.Time;

            return F("{0,2}:{1}.{2:000}", time.Minutes, time.Seconds, time.Milliseconds);
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

        internal static string Laps(int value) {

            return value.ToString(INV_CULTURE);
        }

        internal static string PitCount(int value) {

            return value.ToString(INV_CULTURE);
        }

        private static string F(string format, params object[] args) {            

            return string.Format(INV_CULTURE, format, args);
        }

        internal static object Humidity(double? value) {

            if(value == null) {
                return String.Empty;
            }

            return F("{0:0.00}%", value.Value);
        }
    }
}