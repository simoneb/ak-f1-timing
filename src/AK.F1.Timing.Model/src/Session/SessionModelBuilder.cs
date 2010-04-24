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

using AK.F1.Timing.Messages.Driver;
using AK.F1.Timing.Messages.Feed;
using AK.F1.Timing.Messages.Session;
using AK.F1.Timing.Messages.Weather;
using AK.F1.Timing.Model.Driver;
using AK.F1.Timing.Model.Grid;

namespace AK.F1.Timing.Model.Session
{
    /// <summary>
    /// A builder which builds a <see cref="AK.F1.Timing.Model.Session.SessionModel"/> as it
    /// processes <see cref="AK.F1.Timing.Message"/>s.
    /// </summary>
    public class SessionModelBuilder : MessageVisitorBase, IMessageProcessor
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="SessionModelBuilder"/> class and specifies
        /// the session to build.
        /// </summary>
        /// <param name="session">The session to build.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Throw when <paramref name="session"/> is <see langword="null"/>.
        /// </exception>
        public SessionModelBuilder(SessionModel session) {

            Guard.NotNull(session, "session");

            Session = session;
        }

        /// <summary>
        /// Processes the specified message.
        /// </summary>
        /// <param name="message">The message to process.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Throw when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        public void Process(Message message) {

            message.Accept(this);
            Session.Grid.Process(message);
            ++Session.Feed.MessageCount;
        }

        /// <inheritdoc />
        public override void Visit(SetDriverCarNumberMessage message) {

            GetDriver(message.DriverId).CarNumber = message.CarNumber;
        }

        /// <inheritdoc />
        public override void Visit(SetDriverNameMessage message) {

            GetDriver(message.DriverId).Name = message.DriverName;
        }

        /// <inheritdoc />
        public override void Visit(SetDriverPositionMessage message) {

            var driver = GetDriver(message.DriverId);

            if(driver.Position != message.Position) {
                driver.Position = message.Position;
                Session.InnerDrivers.Sort();
            }
        }

        /// <inheritdoc />
        public override void Visit(SetDriverSectorTimeMessage message) {

            var driver = GetDriver(message.DriverId);            

            driver.LapTimes.GetSector(message.SectorNumber).Add(message.SectorTime);
            TrySetFastestSector(message.SectorNumber, message.SectorTime, driver);
        }

        /// <inheritdoc />
        public override void Visit(ReplaceDriverSectorTimeMessage message) {

            var driver = GetDriver(message.DriverId);
            
            driver.LapTimes.GetSector(message.SectorNumber).ReplaceCurrent(message.Replacement);
            TrySetFastestSector(message.SectorNumber, message.Replacement, driver);
        }

        /// <inheritdoc />
        public override void Visit(SetDriverQuallyTimeMessage message) {

            var driver = GetDriver(message.DriverId);
            var postedTime = new PostedTime(message.QuallyTime, PostedTimeType.Normal, driver.LapsCompleted);
            
            driver.QuallyTimes.Set(message.QuallyNumber, message.QuallyTime);
            driver.LapTimes.Laps.Add(postedTime);
            TrySetFastestLap(postedTime, driver);
        }

        /// <inheritdoc />
        public override void Visit(SetDriverLapTimeMessage message) {

            var driver = GetDriver(message.DriverId);

            driver.LapTimes.Laps.Add(message.LapTime);
            TrySetFastestLap(message.LapTime, driver);
        }

        /// <inheritdoc />
        public override void Visit(ReplaceDriverLapTimeMessage message) {

            var driver = GetDriver(message.DriverId);

            driver.LapTimes.Laps.ReplaceCurrent(message.Replacement);
            TrySetFastestLap(message.Replacement, driver);
        }

        /// <inheritdoc />
        public override void Visit(SetDriverCompletedLapsMessage message) {

            GetDriver(message.DriverId).LapsCompleted = message.CompletedLaps;
        }

        /// <inheritdoc />
        public override void Visit(SetDriverGapMessage message) {

            GetDriver(message.DriverId).Gap = message.Gap;
        }

        /// <inheritdoc />
        public override void Visit(SetDriverIntervalMessage message) {

            GetDriver(message.DriverId).Interval = message.Interval;
        }

        /// <inheritdoc />
        public override void Visit(SetSessionTypeMessage message) {

            if(Session.SessionType != message.SessionType) {
                Session.Reset();
                Session.SessionType = message.SessionType;
                Session.Grid = GridModelBase.Create(message.SessionType);
            }
        }

        /// <inheritdoc />
        public override void Visit(SetSessionStatusMessage message) {

            Session.SessionStatus = message.SessionStatus;
        }

        /// <inheritdoc />
        public override void Visit(SetDriverStatusMessage message) {

            GetDriver(message.DriverId).Status = message.DriverStatus;
        }

        /// <inheritdoc />
        public override void Visit(SetAirTemperatureMessage message) {

            Session.Weather.AirTemperature.Add(message.Temperature);
        }

        /// <inheritdoc />
        public override void Visit(SetTrackTemperatureMessage message) {

            Session.Weather.TrackTemperature.Add(message.Temperature);
        }

        /// <inheritdoc />
        public override void Visit(SetWindSpeedMessage message) {

            Session.Weather.WindSpeed.Add(message.Speed);
        }

        /// <inheritdoc />
        public override void Visit(SetAtmosphericPressureMessage message) {

            Session.Weather.Pressure.Add(message.Pressure);
        }

        /// <inheritdoc />
        public override void Visit(SetHumidityMessage message) {

            Session.Weather.Humidity.Add(message.Humidity);
        }

        /// <inheritdoc />
        public override void Visit(SetWindAngleMessage message) {

            Session.Weather.WindAngle.Add(message.Angle);
        }

        /// <inheritdoc />
        public override void Visit(SetDriverPitCountMessage message) {

            GetDriver(message.DriverId).PitCount = message.PitCount;
        }

        /// <inheritdoc />
        public override void Visit(SetDriverPitTimeMessage message) {

            GetDriver(message.DriverId).PitTimes.Add(new PitTimeModel(message.Time, message.LapNumber));
        }

        /// <inheritdoc />
        public override void Visit(SetElapsedSessionTimeMessage message) {

            Session.ElapsedSessionTime = message.Elapsed;
            if(message.Elapsed > TimeSpan.Zero) {
                Session.OneSecondTimer.Start();
            } else {
                Session.OneSecondTimer.Stop();
            }
        }

        /// <inheritdoc />
        public override void Visit(SetRemainingSessionTimeMessage message) {

            Session.RemainingSessionTime = message.Remaining;
        }

        /// <inheritdoc />
        public override void Visit(SetRaceLapNumberMessage message) {

            Session.RaceLapNumber = message.LapNumber;
        }

        /// <inheritdoc />
        public override void Visit(AddCommentaryMessage message) {

            Session.Messages.AddCommentary(message.Commentary);
        }

        /// <inheritdoc />
        public override void Visit(SetSystemMessageMessage message) {

            Session.Messages.System = message.Message;                
        }

        /// <inheritdoc />
        public override void Visit(SetPingIntervalMessage message) {

            Session.Feed.PingInterval = message.PingInterval;
        }

        /// <inheritdoc />
        public override void Visit(SetKeyframeMessage message) {

            Session.Feed.KeyframeNumber = message.Keyframe;
        }

        /// <inheritdoc />
        public override void Visit(SetCopyrightMessage message) {

            Session.Feed.Copyright = message.Copyright;
        }

        /// <inheritdoc />
        public override void Visit(StartSessionTimeCountdownMessage message) {

            Session.DecrementRemainingSessionTime = true;
        }

        /// <inheritdoc />
        public override void Visit(StopSessionTimeCountdownMessage message) {

            Session.DecrementRemainingSessionTime = false;
        }

        /// <inheritdoc />
        public override void Visit(SetIsWetMessage message) {

            Session.Weather.IsWet = message.IsWet;
        }

        /// <inheritdoc />
        public override void Visit(EndOfSessionMessage message) {

            Session.OneSecondTimer.Stop();
        }

        #endregion

        #region Public Interface.

        /// <summary>
        /// Gets the driver with the specified Id from the session.
        /// </summary>
        /// <param name="driverId">The Id of the driver to get.</param>
        /// <returns>The driver with the specified Id from the session.</returns>
        protected DriverModel GetDriver(int driverId) {

            return Session.GetDriver(driverId);
        }

        /// <summary>
        /// Gets the session being built.
        /// </summary>
        protected SessionModel Session { get; private set; }

        #endregion

        #region Private Impl.

        private void TrySetFastestLap(PostedTime time, DriverModel driver) {

            // TODO does this belong here?

            var times = Session.FastestTimes;
            var isSessionBest = time.Type == PostedTimeType.SessionBest ||
                (Session.SessionType != SessionType.Race && (times.Lap == null || time.Time < times.Lap.Time));

            if(isSessionBest) {
                times.SetLap(driver, time.Time, time.LapNumber);
            }
        }

        private void TrySetFastestSector(int sectorNumber, PostedTime time, DriverModel driver) {

            if(time.Type == PostedTimeType.SessionBest) {
                Session.FastestTimes.SetSector(sectorNumber, driver, time.Time, time.LapNumber);
            }
        }

        #endregion
    }
}
