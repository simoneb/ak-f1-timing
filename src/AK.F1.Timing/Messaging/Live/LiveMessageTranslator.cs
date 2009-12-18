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
using System.Collections.Generic;
using System.Text;

using AK.F1.Timing.Extensions;
using AK.F1.Timing.Messaging.Messages.Driver;
using AK.F1.Timing.Messaging.Messages.Feed;
using AK.F1.Timing.Messaging.Messages.Session;

namespace AK.F1.Timing.Messaging.Live
{
    /// <summary>
    /// Translates the messages read from a <see cref="LiveMessageReader"/> into more meaningful
    /// messages. This class is <see langword="sealed"/>.
    /// </summary>
    [Serializable]
    public sealed class LiveMessageTranslator : MessageVisitor
    {
        #region Private Fields.

        private static readonly log4net.ILog _log =
            log4net.LogManager.GetLogger(typeof(LiveMessageTranslator));

        /// <summary>
        /// The maximum ping inteval before we interpret it as being the end of the session. This
        /// field is <see langword="readonly"/>.
        /// </summary>
        private static readonly TimeSpan MAX_PING_INTERVAL = TimeSpan.FromSeconds(30);

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="LiveMessageTranslator"/> class.
        /// </summary>
        public LiveMessageTranslator() {

            this.Drivers = new List<LiveDriver>(25);
            this.SessionType = SessionType.None;
            this.StateEngine = new LiveMessageTranslatorStateEngine(this);            
        }

        /// <summary>
        /// Resets all state information associated with this translator.
        /// </summary>
        public void Reset() {
            
            this.RaceLapNumber = 0;
            this.SessionType = SessionType.None;
            foreach(var driver in this.Drivers) {
                driver.Reset();
            }
        }

        /// <summary>
        /// Attempts to translate the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to translate.</param>
        /// <returns>The translated message if possible, otherwise; <see langword="null"/>.</returns>
        public Message Translate(Message message) {

            this.Translated = null;

            message.Accept(this);
            this.StateEngine.Process(message);
            if(this.Translated != null) {
                this.StateEngine.Process(this.Translated);                
            }

            return this.Translated;
        }

        /// <inheritdoc />
        public override void Visit(SetPingIntervalMessage message) {

            this.Translated = Translate(message);         
        }        

        /// <inheritdoc />
        public override void Visit(SetGridColumnColourMessage message) {

            this.Translated = Translate(message);
        }

        /// <inheritdoc />
        public override void Visit(SetGridColumnValueMessage message) {

            this.Translated = Translate(message);
        }

        #endregion

        #region Internal Interface.

        /// <summary>
        /// Returns the driver that the specified message relates to.
        /// </summary>
        /// <param name="message">The driver related message.</param>
        /// <returns>The driver that the specified message related to.</returns>
        internal LiveDriver GetDriver(DriverMessageBase message) {            

            while(this.Drivers.Count < message.DriverId) {
                this.Drivers.Add(new LiveDriver(message.DriverId));
            }
            
            return this.Drivers[message.DriverId - 1];
        }

        /// <summary>
        /// Gets or sets the current session type.
        /// </summary>
        internal SessionType SessionType { get; set; }

        /// <summary>
        /// Gets the collection of drivers.
        /// </summary>
        internal IList<LiveDriver> Drivers { get; private set; }

        /// <summary>
        /// Gets or sets the current race lap number.
        /// </summary>
        internal int RaceLapNumber { get; set; }

        /// <summary>
        /// Gets a value indicating if the session has started.
        /// </summary>
        internal bool HasSessionStarted {

            get {
                return this.SessionType != SessionType.None &&
                    (this.SessionType != SessionType.Race || this.RaceLapNumber > 0);
            }
        }

        #endregion

        #region Private Impl.

        private Message Translate(SetPingIntervalMessage message) {

            if(!(message.PingInterval == TimeSpan.Zero || message.PingInterval >= MAX_PING_INTERVAL)) {
                return null;
            }

            _log.InfoFormat("read terminal message {0}", message);

            return new CompositeMessage(
                new SetSessionStatusMessage(SessionStatus.Finished),
                EndOfSessionMessage.Instance);
        }

        private Message Translate(SetGridColumnValueMessage message) {

            if(message.ClearColumn) {
                switch(message.Column) {
                    case GridColumn.S1:
                        return TranslateSetSectorClear(message, 1);
                    case GridColumn.S2:
                        return TranslateSetSectorClear(message, 2);
                    case GridColumn.S3:
                        return TranslateSetSectorClear(message, 3);
                }
            } else {
                switch(message.Column) {
                    case GridColumn.CarNumber:
                        return TranslateSetCarNumberValue(message);
                    case GridColumn.DriverName:
                        return TranslateSetNameValue(message);
                    case GridColumn.LapTime:
                        return TranslateSetLapTimeValue(message);
                    case GridColumn.Gap:
                        return TranslateSetGapTimeValue(message);
                    case GridColumn.S1:
                        return TranslateSetSectorTimeValue(message, 1);
                    case GridColumn.S2:
                        return TranslateSetSectorTimeValue(message, 2);
                    case GridColumn.S3:
                        return TranslateSetSectorTimeValue(message, 3);
                    case GridColumn.Laps:
                        return TranslateSetCompletedLapsValue(message);
                    case GridColumn.Interval:
                        return TranslateSetIntervalTimeValue(message);
                    case GridColumn.Q1:
                        return TranslateSetQuallyTimeValue(message, 1);
                    case GridColumn.Q2:
                        return TranslateSetQuallyTimeValue(message, 2);
                    case GridColumn.Q3:
                        return TranslateSetQuallyTimeValue(message, 3);
                    case GridColumn.PitCount:
                        return TranslateSetPitCountValue(message);
                }
            }

            return null;
        }

        private Message Translate(SetGridColumnColourMessage message) {

            // Yellow indicates that the next column is about to / has received an update and this
            // column is no longer shows the latest information for the driver.
            if(message.Colour == GridColumnColour.Yellow) {
                return null;
            }
            // The feed often seeds colour updates to columns which have no value.
            if(!GetDriver(message).ColumnHasValue(message.Column)) {
                return Ignored("column has no value", message);
            }
            switch(message.Column) {
                case GridColumn.CarNumber:
                    return TranslateSetCarNumberColour(message);
                case GridColumn.LapTime:
                    return TranslateSetLapTimeColour(message);
                case GridColumn.Gap:
                    return TranslateSetGapTimeColour(message);
                case GridColumn.S1:
                    return TranslateSetSectorTimeColour(message, 1);
                case GridColumn.S2:
                    return TranslateSetSectorTimeColour(message, 2);
                case GridColumn.S3:
                    return TranslateSetSectorTimeColour(message, 3);
                case GridColumn.Interval:
                    return TranslateSetIntervalTimeColour(message);
                default:
                    return null;
            }
        }

        private Message TranslateSetPitCountValue(SetGridColumnValueMessage message) {

            return new SetDriverPitCountMessage(message.DriverId, LiveData.ParseInt32(message.Value));
        }

        private Message TranslateSetIntervalTimeValue(SetGridColumnValueMessage message) {

            // The interval column for the lead driver displays the current lap number.
            if(GetDriver(message).Position == 1) {
                return new CompositeMessage(
                    new SetRaceLapNumberMessage(LiveData.ParseInt32(message.Value)),
                    new SetDriverIntervalMessage(message.DriverId, TimeGap.Zero));
            }

            if(message.Value.OrdinalEndsWith("L")) {
                string s = message.Value.Substring(0, message.Value.Length - 1);
                return new SetDriverIntervalMessage(message.DriverId, new LapGap(LiveData.ParseInt32(s)));
            }

            return new SetDriverIntervalMessage(message.DriverId,
                new TimeGap(LiveData.ParseTime(message.Value)));
        }

        private Message TranslateSetIntervalTimeColour(SetGridColumnColourMessage message) {

            return message.Colour == GridColumnColour.White ? GetDriver(message).LastIntervalMessage : null;
        }

        private Message TranslateSetCompletedLapsValue(SetGridColumnValueMessage message) {            

            return new SetDriverCompletedLapsMessage(message.DriverId, LiveData.ParseInt32(message.Value));
        }

        private Message TranslateSetGapTimeValue(SetGridColumnValueMessage message) {

            // LAP is displayed in the gap column of the lead driver.
            if(message.Value.OrdinalEquals("LAP")) {
                return new SetDriverGapMessage(message.DriverId, TimeGap.Zero);
            }

            if(message.Value.OrdinalEndsWith("L")) {
                string s = message.Value.Substring(0, message.Value.Length - 1);
                return new SetDriverGapMessage(message.DriverId,
                    new LapGap(LiveData.ParseInt32(s)));
            }

            return new SetDriverGapMessage(message.DriverId,
                new TimeGap(LiveData.ParseTime(message.Value)));
        }

        private Message TranslateSetGapTimeColour(SetGridColumnColourMessage message) {

            return message.Colour == GridColumnColour.White ? GetDriver(message).LastGapMessage : null;
        }

        private Message TranslateSetLapTimeValue(SetGridColumnValueMessage message) {

            if(!this.HasSessionStarted) {
                return Ignored("session has not started", message);
            }

            LiveDriver driver = GetDriver(message);

            if(message.Value.OrdinalEquals("OUT")) {
                return CreateStatusMessageIfStatusChanged(driver, DriverStatus.OnTrack);
            }

            if(message.Value.OrdinalEquals("IN PIT")) {
                return CreateStatusMessageIfStatusChanged(driver, DriverStatus.InPits);
            }

            if(message.Value.OrdinalEquals("RETIRED")) {
                return CreateStatusMessageIfStatusChanged(driver, DriverStatus.Retired);
            }

            return new SetDriverLapTimeMessage(driver.Id, new PostedTime(
                LiveData.ParseTime(message.Value),
                LiveData.ToPostedTimeType(message.Colour),
                driver.LapNumber));
        }

        private Message TranslateSetLapTimeColour(SetGridColumnColourMessage message) {

            LiveDriver driver = GetDriver(message);

            switch(message.Colour) {
                case GridColumnColour.White:
                    _log.DebugFormat("using previous lap time: {0}", message);
                    return new SetDriverLapTimeMessage(driver.Id, new PostedTime(
                        driver.LastLapTime.Time,
                        LiveData.ToPostedTimeType(message.Colour),
                        driver.LapNumber));
                case GridColumnColour.Green:
                case GridColumnColour.Magenta:
                    // The feed often sends a colour update for the previous lap time to indicate
                    // that it was a PB or SB. To hack this we publish a replacement when we receive
                    // such a message.
                    _log.DebugFormat("received out of order lap time colour update: {0}", message);
                    return new ReplaceDriverLapTimeMessage(driver.Id, new PostedTime(
                        driver.LastLapTime.Time,
                        LiveData.ToPostedTimeType(message.Colour),
                        driver.LastLapTime.LapNumber));
                default:
                    return null;
            }
        }

        private Message TranslateSetQuallyTimeValue(SetGridColumnValueMessage message, int quallyNumber) {

            return new SetDriverQuallyTimeMessage(message.DriverId, quallyNumber,
                LiveData.ParseTime(message.Value));
        }

        private Message TranslateSetSectorTimeValue(SetGridColumnValueMessage message, int sectorNumber) {

            LiveDriver driver = GetDriver(message);

            if(driver.IsPitTimeSector(this.SessionType)) {
                if(sectorNumber != 3) {
                    return Ignored("irrelevant pit time sector update", message);
                }
                // After a driver pits, the pit times are displayed and the S3 column always displays the
                // length of the last pit stop.
                return new SetDriverPitTimeMessage(driver.Id,
                    LiveData.ParseTime(message.Value),
                    driver.LapNumber);
            }

            if(message.Value.OrdinalEquals("OUT")) {
                return CreateStatusMessageIfStatusChanged(driver, DriverStatus.Out);
            }

            if(message.Value.OrdinalEquals("STOP")) {
                return CreateStatusMessageIfStatusChanged(driver, DriverStatus.Stopped);
            }

            return Translate(new SetDriverSectorTimeMessage(driver.Id, sectorNumber, new PostedTime(
                LiveData.ParseTime(message.Value),
                LiveData.ToPostedTimeType(message.Colour),
                driver.LapNumber)));
        }

        private Message TranslateSetSectorTimeColour(SetGridColumnColourMessage message, int sectorNumber) {
            
            LiveDriver driver = GetDriver(message);

            if(driver.IsPitTimeSector(this.SessionType)) {
                return null;
            }

            PostedTime lastSectorTime = driver.LastSectors[sectorNumber - 1];
            PostedTimeType newTimeType = LiveData.ToPostedTimeType(message.Colour);
            // The feed often sends a colour update for the previous sector time to indicate that
            // it was a PB or SB. To hack this we publish a replacement when we receive such a
            // message.
            if(sectorNumber != driver.NextSectorNumber) {                
                if(!driver.IsPreviousSectorNumber(sectorNumber)) {
                    _log.WarnFormat("received completely out of order S{0} update when an S{1} update" +
                        " was expected, cannot translate this message: {2}", sectorNumber,
                        driver.NextSectorNumber, message);
                    return null;
                }
                _log.DebugFormat("received out of order sector update: {0}", message);
                return new ReplaceDriverSectorTimeMessage(driver.Id, sectorNumber,
                    new PostedTime(lastSectorTime.Time, newTimeType, lastSectorTime.LapNumber));
            }

            _log.DebugFormat("using previous sector time with new colour: {0}", message);

            return Translate(new SetDriverSectorTimeMessage(driver.Id, sectorNumber,
                new PostedTime(lastSectorTime.Time, newTimeType, driver.LapNumber)));
        }

        private Message TranslateSetSectorClear(SetGridColumnValueMessage message, int sectorNumber) {

            PostedTime lastS1Time;
            LiveDriver driver = GetDriver(message);
            // The feed will only send a value / colour update for S1 if the value has changed. We
            // can detect this when the S2 time is cleared and we are expecting an S1 update. Note
            // that an S2 clear can be received when we do not have a previous S1 time, usually
            // at the start of a session.
            if(sectorNumber == 2 && driver.NextSectorNumber == 1 &&
                (lastS1Time = driver.LastSectors[0]) != null) {
                _log.Debug("received clear S2 before S1 set, using previous posted time");
                return Translate(new SetDriverSectorTimeMessage(driver.Id, 1,
                    new PostedTime(lastS1Time.Time, lastS1Time.Type, driver.LapNumber)));
            }

            return null;
        }

        private Message Translate(SetDriverSectorTimeMessage message) {

            if(message.SectorNumber != 3 || this.SessionType != SessionType.Race) {
                return message;
            }
            // We do not receive lap messages during a race session so we infer it from the
            // current race lap number and the driver's current gap.
            return new CompositeMessage(message,
                new SetDriverCompletedLapsMessage(message.DriverId,
                    GetDriver(message).ComputeLapNumber(this.RaceLapNumber)));
        }

        private Message TranslateSetNameValue(SetGridColumnValueMessage message) {

            return new SetDriverNameMessage(message.DriverId, message.Value);
        }

        private Message TranslateSetCarNumberValue(SetGridColumnValueMessage message) {

            Message temp;
            Message translated = null;            
            LiveDriver driver = GetDriver(message);
            int carNumber = LiveData.ParseInt32(message.Value);
            DriverStatus status = LiveData.ToDriverStatus(message.Colour);
            
            if(driver.CarNumber != carNumber) {
                translated = new SetDriverCarNumberMessage(driver.Id, carNumber);
            }
            if(driver.Status != status) {
                temp = new SetDriverStatusMessage(driver.Id, status);
                translated = translated == null ? temp : new CompositeMessage(translated, temp);
            }

            return translated;
        }

        private Message TranslateSetCarNumberColour(SetGridColumnColourMessage message) {

            LiveDriver driver = GetDriver(message);
            DriverStatus status = LiveData.ToDriverStatus(message.Colour);

            return CreateStatusMessageIfStatusChanged(driver, status);
        }

        private static Message CreateStatusMessageIfStatusChanged(LiveDriver driver, DriverStatus status) {

            if(driver.Status != status) {
                return new SetDriverStatusMessage(driver.Id, status);
            }

            return null;
        }

        private static Message Ignored(string reason, Message message) {

            if(_log.IsDebugEnabled) {
                _log.Debug(new StringBuilder()
                    .Append("ignored, ")
                    .Append(reason)
                    .Append(": ")
                    .Append(message));
            }

            return null;
        }

        private Message Translated { get; set; }

        private LiveMessageTranslatorStateEngine StateEngine { get; set; }        

        #endregion
    }
}
