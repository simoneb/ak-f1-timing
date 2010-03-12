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

namespace AK.F1.Timing
{
    /// <summary>
    /// Defines a base class for <see cref="AK.F1.Timing.IMessageVisitor"/> implementations.
    /// This class is <see langword="abstract"/>.
    /// </summary>
    [Serializable]
    public abstract class MessageVisitor : IMessageVisitor
    {
        #region Public Interface.
   
        /// <summary>
        /// Defines a null message visitor. This field is <see langword="readonly"/>.
        /// </summary>
        public static readonly IMessageVisitor Null = new NullMessageVisitor();

        /// <inheritdoc />
        public virtual void Visit(EndOfSessionMessage message) { }

        /// <inheritdoc />
        public virtual void Visit(StartSessionTimeCountdownMessage message) { }

        /// <inheritdoc />
        public virtual void Visit(StopSessionTimeCountdownMessage message) { }

        /// <inheritdoc />
        public virtual void Visit(ClearGridRowMessage message) { }

        /// <inheritdoc />
        public virtual void Visit(SetGridColumnColourMessage message) { }

        /// <inheritdoc />
        public virtual void Visit(SetGridColumnValueMessage message) { }

        /// <inheritdoc />
        public virtual void Visit(ReplaceDriverLapTimeMessage message) { }

        /// <inheritdoc />
        public virtual void Visit(ReplaceDriverSectorTimeMessage message) { }

        /// <inheritdoc />
        public virtual void Visit(SetElapsedSessionTimeMessage message) { }

        /// <inheritdoc />
        public virtual void Visit(SetAirTemperatureMessage message) { }

        /// <inheritdoc />
        public virtual void Visit(SetAtmosphericPressureMessage message) { }

        /// <inheritdoc />
        public virtual void Visit(AddCommentaryMessage message) { }

        /// <inheritdoc />
        public virtual void Visit(SetCopyrightMessage message) { }

        /// <inheritdoc />
        public virtual void Visit(SetDriverLapTimeMessage message) { }

        /// <inheritdoc />
        public virtual void Visit(SetDriverCompletedLapsMessage message) { }        

        /// <inheritdoc />
        public virtual void Visit(SetDriverGapMessage message) { }

        /// <inheritdoc />
        public virtual void Visit(SetDriverCarNumberMessage message) { }

        /// <inheritdoc />
        public virtual void Visit(SetDriverIntervalMessage message) { }

        /// <inheritdoc />
        public virtual void Visit(SetDriverNameMessage message) { }

        /// <inheritdoc />
        public virtual void Visit(SetDriverLapNumberMessage message) { }

        /// <inheritdoc />
        public virtual void Visit(SetDriverPitCountMessage message) { }

        /// <inheritdoc />
        public virtual void Visit(SetDriverPositionMessage message) { }

        /// <inheritdoc />
        public virtual void Visit(SetDriverQuallyTimeMessage message) { }

        /// <inheritdoc />
        public virtual void Visit(SetDriverSectorTimeMessage message) { }

        /// <inheritdoc />
        public virtual void Visit(SetDriverStatusMessage message) { }

        /// <inheritdoc />
        public virtual void Visit(SetKeyframeMessage message) { }

        /// <inheritdoc />
        public virtual void Visit(SetHumidityMessage message) { }

        /// <inheritdoc />
        public virtual void Visit(SetRaceLapNumberMessage message) { }

        /// <inheritdoc />
        public virtual void Visit(SetSessionStatusMessage message) { }

        /// <inheritdoc />
        public virtual void Visit(SetPingIntervalMessage message) { }

        /// <inheritdoc />
        public virtual void Visit(SetSystemMessageMessage message) { }

        /// <inheritdoc />
        public virtual void Visit(SetSessionTypeMessage message) { }

        /// <inheritdoc />
        public virtual void Visit(SetStreamValidityMessage message) { }

        /// <inheritdoc />
        public virtual void Visit(SetTrackTemperatureMessage message) { }

        /// <inheritdoc />
        public virtual void Visit(SetIsWetMessage message) { }

        /// <inheritdoc />
        public virtual void Visit(SetWindAngleMessage message) { }

        /// <inheritdoc />
        public virtual void Visit(SetWindSpeedMessage message) { }

        /// <inheritdoc />
        public virtual void Visit(SetRemainingSessionTimeMessage message) { }

        /// <inheritdoc />
        public virtual void Visit(SetNextMessageDelayMessage message) { }

        /// <inheritdoc />
        public virtual void Visit(SetDriverPitTimeMessage message) { }

        #endregion

        #region Private Impl.

        [Serializable]
        private sealed class NullMessageVisitor : MessageVisitor { }

        #endregion
    }
}
