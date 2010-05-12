// Copyright 2009 Andy Kernahan
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use session file except in compliance with the License.
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
using System.Linq;
using AK.F1.Timing.Messages.Driver;
using AK.F1.Timing.Messages.Session;
using AK.F1.Timing.Model.Grid;
using AK.F1.Timing.Model.Session;

namespace AK.F1.Timing.Utility.Tms.Operations
{
    internal static class SessionModelPrinter
    {
        public static void Print(this SessionModel session)
        {
            switch(session.SessionType)
            {
                case SessionType.Practice:
                    WritePractice(session);
                    break;
                case SessionType.Qually:
                    WriteQually(session);
                    break;
                case SessionType.Race:
                    WriteRace(session);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            WriteTrackStatistics(session);
            WriteSessionStatistics(session);
        }

        private static void WriteQually(SessionModel session)
        {
            WriteLine("+---+---+---+--------------------+-----------+-----------+-----------+-------+-------+-------+----+");
            WriteLine("| I | P | C | DRIVER             |     Q1    |     Q2    |     Q3    |    S1 |    S2 |    S3 |  L |");
            WriteLine("+---+---+---+--------------------+-----------+-----------+-----------+-------+-------+-------+----+");
            foreach(var row in ((QuallyGridModel)session.Grid).Rows.OrderBy(r => r.RowIndex))
            {
                Write("|");
                Write("{0,3}", row.Id);
                Write("|");
                Write("{0,3}", row.Position);
                Write("|");
                Write("{0,3}", row.CarNumber);
                Write("|");
                Write("{0,-20}", row.DriverName);
                Write("|");
                Write("{0,11}", row.Q1);
                Write("|");
                Write("{0,11}", row.Q2);
                Write("|");
                Write("{0,11}", row.Q3);
                Write("|");
                Write("{0,7}", row.S1);
                Write("|");
                Write("{0,7}", row.S2);
                Write("|");
                Write("{0,7}", row.S3);
                Write("|");
                Write("{0,4}", row.Laps);
                Write("|");
                WriteLine();
            }
            WriteLine("+-------------------------------------------------------------------------------------------------+");
        }

        private static void WriteRace(SessionModel session)
        {
            WriteLine("+---+---+---+--------------------+-------+-------+-----------+-------+-------+-------+----+");
            WriteLine("| I | P | C | DRIVER             |   GAP |   INT |      TIME |    S1 |    S2 |    S3 |  P |");
            WriteLine("+---+---+---+--------------------+-------+-------+-----------+-------+-------+-------+----+");
            foreach(var row in ((RaceGridModel)session.Grid).Rows.OrderBy(r => r.RowIndex))
            {
                Write("|");
                Write("{0,3}", row.Id);
                Write("|");
                Write("{0,3}", row.Position);
                Write("|");
                Write("{0,3}", row.CarNumber);
                Write("|");
                Write("{0,-20}", row.DriverName);
                Write("|");
                Write("{0,7}", row.Gap);
                Write("|");
                Write("{0,7}", row.Interval);
                Write("|");
                Write("{0,11}", row.LapTime);
                Write("|");
                Write("{0,7}", row.S1);
                Write("|");
                Write("{0,7}", row.S2);
                Write("|");
                Write("{0,7}", row.S3);
                Write("|");
                Write("{0,4}", row.PitCount);
                Write("|");
                WriteLine();
            }
            WriteLine("+-------------------------------------------------------------------------------------------+");
        }

        private static void Write(string s)
        {
            Console.Write(s);
        }

        private static void Write(string format, object arg0)
        {
            Console.Write(format, arg0);
        }

        private static void Write(string format, GridColumnModel column)
        {
            Write(format, column.Text, column.TextColour);
        }

        private static void Write(string format, object arg0, GridColumnColour colour)
        {
            switch(colour)
            {
                case GridColumnColour.White:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case GridColumnColour.Red:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case GridColumnColour.Green:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case GridColumnColour.Magenta:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
                case GridColumnColour.Blue:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case GridColumnColour.Yellow:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case GridColumnColour.Grey:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                default:
                    break;
            }
            Console.Write(format, arg0);
            Console.ResetColor();
        }

        private static void WriteLine()
        {
            Console.WriteLine();
        }

        private static void WriteLine(string format, params object[] args)
        {
            Console.WriteLine(format, args);
        }

        private static void WritePractice(SessionModel session)
        {
            WriteLine("+---+---+---+---------------------+-----------+--------+-------+-------+-------+----+");
            WriteLine("| I | P | C | DRIVER              |      BEST |    GAP |    S1 |    S2 |    S3 |  L |");
            WriteLine("+---+---+---+---------------------+-----------+--------+-------+-------+-------+----+");
            foreach(var row in ((PracticeGridModel)session.Grid).Rows.OrderBy(r => r.RowIndex))
            {
                Write("|");
                Write("{0,3}", row.Id);
                Write("|");
                Write("{0,3}", row.Position);
                Write("|");
                Write("{0,3}", row.CarNumber);
                Write("|");
                Write("{0,-21}", row.DriverName);
                Write("|");
                Write("{0,11}", row.Best);
                Write("|");
                Write("{0,8}", row.Gap);
                Write("|");
                Write("{0,7}", row.S1);
                Write("|");
                Write("{0,7}", row.S2);
                Write("|");
                Write("{0,7}", row.S3);
                Write("|");
                Write("{0,4}", row.Laps);
                Write("|");
                WriteLine();
            }
            WriteLine("+----------------------------------------------------------------------------------+");
        }

        private static void WriteTrackStatistics(SessionModel session)
        {
            WriteLine();
            WriteLine("+------------------------------------------------------------------------------------------+");
            WriteLine("|                                  WEATHER STATISTICS                                      |");
            WriteLine("+------------------+-----------+-----------+-----------+-----------+-----------+-----------+");
            WriteLine("|                  |  Current  |    Avg    |    Min    |    Max    |   Range   |  STD DEV  |");
            WriteLine("+------------------+-----------+-----------+-----------+-----------+-----------+-----------+");
            WriteLine("|Air Temperature   |{0,11}|{1,11}|{2,11}|{3,11}|{4,11}|{5,11}|",
                Format.Temperature(session.Weather.AirTemperature.Current.Value),
                Format.Temperature(session.Weather.AirTemperature.Mean),
                Format.Temperature(session.Weather.AirTemperature.Minimum),
                Format.Temperature(session.Weather.AirTemperature.Maximum),
                Format.Temperature(session.Weather.AirTemperature.Range),
                Format.Temperature(session.Weather.AirTemperature.StandardDeviation));
            WriteLine("|Track Temperature |{0,11}|{1,11}|{2,11}|{3,11}|{4,11}|{5,11}|",
                Format.Temperature(session.Weather.TrackTemperature.Current),
                Format.Temperature(session.Weather.TrackTemperature.Mean),
                Format.Temperature(session.Weather.TrackTemperature.Minimum),
                Format.Temperature(session.Weather.TrackTemperature.Maximum),
                Format.Temperature(session.Weather.TrackTemperature.Range),
                Format.Temperature(session.Weather.TrackTemperature.StandardDeviation));
            WriteLine("|Pressure:         |{0,11}|{1,11}|{2,11}|{3,11}|{4,11}|{5,11}|",
                Format.Pressure(session.Weather.Pressure.Current),
                Format.Pressure(session.Weather.Pressure.Mean),
                Format.Pressure(session.Weather.Pressure.Minimum),
                Format.Pressure(session.Weather.Pressure.Maximum),
                Format.Pressure(session.Weather.Pressure.Range),
                Format.Pressure(session.Weather.Pressure.StandardDeviation));
            WriteLine("|Wind Speed:       |{0,11}|{1,11}|{2,11}|{3,11}|{4,11}|{5,11}|",
                Format.WindSpeed(session.Weather.WindSpeed.Current),
                Format.WindSpeed(session.Weather.WindSpeed.Mean),
                Format.WindSpeed(session.Weather.WindSpeed.Minimum),
                Format.WindSpeed(session.Weather.WindSpeed.Maximum),
                Format.WindSpeed(session.Weather.WindSpeed.Range),
                Format.WindSpeed(session.Weather.WindSpeed.StandardDeviation));
            WriteLine("|Wind Direction:   |{0,11}|{1,11}|{2,11}|{3,11}|{4,11}|{5,11}|",
                Format.WindDirection(session.Weather.WindAngle.Current.Value),
                Format.WindDirection(session.Weather.WindAngle.Mean),
                Format.WindDirection(session.Weather.WindAngle.Minimum),
                Format.WindDirection(session.Weather.WindAngle.Maximum),
                Format.WindDirection(session.Weather.WindAngle.Range),
                Format.WindDirection(session.Weather.WindAngle.StandardDeviation));
            WriteLine("|Humidity:         |{0,11}|{1,11}|{2,11}|{3,11}|{4,11}|{5,11}|",
                Format.Humidity(session.Weather.Humidity.Current),
                Format.Humidity(session.Weather.Humidity.Mean),
                Format.Humidity(session.Weather.Humidity.Minimum),
                Format.Humidity(session.Weather.Humidity.Maximum),
                Format.Humidity(session.Weather.Humidity.Range),
                Format.Humidity(session.Weather.Humidity.StandardDeviation));
            WriteLine("+------------------+---------------------------------------------------------------------+");
        }

        private static void WriteSessionStatistics(SessionModel session)
        {
            WriteLine();
            WriteLine("+---------------------------------------+");
            WriteLine("|          SESSION STATISTICS           |");
            WriteLine("+------------------+--------------------+");
            WriteLine("|Session Type      |{0,20}|", session.SessionType);
            WriteLine("|Session Status    |{0,20}|", session.SessionStatus);
            WriteLine("|Session Time      |{0,20}|", session.ElapsedSessionTime);
            WriteLine("|Rem Session Time  |{0,20}|", session.RemainingSessionTime);
            WriteLine("|Session Laps      |{0,20}|", session.RaceLapNumber);
            WriteLine("|Drivers           |{0,20}|", session.Drivers.Count);
            WriteLine("|Message Count     |{0,20}|", session.Feed.MessageCount);
            WriteLine("+------------------+--------------------+");

            WriteLine("+--------------------------------------------------------------------+");
            WriteLine("|                       BEST LAP AND SECTOR TIMES                    |");
            WriteLine("+--------------------+-----------+-------+-------+-------+-----------+");
            WriteLine("| DRIVER             |      BEST |    S1 |    S2 |    S3 |      POSS |");
            WriteLine("+--------------------+-----------+-------+-------+-------+-----------+");
            foreach(var driver in session.Drivers)
            {
                var times = driver.LapTimes;
                Write("|{0,-20}|", driver.Name);
                Write("{0,11}|", Format.LapTime(times.Laps.Minimum));
                Write("{0,7}|", Format.SectorTime(times.S1.Minimum));
                Write("{0,7}|", Format.SectorTime(times.S2.Minimum));
                Write("{0,7}|", Format.SectorTime(times.S3.Minimum));
                TimeSpan? possible = null;
                if(times.S1.Minimum != null && times.S2.Minimum != null && times.S3.Minimum != null)
                {
                    possible = times.S1.Minimum.Time + times.S2.Minimum.Time + times.S3.Minimum.Time;
                }
                Write("{0,11}|", Format.LapTime(possible));
                WriteLine();
            }
            WriteLine("+--------------------------------------------------------------------+");

            WriteLine();
            WriteLine("+---------------------------------------------+");
            WriteLine("|   TOTAL SECTOR / LAP TIME / PIT COUNTS      |");
            WriteLine("+--------------------+----+----+----+----+----+");
            WriteLine("| DRIVER             | S1 | S2 | S3 |  L |  P |");
            WriteLine("+--------------------+----+----+----+----+----+");
            foreach(var driver in session.Drivers)
            {
                Write("|{0,-20}|", driver.Name);
                Write("{0,4}|", driver.LapTimes.S1.Items.Count);
                Write("{0,4}|", driver.LapTimes.S2.Items.Count);
                Write("{0,4}|", driver.LapTimes.S3.Items.Count);
                Write("{0,4}|", driver.LapTimes.Laps.Items.Count);
                WriteLine("{0,4}|", driver.PitTimes.Count);
            }
            WriteLine("+--------------------+----+----+----+----+----+");
        }
    }
}