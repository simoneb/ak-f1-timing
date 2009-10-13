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
using System.IO;
using System.Text;

using AK.F1.Timing.Extensions;
using AK.F1.Timing.Messaging.Live.Encryption;
using AK.F1.Timing.Messaging.Live.IO;
using AK.F1.Timing.Messaging.Messages.Driver;
using AK.F1.Timing.Messaging.Messages.Feed;
using AK.F1.Timing.Messaging.Messages.Session;
using AK.F1.Timing.Messaging.Messages.Weather;
using AK.F1.Timing.Utility;

namespace AK.F1.Timing.Messaging.Live
{    
    /// <summary>
    /// 
    /// </summary>
    public sealed class LiveMessageReader : MessageReaderBase
    {
        #region Private Fields.

        private const int BUFFER_SIZE = 256;

        private static readonly Encoding UTF8 = Encoding.UTF8;
        private static readonly Encoding UTF_16LE = Encoding.GetEncoding("UTF-16LE");
        private static readonly Encoding ISO_8859_1 = Encoding.GetEncoding("ISO-8859-1");

        #endregion

        #region Public Interface.

        /// <summary>
        /// 
        /// </summary>        
        /// <param name="messageStreamEndpoint"></param>
        /// <param name="decryptorFactory"></param>
        public LiveMessageReader(IMessageStreamEndpoint messageStreamEndpoint,
            IDecryptorFactory decryptorFactory) {
            
            Guard.NotNull(messageStreamEndpoint, "messageStreamEndpoint");
            Guard.NotNull(decryptorFactory, "decryptorFactory");
            
            this.MessageStreamEndpoint = messageStreamEndpoint;
            this.DecryptorFactory = decryptorFactory;
            this.QueuedMessages = new Queue<Message>();            
            this.SessionType = SessionType.None;
            this.State = LiveMessageReaderState.Initial;
            this.StateEngine = new LiveMessageReaderStateEngine(this);
            this.MessageTranslator = new LiveMessageTranslator();
        }

        #endregion

        #region Protected Interface.

        /// <inheritdoc />
        protected override Message ReadImpl() {

            try {
                switch(this.State) {
                    case LiveMessageReaderState.Initial:
                        InitialiseOnFirstRead();                        
                        break;
                    case LiveMessageReaderState.Reading:                        
                        break;
                    case LiveMessageReaderState.Closing:                        
                        break;
                    case LiveMessageReaderState.Closed:
                        return null;
                    case LiveMessageReaderState.Error:
                        Guard.Fail("LiveMessageReader.ReadImpl should not have been called when in an error state.");
                        break;
                    default:
                        throw Guard.ArgumentOutOfRange("LiveMessageReader.CurrentState");
                }
            } catch {
                DisposeOfMessageStream();
                this.State = LiveMessageReaderState.Error;
                throw;
            }

            Message message = DequeueOrReadNextMessage();

            PostProcessMessage(message, true);

            return message;
        }

        #endregion

        #region Protected Interface.

        /// <inheritdoc />
        protected override void Dispose(bool disposing) {

            if(disposing && !this.IsDisposed) {
                DisposeOfMessageStream();
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Internal Interface.

        internal void DisposeOfMessageStream() {

            DisposeOf(this.MessageStream);
            this.MessageStream = null;
        }

        internal IMessageStream MessageStream { get; set; }

        internal SessionType SessionType { get; set; }

        internal IDecryptorFactory DecryptorFactory { get; set; }

        internal Queue<Message> QueuedMessages { get; set; }

        internal IDecryptor Decryptor { get; set; }

        internal LiveMessageReaderState State { get; set; }

        #endregion

        #region Private Impl.

        private Message DequeueOrReadNextMessage() {

            return this.QueuedMessages.Count > 0 ? this.QueuedMessages.Dequeue() : ReadMessage();
        }

        private void PostProcessMessage(Message message, bool translate) {

            this.StateEngine.Process(message);
            if(translate) {
                Message translated = this.MessageTranslator.Translate(message);
                if(translated != null) {
                    this.QueuedMessages.Enqueue(translated);
                }
            }
        }

        private void InitialiseOnFirstRead() {

            Message message;
            SetKeyframeMessage keyframeMessage;

            this.Log.Info("initialising on first read");

            this.Buffer = CreateBuffer();
            this.Decryptor = this.DecryptorFactory.Create();
            this.MessageStream = this.MessageStreamEndpoint.Open();
            message = ReadMessage();
            if((keyframeMessage = message as SetKeyframeMessage) == null) {
                this.Log.ErrorFormat("unexpected first message, expected set keyframe, instead: {0}", message);
                throw Guard.LiveMessageReader_UnexpectedFirstMessage(message);
            }
            EnqueueMessagesFromKeyframe(keyframeMessage.Keyframe);
            this.State = LiveMessageReaderState.Reading;
        }

        private void EnqueueMessagesFromKeyframe(int keyframe) {

            Message message;
            SetKeyframeMessage keyframeMessage;

            this.Log.InfoFormat("enqueuing messages from keyframe {0}", keyframe);

            using(StreamAndBufferBackup()) {
                this.MessageStream = this.MessageStreamEndpoint.OpenKeyframe(keyframe);
                this.Decryptor.Reset();
                try {
                    do {
                        if((message = ReadMessage()) != Message.Empty) {
                            this.QueuedMessages.Enqueue(message);
                            PostProcessMessage(message, false);
                        }
                    } while((keyframeMessage = message as SetKeyframeMessage) == null);
                    if(keyframeMessage.Keyframe > keyframe) {
                        this.Log.InfoFormat("keyframe contained a set keyframe message " +
                            "with a higher keyframe number ({0}) than the one currently " +
                            "being read ({1}), reloading.", keyframeMessage.Keyframe, keyframe);
                        // Clear all messages contained within the keyframe as they will be
                        // superceeded in the new keyframe.
                        this.QueuedMessages.Clear();
                        EnqueueMessagesFromKeyframe(keyframeMessage.Keyframe);
                        return;
                    }
                } finally {
                    DisposeOfMessageStream();
                }
            }

            this.Decryptor.Reset();

            this.Log.InfoFormat("enqueued {0} messages from keyframe {1}",
                this.QueuedMessages.Count, keyframe);
        }

        private Message ReadMessage() {
            
            LiveMessageHeader header = ReadHeader();

            if(header.IsDriverMessage) {             
                return ReadDriverMessage(header);
            }
            Guard.Assert(header.IsSystemMessage);
            return ReadSystemMessage(header);
        }

        private Message ReadSystemMessage(LiveMessageHeader header) {

            switch(header.MessageType) {
                case 0:
                    return ReadSetNextMessageDelayMessage();
                case 1:
                    return ReadSetSessionTypeMessage(header);
                case 2:
                    return ReadSetKeyframeMessage(header);
                case 3:
                    return ReadSetStreamValidityMessage(header);
                case 4:
                    return ReadAddCommentaryMessage(header);
                case 5:
                    return ReadRefrehRateMessage(header);
                case 6:
                    return ReadSetSystemMessageMessage(header);
                case 7:
                    return ReadSetElapsedSessionTimeMessage(header);
                case 9:
                    if(header.DataLength >= 15) {
                        return new StartSessionTimeCountdownMessage();
                    }
                    if(header.Colour > 0) {
                        return this.ReadWeatherMessage(header);
                    }
                    if(header.DataLength > 0) {
                        return this.ReadSetRemainingSessionTimeMessage(header);
                    }
                    return Message.Empty;
                case 10:
                    return ReadApexSpeedMessage(header);
                case 11:
                    return ReadSetSessionStatusMessage(header);
                case 12:
                    return ReadSetCopyrightMessage(header);
                default:
                    this.Log.ErrorFormat("unknown system message: {0}", header);
                    throw Guard.LiveMessageReader_InvalidMessageType(header.MessageType);
            }
        }

        private Message ReadSetNextMessageDelayMessage() {

            ReadBytes(16);

            return new SetNextMessageDelayMessage(TimeSpan.Parse(GetLatin1(0, 16).Trim()));
        }

        private Message ReadDriverMessage(LiveMessageHeader header) {

            if(header.MessageType == 0) {
                return ReadSetDriverPositionMessage(header);
            }
            if(header.MessageType == 15) {
                // We ignore historical position updates.
                ReadAndDecryptBytes(header.Value);
                return Message.Empty;
            }
            if(header.MessageType <= 13) {
                return ReadGridColumnMessage(header);
            }
            this.Log.ErrorFormat("unknown driver related message: {0}", header);
            throw Guard.LiveMessageReader_InvalidMessageType(header.MessageType);
        }

        private Message ReadGridColumnMessage(LiveMessageHeader header) {            
            
            bool isSetClear = header.DataLength == 0;
            bool isSetValue = header.DataLength > 0 && header.DataLength < 15 && header.MessageType <= 13;
            bool isSetColour = !(isSetClear || isSetValue);            

            if(isSetValue) {
                return ReadSetGridColumnValueMessage(header);                
            }
            if(isSetColour) {
                return ReadSetGridColumnColourMessage(header);
            }
            Guard.Assert(isSetClear);
            return ReadClearGridColumnValueMessage(header);
        }

        private Message ReadSetGridColumnValueMessage(LiveMessageHeader header) {

            ReadAndDecryptBytes(header.DataLength);

            string value = GetLatin1(0, header.DataLength);

            return new SetGridColumnValueMessage(
                header.DriverId,
                LiveData.ToGridColumn(header.MessageType, this.SessionType),
                LiveData.ToGridColumnColour(header.Colour),
                value);
        }

        private Message ReadSetGridColumnColourMessage(LiveMessageHeader header) {

            return new SetGridColumnColourMessage(
                header.DriverId,
                LiveData.ToGridColumn(header.MessageType, this.SessionType),
                LiveData.ToGridColumnColour(header.Colour));
        }

        private Message ReadClearGridColumnValueMessage(LiveMessageHeader header) {

            return new SetGridColumnValueMessage(
                header.DriverId,
                LiveData.ToGridColumn(header.MessageType, this.SessionType),
                LiveData.ToGridColumnColour(header.Colour),
                null);
        }

        private Message ReadSetDriverPositionMessage(LiveMessageHeader header) {

            int position = header.Value;            

            // A position of zero instructs the UI to clear the driver's row.
            if(position == 0) {
                return new ClearGridRowMessage(header.DriverId);
            }

            return new SetDriverPositionMessage(header.DriverId, position);
        }

        private Message ReadWeatherMessage(LiveMessageHeader header) {

            ReadAndDecryptBytes(header.DataLength);

            string s = GetLatin1(0, header.DataLength);

            switch(header.Colour) {
                case 1:
                    return new SetTrackTemperatureMessage(LiveData.ParseDouble(s));
                case 2:
                    return new SetAirTemperatureMessage(LiveData.ParseDouble(s));
                case 3:
                    return new SetWetDryMessage(LiveData.ParseInt32(s));
                case 4:
                    return new SetWindSpeedMessage(LiveData.ParseDouble(s));
                case 5:
                    return new SetHumidityMessage(LiveData.ParseDouble(s));
                case 6:
                    return new SetAtmosphericPressureMessage(LiveData.ParseDouble(s));
                case 7:
                    return new SetWindAngleMessage(LiveData.ParseInt32(s));
                default:
                    this.Log.ErrorFormat("unknown weather message: {0}", header);
                    // TODO might it be better to pass header.ToString()?
                    throw Guard.LiveMessageReader_InvalidMessageType(header.MessageType);
            }            
        }

        private Message ReadSetElapsedSessionTimeMessage(LiveMessageHeader header) {

            ReadAndDecryptBytes(2);

            int seconds = this.Buffer[1] << 8 & 0xFF00 | this.Buffer[0] & 0xFF |
                header.Value << 16 & 0xFF0000;

            return new SetElapsedSessionTimeMessage(TimeSpan.FromSeconds(seconds));
        }

        private Message ReadSetRemainingSessionTimeMessage(LiveMessageHeader header) {

            ReadAndDecryptBytes(header.DataLength);

            TimeSpan remaining = LiveData.ParseTime(GetLatin1(0, header.DataLength));

            return new CompositeMessage(
                new StopSessionTimeCountdownMessage(),
                new SetRemainingSessionTimeMessage(remaining));
        }

        private Message ReadAddCommentaryMessage(LiveMessageHeader header) {

            ReadAndDecryptBytes(header.Value);

            return new AddCommentaryMessage(GetUtf8(2, header.Value - 2));
        }

        private Message ReadApexSpeedMessage(LiveMessageHeader header) {

            ReadAndDecryptBytes(header.Value);

            // TODO parse these out.
            //string s = GetLatin1(1, header.Value - 1);

            return Message.Empty;
        }

        private Message ReadSetSessionTypeMessage(LiveMessageHeader header) {

            ReadBytes(header.DataLength);

            return new SetSessionTypeMessage(
                LiveData.ToSessionType(header.Colour),
                GetLatin1(1, header.DataLength - 1));
        }

        private Message ReadSetSessionStatusMessage(LiveMessageHeader header) {

            ReadAndDecryptBytes(header.DataLength);

            return new SetSessionStatusMessage(
                LiveData.ToSessionStatus(GetLatin1(0, header.DataLength)));
        }

        private Message ReadSetKeyframeMessage(LiveMessageHeader header) {

            if(header.DataLength != 2) {
                this.Log.ErrorFormat("invalid keyframe data length: {0}", header.DataLength);
                throw Guard.MessageReader_InvalidMessage();
            }

            ReadBytes(header.DataLength);

            return new SetKeyframeMessage(this.Buffer[1] << 8 & 0xFF00 | this.Buffer[0] & 0xFF);
        }

        private Message ReadSetStreamValidityMessage(LiveMessageHeader header) {

            return new SetStreamValidityMessage(header.Colour != 0);
        }

        private Message ReadRefrehRateMessage(LiveMessageHeader header) {

            return new SetPingIntervalMessage(new TimeSpan(0, 0, header.Value));
        }

        private Message ReadSetSystemMessageMessage(LiveMessageHeader header) {

            ReadAndDecryptBytes(header.Value);

            return new SetSystemMessageMessage(GetUtf8(0, header.Value));
        }

        private Message ReadSetCopyrightMessage(LiveMessageHeader header) {

            ReadBytes(header.Value);

            return new SetCopyrightMessage(GetUtf8(0, header.Value));
        }

        private IDisposable StreamAndBufferBackup() {

            byte[] buffer = this.Buffer;
            IMessageStream stream = this.MessageStream;

            this.Buffer = CreateBuffer();

            return new DisposableCallback(() => {
                this.Buffer = buffer;
                this.MessageStream = stream;
            });
        }

        private static byte[] CreateBuffer() {

            return new byte[BUFFER_SIZE];
        }

        private LiveMessageHeader ReadHeader() {

            ReadBytes(2);

            int b0 = this.Buffer[0];
            int b1 = this.Buffer[1];

            return new LiveMessageHeader() {
                DriverId =  (byte)(b0 & 0x1F),
                MessageType = (byte)((b0 & 0xE0) >> 5 & 0x7 | (b1 & 0x1) << 3),
                Colour = (byte)((b1 & 0xE) >> 1),
                DataLength = (byte)((b1 & 0xF0) >> 4),
                Value = (byte)((b1 & 0xFE) >> 1)
            };
        }

        private void ReadBytes(int count) {

            if(!this.MessageStream.FullyRead(this.Buffer, 0, count)) {
                throw Guard.UnexpectedEndOfStream();
            }
        }

        private void ReadAndDecryptBytes(int count) {

            ReadBytes(count);
            this.Decryptor.Decrypt(this.Buffer, 0, count);
        }

        private string GetUtf8(int offset, int count) {

            return UTF8.GetString(this.Buffer, offset, count);
        }

        private string GetLatin1(int offset, int count) {

            return ISO_8859_1.GetString(this.Buffer, offset, count);
        }

        private string GetUtf16LE(int offset, int count) {

            return UTF_16LE.GetString(this.Buffer, offset, count);
        }        

        private byte[] Buffer { get; set; }

        private IMessageStreamEndpoint MessageStreamEndpoint { get; set; }        

        private LiveMessageReaderStateEngine StateEngine { get; set; }

        private LiveMessageTranslator MessageTranslator { get; set; }

        #endregion
    }    
}