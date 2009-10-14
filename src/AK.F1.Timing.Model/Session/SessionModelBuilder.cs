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

using AK.F1.Timing.Messaging;
using AK.F1.Timing.Messaging.Messages.Driver;
using AK.F1.Timing.Messaging.Messages.Feed;
using AK.F1.Timing.Messaging.Messages.Session;
using AK.F1.Timing.Messaging.Messages.Weather;
using AK.F1.Timing.Model.Collections;
using AK.F1.Timing.Model.Driver;
using AK.F1.Timing.Model.Grid;

namespace AK.F1.Timing.Model.Session
{
    /// <summary>
    /// A builder which builds a <see cref="AK.F1.Timing.Model.Session.SessionModel"/> as it
    /// processes <see cref="AK.F1.Timing.Messaging.Message"/>s.
    /// </summary>
    public class SessionModelBuilder : MessageVisitor, IMessageProcessor
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

            this.Session = session;
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
            this.Session.Grid.Process(message);
            ++this.Session.Feed.MessageCount;
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
                this.Session.Drivers.Sort();
            }
        }

        /// <inheritdoc />
        public override void Visit(SetDriverSectorTimeMessage message) {

            var driver = GetDriver(message.DriverId);            

            driver.SectorTimes.Get(message.SectorNumber).Values.Add(message.SectorTime);
            TrySetFastestSector(message.SectorNumber, message.SectorTime, driver);
        }

        /// <inheritdoc />
        public override void Visit(ReplaceDriverSectorTimeMessage message) {

            var driver = GetDriver(message.DriverId);
            var values = driver.SectorTimes.Get(message.SectorNumber).Values;

            values[values.Count - 1] = message.Replacement;
            TrySetFastestSector(message.SectorNumber, message.Replacement, driver);
        }

        /// <inheritdoc />
        public override void Visit(SetDriverQuallyTimeMessage message) {

            var driver = GetDriver(message.DriverId);

            driver.QuallyTimes.Set(message.QuallyNumber, message.QuallyTime);
            driver.LapTimes.Values.Add(new PostedTime(driver.LapsCompleted,
                message.QuallyTime, PostedTimeType.Normal));
        }

        /// <inheritdoc />
        public override void Visit(SetDriverLapTimeMessage message) {

            var driver = GetDriver(message.DriverId);

            driver.LapTimes.Values.Add(message.LapTime);            
            TrySetFastestLap(driver, message.LapTime);
        }

        /// <inheritdoc />
        public override void Visit(ReplaceDriverLapTimeMessage message) {

            var driver = GetDriver(message.DriverId);
            var times = driver.LapTimes.Values;

            times[times.Count - 1] = message.Replacement;
            TrySetFastestLap(driver, message.Replacement);
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

            if(this.Session.SessionType != message.SessionType) {
                this.Session.Reset();
                this.Session.SessionType = message.SessionType;
                this.Session.Grid = GridModelBase.Create(message.SessionType);
            }
        }

        /// <inheritdoc />
        public override void Visit(SetSessionStatusMessage message) {

            this.Session.SessionStatus = message.SessionStatus;
        }

        /// <inheritdoc />
        public override void Visit(SetDriverStatusMessage message) {

            GetDriver(message.DriverId).Status = message.DriverStatus;
        }

        /// <inheritdoc />
        public override void Visit(SetAirTemperatureMessage message) {

            this.Session.Weather.AirTemperature.Values.Add(message.Temperature);
        }

        /// <inheritdoc />
        public override void Visit(SetTrackTemperatureMessage message) {

            this.Session.Weather.TrackTemperature.Values.Add(message.Temperature);
        }

        /// <inheritdoc />
        public override void Visit(SetWindSpeedMessage message) {

            this.Session.Weather.WindSpeed.Values.Add(message.Speed);
        }

        /// <inheritdoc />
        public override void Visit(SetAtmosphericPressureMessage message) {

            this.Session.Weather.Pressure.Values.Add(message.Pressure);
        }

        /// <inheritdoc />
        public override void Visit(SetHumidityMessage message) {

            this.Session.Weather.Humidity.Values.Add(message.Humidity);
        }

        /// <inheritdoc />
        public override void Visit(SetWindAngleMessage message) {

            this.Session.Weather.WindAngle.Values.Add(message.Angle);
        }

        /// <inheritdoc />
        public override void Visit(SetDriverPitCountMessage message) {

            GetDriver(message.DriverId).PitCount = message.PitCount;
        }

        /// <inheritdoc />
        public override void Visit(SetElapsedSessionTimeMessage message) {

            this.Session.ElapsedSessionTime = message.Elapsed;
            if(message.Elapsed > TimeSpan.Zero) {
                this.Session.OneSecondTimer.Start();
            } else {
                this.Session.OneSecondTimer.Stop();
            }
        }

        /// <inheritdoc />
        public override void Visit(SetRemainingSessionTimeMessage message) {

            this.Session.RemainingSessionTime = message.Remaining;
        }

        /// <inheritdoc />
        public override void Visit(SetRaceLapNumberMessage message) {

            this.Session.RaceLapNumber = message.LapNumber;
        }

        /// <inheritdoc />
        public override void Visit(AddCommentaryMessage message) {

            this.Session.Messages.AddCommentary(message.Commentary);
        }

        /// <inheritdoc />
        public override void Visit(SetSystemMessageMessage message) {

            this.Session.Messages.System = message.Message;                
        }

        /// <inheritdoc />
        public override void Visit(SetPingIntervalMessage message) {

            this.Session.Feed.PingInterval = message.PingInterval;
        }

        /// <inheritdoc />
        public override void Visit(SetKeyframeMessage message) {

            this.Session.Feed.KeyframeNumber = message.Keyframe;
        }

        /// <inheritdoc />
        public override void Visit(SetCopyrightMessage message) {

            this.Session.Feed.Copyright = message.Copyright;
        }

        /// <inheritdoc />
        public override void Visit(StartSessionTimeCountdownMessage message) {

            this.Session.DecrementRemainingSessionTime = true;
        }

        /// <inheritdoc />
        public override void Visit(StopSessionTimeCountdownMessage message) {

            this.Session.DecrementRemainingSessionTime = false;
        }

        /// <inheritdoc />
        public override void Visit(SetWetDryMessage message) {

            this.Session.Weather.WetDry = message.WetDry;
        }

        /// <inheritdoc />
        public override void Visit(EndOfSessionMessage message) {

            this.Session.OneSecondTimer.Stop();
        }

        #endregion

        #region Public Interface.

        /// <summary>
        /// Gets the driver with the specified Id from the session.
        /// </summary>
        /// <param name="driverId">The Id of the driver to get.</param>
        /// <returns>The driver with the specified Id from the session.</returns>
        protected DriverModel GetDriver(int driverId) {

            return this.Session.GetDriver(driverId);
        }

        /// <summary>
        /// Gets the session being built.
        /// </summary>
        protected SessionModel Session { get; private set; }

        #endregion

        #region Private Impl.

        private void TrySetFastestLap(DriverModel driver, PostedTime time) {

            if(time.Type == PostedTimeType.SessionBest) {
                this.Session.FastestTimes.Lap = new FastestTimeModel(driver, time.Time, time.Lap);
            }
        }

        private void TrySetFastestSector(int sectorNumber, PostedTime time, DriverModel driver) {

            if(time.Type == PostedTimeType.SessionBest) {
                this.Session.FastestTimes.SetSector(sectorNumber,
                    new FastestTimeModel(driver, time.Time, time.Lap));
            }
        }

        #endregion
    }
}