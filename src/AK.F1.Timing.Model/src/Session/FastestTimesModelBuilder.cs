// Copyright 2010 Andy Kernahan
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
    public partial class FastestTimesModel
    {
        /// <summary>
        /// A <see cref="AK.F1.Timing.IMessageProcessor"/> which builds a
        /// <see cref="AK.F1.Timing.Model.Session.MessageModel"/> as it processes
        /// <see cref="AK.F1.Timing.Message"/>s. This class cannot be inherited.
        /// </summary>
        [Serializable]
        private sealed class FastestTimesModelBuilder : MessageVisitorBase, IMessageProcessor
        {
            #region Public Interface.

            /// <summary>
            /// Initialises a new instance of the <see cref="FastestTimesModelBuilder"/> class and specifies
            /// the <paramref name="model"/> to build.
            /// </summary>
            /// <param name="model">The model to build.</param>
            /// <exception cref="System.ArgumentNullException">
            /// Thrown when <paramref name="model"/> is <see langword="null"/>.
            /// </exception>
            public FastestTimesModelBuilder(FastestTimesModel model) {

                Guard.NotNull(model, "model");

                Model = model;
                CurrentSessionType = SessionType.None;
            }

            /// <inheritdoc/>
            public void Process(Message message) {

                message.Accept(this);
            }

            /// <inheritdoc />
            public override void Visit(SetDriverSectorTimeMessage message) {
                
                TrySetFastestSector(message.SectorNumber, message.SectorTime, message.DriverId);
            }

            /// <inheritdoc />
            public override void Visit(ReplaceDriverSectorTimeMessage message) {

                TrySetFastestSector(message.SectorNumber, message.Replacement, message.DriverId);
            }

            /// <inheritdoc />
            public override void Visit(SetDriverQuallyTimeMessage message) {

                var driver = GetDriver(message.DriverId);
                var postedTime = new PostedTime(message.QuallyTime, PostedTimeType.Normal, driver.LapsCompleted);

                TrySetFastestLap(postedTime, message.DriverId);
            }

            /// <inheritdoc />
            public override void Visit(SetDriverLapTimeMessage message) {

                TrySetFastestLap(message.LapTime, message.DriverId);
            }

            /// <inheritdoc />
            public override void Visit(ReplaceDriverLapTimeMessage message) {

                TrySetFastestLap(message.Replacement, message.DriverId);
            }

            /// <inheritdoc />
            public override void Visit(SetSessionTypeMessage message) {

                CurrentSessionType = message.SessionType;
            }

            #endregion

            #region Private Impl.

            private void TrySetFastestLap(PostedTime time, int driverId) {                
                
                var isSessionBest = time.Type == PostedTimeType.SessionBest ||
                    // We only receive session best lap times during a race session so we determine here
                    // if the specified time should be promoted.
                    (CurrentSessionType != SessionType.Race && (Model.Lap == null || time.Time < Model.Lap.Time));

                if(isSessionBest) {
                    Model.SetLap(GetDriver(driverId), time.Time, time.LapNumber);
                }
            }

            private void TrySetFastestSector(int sectorNumber, PostedTime time, int driverId) {

                if(time.Type == PostedTimeType.SessionBest) {
                    Model.SetSector(sectorNumber, GetDriver(driverId), time.Time, time.LapNumber);
                }
            }

            private DriverModel GetDriver(int id) {

                return Model.DriverLocator.GetDriver(id);
            }

            private SessionType CurrentSessionType { get; set; }

            private FastestTimesModel Model { get; set; }

            #endregion
        }
    }
}