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
using AK.F1.Timing.Messages.Feed;
using AK.F1.Timing.Messages.Session;
using AK.F1.Timing.Messages.Weather;

namespace AK.F1.Timing.Model.Session
{
    /// <summary>
    /// An <see cref="AK.F1.Timing.IMessageProcessor"/> which dispatches driver related messages
    /// to drivers provided by an <see cref="AK.F1.Timing.Model.Session.IDriverModelLocator"/>.
    /// This class cannot be inherited.
    /// </summary>
    [Serializable]
    internal sealed class DriverMessageDispatcher : IMessageProcessor, IMessageVisitor
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="DriverMessageDispatcher"/> class and
        /// specifies the driver model locator.
        /// </summary>
        /// <param name="driverLocator">The driver model locator.</param>
        public DriverMessageDispatcher(IDriverModelLocator driverLocator)
        {
            Guard.NotNull(driverLocator, "driverLocator");

            DriverLocator = driverLocator;
        }

        /// <inheritdoc/>        
        public void Process(Message message)
        {
            message.Accept(this);
        }

        #endregion

        #region Explicit Interface.

        void IMessageVisitor.Visit(SetDriverPitTimeMessage message)
        {
            Dispatch(message);
        }

        void IMessageVisitor.Visit(ClearGridRowMessage message)
        {
            Dispatch(message);
        }

        void IMessageVisitor.Visit(SetGridColumnColourMessage message)
        {
            Dispatch(message);
        }

        void IMessageVisitor.Visit(SetGridColumnValueMessage message)
        {
            Dispatch(message);
        }

        void IMessageVisitor.Visit(ReplaceDriverLapTimeMessage message)
        {
            Dispatch(message);
        }

        void IMessageVisitor.Visit(ReplaceDriverSectorTimeMessage message)
        {
            Dispatch(message);
        }

        void IMessageVisitor.Visit(SetDriverLapTimeMessage message)
        {
            Dispatch(message);
        }

        void IMessageVisitor.Visit(SetDriverLapNumberMessage message)
        {
            Dispatch(message);
        }

        void IMessageVisitor.Visit(SetDriverGapMessage message)
        {
            Dispatch(message);
        }

        void IMessageVisitor.Visit(SetDriverCarNumberMessage message)
        {
            Dispatch(message);
        }

        void IMessageVisitor.Visit(SetDriverIntervalMessage message)
        {
            Dispatch(message);
        }

        void IMessageVisitor.Visit(SetDriverNameMessage message)
        {
            Dispatch(message);
        }

        void IMessageVisitor.Visit(SetDriverPitCountMessage message)
        {
            Dispatch(message);
        }

        void IMessageVisitor.Visit(SetDriverPositionMessage message)
        {
            Dispatch(message);
        }

        void IMessageVisitor.Visit(SetDriverQuallyTimeMessage message)
        {
            Dispatch(message);
        }

        void IMessageVisitor.Visit(SetDriverSectorTimeMessage message)
        {
            Dispatch(message);
        }

        void IMessageVisitor.Visit(SetDriverStatusMessage message)
        {
            Dispatch(message);
        }

        void IMessageVisitor.Visit(SetDriverSpeedMessage message)
        {
            Dispatch(message);
        }

        void IMessageVisitor.Visit(StartSessionTimeCountdownMessage message) { }

        void IMessageVisitor.Visit(EndOfSessionMessage message) { }

        void IMessageVisitor.Visit(StopSessionTimeCountdownMessage message) { }

        void IMessageVisitor.Visit(SetElapsedSessionTimeMessage message) { }

        void IMessageVisitor.Visit(SetAirTemperatureMessage message) { }

        void IMessageVisitor.Visit(SetAtmosphericPressureMessage message) { }

        void IMessageVisitor.Visit(AddCommentaryMessage message) { }

        void IMessageVisitor.Visit(SetCopyrightMessage message) { }

        void IMessageVisitor.Visit(SetKeyframeMessage message) { }

        void IMessageVisitor.Visit(SetHumidityMessage message) { }

        void IMessageVisitor.Visit(SetRemainingSessionTimeMessage message) { }

        void IMessageVisitor.Visit(SetRaceLapNumberMessage message) { }

        void IMessageVisitor.Visit(SetSessionStatusMessage message) { }

        void IMessageVisitor.Visit(SetPingIntervalMessage message) { }

        void IMessageVisitor.Visit(SetSystemMessageMessage message) { }

        void IMessageVisitor.Visit(SetSessionTypeMessage message) { }

        void IMessageVisitor.Visit(SetStreamValidityMessage message) { }

        void IMessageVisitor.Visit(SetTrackTemperatureMessage message) { }

        void IMessageVisitor.Visit(SetIsWetMessage message) { }

        void IMessageVisitor.Visit(SetWindAngleMessage message) { }

        void IMessageVisitor.Visit(SetWindSpeedMessage message) { }

        void IMessageVisitor.Visit(SetNextMessageDelayMessage message) { }

        void IMessageVisitor.Visit(SetMinRequiredQuallyTimeMessage message) { }

        void IMessageVisitor.Visit(SpeedCaptureMessage message) { }

        void IMessageVisitor.Visit(SetStreamTimestampMessage message) { }

        #endregion

        #region Private Impl.

        private void Dispatch(DriverMessageBase message)
        {
            DriverLocator.GetDriver(message.DriverId).Process(message);
        }

        private IDriverModelLocator DriverLocator { get; set; }

        #endregion
    }
}