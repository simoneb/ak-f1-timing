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

namespace AK.F1.Timing.Model.Driver
{
    public partial class DriverModel
    {
        /// <summary>
        /// An <see cref="AK.F1.Timing.IMessageProcessor"/> which builds a
        /// <see cref="AK.F1.Timing.Model.Session.MessageModel"/> as it processes
        /// <see cref="AK.F1.Timing.Message"/>s. This class cannot be inherited.
        /// </summary>
        [Serializable]
        private sealed class DriverModelBuilder : MessageVisitorBase, IMessageProcessor
        {
            #region Public Interface.

            /// <summary>
            /// Initialises a new instance of the <see cref="DriverModelBuilder"/> class and specifies
            /// the <paramref name="model"/> to build.
            /// </summary>
            /// <param name="model">The model to build.</param>
            /// <exception cref="System.ArgumentNullException">
            /// Thrown when <paramref name="model"/> is <see langword="null"/>.
            /// </exception>
            public DriverModelBuilder(DriverModel model)
            {
                Guard.NotNull(model, "model");

                Model = model;
            }

            /// <inheritdoc/>
            public void Process(Message message)
            {
                message.Accept(this);
            }

            /// <inheritdoc/>
            public override void Visit(SetDriverCarNumberMessage message)
            {
                if(IsForMyDriver(message))
                {
                    Model.CarNumber = message.CarNumber;
                }
            }

            /// <inheritdoc/>
            public override void Visit(SetDriverGapMessage message)
            {
                if(IsForMyDriver(message))
                {
                    Model.Gap = message.Gap;
                }
            }

            /// <inheritdoc/>
            public override void Visit(SetDriverIntervalMessage message)
            {
                if(IsForMyDriver(message))
                {
                    Model.Interval = message.Interval;
                }
            }

            /// <inheritdoc/>
            public override void Visit(SetDriverLapNumberMessage message)
            {
                if(IsForMyDriver(message))
                {
                    Model.LapsCompleted = message.LapNumber;
                }
            }

            /// <inheritdoc/>
            public override void Visit(SetDriverNameMessage message)
            {
                if(IsForMyDriver(message))
                {
                    Model.Name = message.DriverName;
                }
            }

            /// <inheritdoc/>
            public override void Visit(SetDriverLapTimeMessage message)
            {
                if(IsForMyDriver(message))
                {
                    Model.LapTimes.Laps.Add(message.LapTime);
                }
            }

            /// <inheritdoc/>
            public override void Visit(ReplaceDriverLapTimeMessage message)
            {
                if(IsForMyDriver(message))
                {
                    Model.LapTimes.Laps.ReplaceCurrent(message.Replacement);
                }
            }

            /// <inheritdoc/>
            public override void Visit(SetDriverPitTimeMessage message)
            {
                if(IsForMyDriver(message))
                {
                    Model.PitTimes.Add(message.PitTime);
                }
            }

            /// <inheritdoc/>
            public override void Visit(SetDriverPositionMessage message)
            {
                if(IsForMyDriver(message))
                {
                    Model.Position = message.Position;
                }
            }

            /// <inheritdoc/>
            public override void Visit(SetDriverSectorTimeMessage message)
            {
                if(IsForMyDriver(message))
                {
                    Model.LapTimes.GetSector(message.SectorNumber).Add(message.SectorTime);
                }
            }

            /// <inheritdoc/>
            public override void Visit(ReplaceDriverSectorTimeMessage message)
            {
                if(IsForMyDriver(message))
                {
                    Model.LapTimes.GetSector(message.SectorNumber).ReplaceCurrent(message.Replacement);
                }
            }

            /// <inheritdoc/>
            public override void Visit(SetDriverStatusMessage message)
            {
                if(IsForMyDriver(message))
                {
                    Model.Status = message.DriverStatus;
                }
            }

            /// <inheritdoc/>
            public override void Visit(SetDriverQuallyTimeMessage message)
            {
                if(IsForMyDriver(message))
                {
                    Model.QuallyTimes.SetTime(message.QuallyNumber, message.QuallyTime);
                }
            }

            #endregion

            #region Private Impl.

            private bool IsForMyDriver(DriverMessageBase message)
            {
                return message.DriverId == Model.Id;
            }

            private DriverModel Model { get; set; }

            #endregion
        }
    }
}