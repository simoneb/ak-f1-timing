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

using AK.F1.Timing.Messages.Session;
using DriverMessageBase = AK.F1.Timing.Messages.Driver.DriverMessageBase;

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
                TryDispatchToDriver(message);
                Model.Feed.Process(message);
                Model.FastestTimes.Process(message);
                Model.Grid.Process(message);
                Model.Messages.Process(message);
                Model.Weather.Process(message);
            }

            /// <inheritdoc />
            public override void Visit(AK.F1.Timing.Messages.Driver.SetDriverPositionMessage message) {

                // TODO

                //var driver = GetDriver(message);

                //if(driver.Position != message.Position) {
                //    driver.Position = message.Position;
                //    Model.SortDrivers();
                //}
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

            private void TryDispatchToDriver(Message message) {

                var driverMessage = message as DriverMessageBase;

                if(driverMessage != null) {
                    Model.GetDriver(driverMessage.DriverId).Process(message);
                }
            }

            private SessionModel Model { get; set; }

            #endregion
        }
    }
}
