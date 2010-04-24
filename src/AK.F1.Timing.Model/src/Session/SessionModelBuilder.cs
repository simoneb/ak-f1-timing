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
using AK.F1.Timing.Messages.Session;
using AK.F1.Timing.Model.Driver;
using AK.F1.Timing.Model.Grid;

namespace AK.F1.Timing.Model.Session
{
    /// <summary>
    /// A <see cref="AK.F1.Timing.IMessageProcessor"/> which builds a
    /// <see cref="AK.F1.Timing.Model.Session.SessionModel"/> as it processes
    /// <see cref="AK.F1.Timing.Message"/>s. This class cannot be inherited.
    /// </summary>
    public class SessionModelBuilder : MessageVisitorBase, IMessageProcessor
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="SessionModelBuilder"/> class and specifies
        /// the <paramref name="session"/> to build.
        /// </summary>
        /// <param name="session">The session to build.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Throw when <paramref name="session"/> is <see langword="null"/>.
        /// </exception>
        public SessionModelBuilder(SessionModel session) {

            Guard.NotNull(session, "session");

            Session = session;
        }

        /// <inheritdoc/>        
        public void Process(Message message) {

            Guard.NotNull(message, "message");
            
            message.Accept(this);
            Session.Feed.Process(message);
            Session.FastestTimes.Process(message);
            Session.Grid.Process(message);
            Session.Messages.Process(message);
            Session.Weather.Process(message);            
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
        }

        /// <inheritdoc />
        public override void Visit(ReplaceDriverSectorTimeMessage message) {

            var driver = GetDriver(message.DriverId);
            
            driver.LapTimes.GetSector(message.SectorNumber).ReplaceCurrent(message.Replacement);
        }

        /// <inheritdoc />
        public override void Visit(SetDriverQuallyTimeMessage message) {

            var driver = GetDriver(message.DriverId);
            var postedTime = new PostedTime(message.QuallyTime, PostedTimeType.Normal, driver.LapsCompleted);
            
            driver.QuallyTimes.Set(message.QuallyNumber, message.QuallyTime);
            driver.LapTimes.Laps.Add(postedTime);
        }

        /// <inheritdoc />
        public override void Visit(SetDriverLapTimeMessage message) {

            var driver = GetDriver(message.DriverId);

            driver.LapTimes.Laps.Add(message.LapTime);
        }

        /// <inheritdoc />
        public override void Visit(ReplaceDriverLapTimeMessage message) {

            var driver = GetDriver(message.DriverId);

            driver.LapTimes.Laps.ReplaceCurrent(message.Replacement);
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
        public override void Visit(StartSessionTimeCountdownMessage message) {

            Session.DecrementRemainingSessionTime = true;
        }

        /// <inheritdoc />
        public override void Visit(StopSessionTimeCountdownMessage message) {

            Session.DecrementRemainingSessionTime = false;
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


        #endregion
    }
}
