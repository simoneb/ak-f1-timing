// Copyright 2011 Andy Kernahan
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
using System.Linq;
using System.Xml.Linq;
using DDay.iCal;

namespace AK.F1.Timing.Service.Utility
{
    /// <summary>
    /// Converts http://www.f1calendar.com/ ICS files to the race XML document schema as expected by the service. This
    /// class is <see langword="static"/>.
    /// </summary>
    internal static class IcsConverter
    {
        #region Public Interface.

        /// <summary>
        /// Converts the specified http://www.f1calendar.com/ ICS file to a race XML document schema expected by the
        /// service.
        /// </summary>
        /// <param name="path">The path of the ICS file.</param>
        /// <param name="startTimeOffset">The start time offset (in minutes).</param>
        /// <returns>The <see cref="System.Xml.Linq.XDocument"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="path"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <paramref name="path"/> is of zero length.
        /// </exception>
        public static XDocument ToXDocument(string path, int startTimeOffset = -2)
        {
            Guard.NotNullOrEmpty(path, "path");

            int i = 0;
            var calendar = iCalendar.LoadFromFile(path);
            return new XDocument(
                new XElement("races",
                    from e in calendar.First().Events
                    group e by GetRaceId(e) into race
                    orderby race.Min(session => session.Start)
                    select new XElement("race",
                        new XAttribute("id", String.Format(CultureInfo.InvariantCulture, "{0:00}-{1}", ++i, race.Key)),
                        new XAttribute("name", GetRaceName(race.First())),
                        new XAttribute("location", race.First().Location),
                        from session in race
                        orderby session.Start
                        select new XElement("session",
                            new XAttribute("id", GetSessionId(session)),
                            new XAttribute("name", GetSessionName(session)),
                            new XAttribute("startTimeUtc", session.Start.AddMinutes(startTimeOffset).ToString("o", CultureInfo.InvariantCulture))))));
        }

        #endregion

        #region Private Impl.

        private static string GetRaceId(IEvent e)
        {
            switch (e.Location)
            {
                case "Melbourne":
                    return "australia";
                case "Kuala Lumpur":
                    return "malaysia";
                case "Shanghai":
                    return "china";
                case "Sakhir":
                    return "bahrain";
                case "Catalunya":
                    return "spain";
                case "Monte Carlo":
                    return "monaco";
                case "Montreal":
                    return "canada";
                case "Valencia, Spain":
                    return "europe";
                case "Silverstone, England":
                    return "great-britain";
                case "Hockenheim":
                    return "german";
                case "Budapest":
                    return "hungary";
                case "Spa-Francorchamps":
                    return "belguim";
                case "Monza":
                    return "italy";
                case "Singapore":
                    return "singapore";
                case "Suzuka":
                    return "japan";
                case "Yeongam":
                    return "korea";
                case "New Delhi":
                    return "india";
                case "Yas Marina":
                    return "abu-dhabi";
                case "Austin":
                    return "usa";
                case "Sao Paulo":
                    return "brazil";
                default:
                    throw new FormatException("Failed to parse a location from '" + e.Location + "'.");
            }
        }

        private static string GetRaceName(IEvent e)
        {
            var s = e.Summary;
            var i = s.IndexOf('(') + 1;
            return s.Substring(i, s.Length - i - 1);
        }

        private static string GetSessionId(IEvent e)
        {
            switch (GetSessionType(e))
            {
                case SessionType.P1:
                    return "practice1";
                case SessionType.P2:
                    return "practice2";
                case SessionType.P3:
                    return "practice3";
                case SessionType.Qually:
                    return "qually";
                case SessionType.Race:
                    return "race";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static string GetSessionName(IEvent e)
        {
            switch (GetSessionType(e))
            {
                case SessionType.P1:
                    return "Practice 1";
                case SessionType.P2:
                    return "Practice 2";
                case SessionType.P3:
                    return "Practice 3";
                case SessionType.Qually:
                    return "Qualification";
                case SessionType.Race:
                    return "Race";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static SessionType GetSessionType(IEvent e)
        {
            var s = e.Summary;
            if (s.StartsWith("First"))
            {
                return SessionType.P1;
            }
            if (s.StartsWith("Second"))
            {
                return SessionType.P2;
            }
            if (s.StartsWith("Third"))
            {
                return SessionType.P3;
            }
            if (s.StartsWith("Qualifying"))
            {
                return SessionType.Qually;
            }
            if (s.EndsWith("Prix"))
            {
                return SessionType.Race;
            }
            throw new FormatException("Failed to parse the session type from '" + s + "'.");
        }

        private enum SessionType
        {
            P1,
            P2,
            P3,
            Qually,
            Race
        }

        #endregion
    }
}
