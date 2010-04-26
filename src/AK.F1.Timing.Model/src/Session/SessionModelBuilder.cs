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

namespace AK.F1.Timing.Model.Session
{
    public partial class SessionModel
    {
        /// <summary>
        /// An <see cref="AK.F1.Timing.IMessageProcessor"/> which builds a
        /// <see cref="AK.F1.Timing.Model.Session.SessionModel"/> as it processes
        /// <see cref="AK.F1.Timing.Message"/>s. This class cannot be inherited.
        /// </summary>
        private sealed class SessionModelBuilder : MessageVisitorBase, IMessageProcessor
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

                Model = session;
            }

            /// <inheritdoc/>        
            public void Process(Message message) {

                Guard.NotNull(message, "message");

                message.Accept(this);
                Model.Feed.Process(message);
                Model.FastestTimes.Process(message);
                Model.Grid.Process(message);
                Model.Messages.Process(message);
                Model.Weather.Process(message);
            }

            /// <inheritdoc />
            public override void Visit(SetDriverCarNumberMessage message) {

                GetDriver(message).CarNumber = message.CarNumber;
            }

            /// <inheritdoc />
            public override void Visit(SetDriverNameMessage message) {

                GetDriver(message).Name = message.DriverName;
            }

            /// <inheritdoc />
            public override void Visit(SetDriverPositionMessage message) {

                var driver = GetDriver(message);

                if(driver.Position != message.Position) {
                    driver.Position = message.Position;
                    Model.SortDrivers();
                }
            }

            /// <inheritdoc />
            public override void Visit(SetDriverSectorTimeMessage message) {

                GetDriver(message).LapTimes.GetSector(message.SectorNumber).Add(message.SectorTime);
            }

            /// <inheritdoc />
            public override void Visit(ReplaceDriverSectorTimeMessage message) {

                GetDriver(message).LapTimes.GetSector(message.SectorNumber).ReplaceCurrent(message.Replacement);
            }

            /// <inheritdoc />
            public override void Visit(SetDriverQuallyTimeMessage message) {

                var driver = GetDriver(message);
                var postedTime = new PostedTime(message.QuallyTime, PostedTimeType.Normal, driver.LapsCompleted);

                driver.QuallyTimes.Set(message.QuallyNumber, message.QuallyTime);
                driver.LapTimes.Laps.Add(postedTime);
            }

            /// <inheritdoc />
            public override void Visit(SetDriverLapTimeMessage message) {

                GetDriver(message).LapTimes.Laps.Add(message.LapTime);
            }

            /// <inheritdoc />
            public override void Visit(ReplaceDriverLapTimeMessage message) {

                GetDriver(message).LapTimes.Laps.ReplaceCurrent(message.Replacement);
            }

            /// <inheritdoc />
            public override void Visit(SetDriverCompletedLapsMessage message) {

                GetDriver(message).LapsCompleted = message.CompletedLaps;
            }

            /// <inheritdoc />
            public override void Visit(SetDriverGapMessage message) {

                GetDriver(message).Gap = message.Gap;
            }

            /// <inheritdoc />
            public override void Visit(SetDriverIntervalMessage message) {

                GetDriver(message).Interval = message.Interval;
            }

            /// <inheritdoc />
            public override void Visit(SetSessionTypeMessage message) {                

                Model.ChangeSessionType(message.SessionType);
            }

            /// <inheritdoc />
            public override void Visit(SetSessionStatusMessage message) {

                Model.SessionStatus = message.SessionStatus;
            }

            /// <inheritdoc />
            public override void Visit(SetDriverStatusMessage message) {

                GetDriver(message).Status = message.DriverStatus;
            }

            /// <inheritdoc />
            public override void Visit(SetDriverPitCountMessage message) {

                GetDriver(message).PitCount = message.PitCount;
            }

            /// <inheritdoc />
            public override void Visit(SetDriverPitTimeMessage message) {

                GetDriver(message).PitTimes.Add(new PitTimeModel(message.Time, message.LapNumber));
            }

            /// <inheritdoc />
            public override void Visit(SetElapsedSessionTimeMessage message) {

                Model.UpdateElapsedSessionTime(message.Elapsed);
            }

            /// <inheritdoc />
            public override void Visit(SetRemainingSessionTimeMessage message) {

                Model.RemainingSessionTime = message.Remaining;
            }

            /// <inheritdoc />
            public override void Visit(SetRaceLapNumberMessage message) {

                Model.RaceLapNumber = message.LapNumber;
            }

            /// <inheritdoc />
            public override void Visit(StartSessionTimeCountdownMessage message) {

                Model.OnSessionTimeCountDownStarted();                
            }

            /// <inheritdoc />
            public override void Visit(StopSessionTimeCountdownMessage message) {

                Model.OnSessionTimeCountDownStopped();
            }

            /// <inheritdoc />
            public override void Visit(EndOfSessionMessage message) {

                Model.OnSessionEnded();                
            }

            #endregion

            #region Private Impl.

            private DriverModel GetDriver(DriverMessageBase message) {

                return Model.GetDriver(message.DriverId);
            }

            private SessionModel Model { get; set; }

            #endregion
        }
    }
}
