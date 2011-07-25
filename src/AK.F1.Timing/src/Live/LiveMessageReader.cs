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
using System.Text.RegularExpressions;
using AK.F1.Timing.Live.Encryption;
using AK.F1.Timing.Live.IO;
using AK.F1.Timing.Messages;
using AK.F1.Timing.Messages.Driver;
using AK.F1.Timing.Messages.Feed;
using AK.F1.Timing.Messages.Session;
using AK.F1.Timing.Messages.Weather;
using AK.F1.Timing.Utility;

namespace AK.F1.Timing.Live
{
    /// <summary>
    /// A <see cref="AK.F1.Timing.IMessageReader"/> implementation which reads
    /// <see cref="AK.F1.Timing.Message"/>s serialized by the live-timing servers.
    /// This class cannot be inherited.
    /// </summary>
    public sealed class LiveMessageReader : MessageReaderBase
    {
        #region Private Fields.

        private const int BufferSize = 256;

        private static readonly Encoding Utf8Encoding = Encoding.UTF8;
        private static readonly Encoding Latin1Encoding = Encoding.GetEncoding("ISO-8859-1");

        #endregion

        #region Public Interface.

        /// <summary>
        /// 
        /// </summary>        
        /// <param name="messageStreamEndpoint"></param>
        /// <param name="decrypterFactory"></param>
        public LiveMessageReader(IMessageStreamEndpoint messageStreamEndpoint,
            IDecrypterFactory decrypterFactory)
        {
            Guard.NotNull(messageStreamEndpoint, "messageStreamEndpoint");
            Guard.NotNull(decrypterFactory, "decrypterFactory");

            MessageStreamEndpoint = messageStreamEndpoint;
            DecrypterFactory = decrypterFactory;
            QueuedMessages = new Queue<Message>();
            SessionType = SessionType.None;
            State = LiveMessageReaderState.Uninitialised;
            StateEngine = new LiveMessageReaderStateEngine(this);
            MessageTranslator = new LiveMessageTranslator();
        }

        #endregion

        #region Protected Interface.

        /// <inheritdoc/>
        protected override Message ReadImpl()
        {
            switch(State)
            {
                case LiveMessageReaderState.Reading:
                    break;
                case LiveMessageReaderState.Closed:
                    return null;
                case LiveMessageReaderState.Uninitialised:
                    Initialise();
                    break;
                default:
                    throw Guard.ArgumentOutOfRange("State");
            }
            var message = DequeueOrReadNextMessage();
            // TODO message enqueued from a keyframe should not be subject to post processing.
            PostProcessMessage(message, true);
            return message;
        }

        /// <inheritdoc/>        
        protected override void DisposeOfManagedResources()
        {
            DisposeOfMessageStream();
        }

        #endregion

        #region Internal Interface.

        /// <summary>
        /// Disposes of the current message stream.
        /// </summary>
        internal void DisposeOfMessageStream()
        {
            DisposeOf(MessageStream);
        }

        /// <summary>
        /// Gets the current message stream being read by the reader.
        /// </summary>
        internal IMessageStream MessageStream { get; private set; }

        /// <summary>
        /// Gets or sets the current session type.
        /// </summary>
        internal SessionType SessionType { get; set; }

        /// <summary>
        /// Gets the current decrypter factory being used by the reader.
        /// </summary>
        internal IDecrypterFactory DecrypterFactory { get; private set; }

        /// <summary>
        /// Gets or sets the current decrypter being used by the reader.
        /// </summary>
        internal IDecrypter Decrypter { get; set; }

        /// <summary>
        /// Gets or sets the state of the reader.
        /// </summary>
        internal LiveMessageReaderState State { get; set; }

        #endregion

        #region Private Impl.

        private Message DequeueOrReadNextMessage()
        {
            return QueuedMessages.Count > 0 ? QueuedMessages.Dequeue() : ReadMessage();
        }

        private void PostProcessMessage(Message message, bool translate)
        {
            StateEngine.Process(message);
            if(translate)
            {
                var translated = MessageTranslator.Translate(message);
                if(translated != null)
                {
                    QueuedMessages.Enqueue(translated);
                }
            }
        }

        private void Initialise()
        {
            Log.Info("initialising");

            Buffer = CreateBuffer();
            Decrypter = DecrypterFactory.Create();
            MessageStream = MessageStreamEndpoint.OpenStream();
            try
            {
                var message = ReadMessage();
                var keyframeMessage = message as SetKeyframeMessage;
                if(keyframeMessage == null)
                {
                    Log.ErrorFormat("unexpected first message, expected set keyframe, instead: {0}", message);
                    throw Guard.LiveMessageReader_UnexpectedFirstMessage(message);
                }
                EnqueueMessagesFromKeyframe(keyframeMessage.Keyframe);
                State = LiveMessageReaderState.Reading;
            }
            catch
            {
                DisposeOfMessageStream();
                throw;
            }
        }

        private void EnqueueMessagesFromKeyframe(int keyframe)
        {
            Message message;
            SetKeyframeMessage keyframeMessage;

            Log.InfoFormat("enqueuing messages from keyframe {0}", keyframe);

            using(StreamAndBufferBackup())
            {
                Decrypter.Reset();
                MessageStream = MessageStreamEndpoint.OpenKeyframe(keyframe);
                QueuedMessages.Enqueue(new SetStreamTimestampMessage());
                try
                {
                    do
                    {
                        if((message = ReadMessage()) != Message.Empty)
                        {
                            QueuedMessages.Enqueue(message);
                            PostProcessMessage(message, false);
                        }
                    } while((keyframeMessage = message as SetKeyframeMessage) == null);
                    if(keyframeMessage.Keyframe > keyframe)
                    {
                        Log.InfoFormat("keyframe contained a set keyframe message " +
                            "with a higher keyframe number ({0}) than the one currently " +
                            "being read ({1}), reloading.", keyframeMessage.Keyframe, keyframe);
                        // All queued messages will be superceeded in the new keyframe.
                        QueuedMessages.Clear();
                        MessageTranslator.Reset();
                        EnqueueMessagesFromKeyframe(keyframeMessage.Keyframe);
                        return;
                    }
                }
                finally
                {
                    DisposeOfMessageStream();
                }
            }
            Decrypter.Reset();
            Log.InfoFormat("enqueued {0} messages from keyframe {1}", QueuedMessages.Count, keyframe);
        }

        private Message ReadMessage()
        {
            var header = ReadHeader();

            if(header.IsDriverMessage)
            {
                return ReadDriverMessage(header);
            }
            Guard.Assert(header.IsSystemMessage);
            return ReadSystemMessage(header);
        }

        private Message ReadSystemMessage(LiveMessageHeader header)
        {
            switch(header.MessageType)
            {
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
                    if(header.DataLength >= 15)
                    {
                        return StartSessionTimeCountdownMessage.Instance;
                    }
                    if(header.Colour > 0)
                    {
                        return ReadWeatherMessage(header);
                    }
                    if(header.DataLength > 0)
                    {
                        return ReadSetRemainingSessionTimeMessage(header);
                    }
                    break;
                case 10:
                    return ReadSpeedCaptureMessage(header);
                case 11:
                    if(header.Colour == 1)
                    {
                        return ReadSetSessionStatusMessage(header);
                    }
                    if(header.Colour == 4)
                    {
                        return ReadSetMinRequiredQuallyTimeMessage(header);
                    }
                    // TODO mvoe this out to an UnsupportedTextMessage.
                    ReadAndDecryptBytes(header.DataLength);
                    Log.WarnFormat("soft-fail on unsupported type 11 system message: {0}, {1}",
                        header, DecodeLatin1(header.DataLength));
                    return Message.Empty;
                case 12:
                    return ReadSetCopyrightMessage(header);
                default:
                    Log.ErrorFormat("unsupported system message: {0}", header);
                    throw Guard.LiveMessageReader_UnsupportedSystemMessage(header);
            }
            Log.WarnFormat("soft-fail on unsupported system message: {0}", header);
            return Message.Empty;
        }

        private Message ReadDriverMessage(LiveMessageHeader header)
        {
            if(header.MessageType == 0)
            {
                return ReadSetDriverPositionMessage(header);
            }
            if(header.MessageType <= 13)
            {
                return ReadGridColumnMessage(header);
            }
            if(header.MessageType == 15)
            {
                return ReadHistoricalPositionMessage(header);
            }
            Log.ErrorFormat("unsupported driver message: {0}", header);
            throw Guard.LiveMessageReader_UnsupportedDriverMessage(header);
        }

        private Message ReadGridColumnMessage(LiveMessageHeader header)
        {
            bool isSetClear = header.DataLength == 0;
            bool isSetValue = header.DataLength > 0 && header.DataLength < 15 && header.MessageType <= 13;
            bool isSetColour = !(isSetClear || isSetValue);

            if(isSetValue)
            {
                return ReadSetGridColumnValueMessage(header);
            }
            if(isSetColour)
            {
                return ReadSetGridColumnColourMessage(header);
            }
            return ReadClearGridColumnValueMessage(header);
        }

        private Message ReadSetGridColumnValueMessage(LiveMessageHeader header)
        {
            ReadAndDecryptBytes(header.DataLength);

            var value = DecodeLatin1(header.DataLength);

            return new SetGridColumnValueMessage(
                header.DriverId,
                LiveData.ToGridColumn(header.MessageType, SessionType),
                LiveData.ToGridColumnColour(header.Colour),
                value);
        }

        private Message ReadSetGridColumnColourMessage(LiveMessageHeader header)
        {
            return new SetGridColumnColourMessage(
                header.DriverId,
                LiveData.ToGridColumn(header.MessageType, SessionType),
                LiveData.ToGridColumnColour(header.Colour));
        }

        private Message ReadClearGridColumnValueMessage(LiveMessageHeader header)
        {
            return new SetGridColumnValueMessage(
                header.DriverId,
                LiveData.ToGridColumn(header.MessageType, SessionType),
                LiveData.ToGridColumnColour(header.Colour),
                null);
        }

        private Message ReadSetDriverPositionMessage(LiveMessageHeader header)
        {
            if(header.Value == 0)
            {
                Log.DebugFormat("ignoring clear grid row: {0}", header);
                return Message.Empty;
            }
            return new SetDriverPositionMessage(header.DriverId, header.Value);
        }

        private Message ReadHistoricalPositionMessage(LiveMessageHeader header)
        {
            // We ignore historical position updates as it should be computed by a model.
            ReadAndDecryptBytes(header.Value);
            return Message.Empty;
        }

        private Message ReadWeatherMessage(LiveMessageHeader header)
        {
            ReadAndDecryptBytes(header.DataLength);

            var s = DecodeLatin1(header.DataLength);

            switch(header.Colour)
            {
                case 1:
                    return new SetTrackTemperatureMessage(LiveData.ParseDouble(s));
                case 2:
                    return new SetAirTemperatureMessage(LiveData.ParseDouble(s));
                case 3:
                    return new SetIsWetMessage(s[0] == '1');
                case 4:
                    return new SetWindSpeedMessage(LiveData.ParseDouble(s));
                case 5:
                    return new SetHumidityMessage(LiveData.ParseDouble(s));
                case 6:
                    return new SetAtmosphericPressureMessage(LiveData.ParseDouble(s));
                case 7:
                    return ReadSetWindAngleMessage(s);
                default:
                    Log.ErrorFormat("unsupported weather message: {0}", header);
                    throw Guard.LiveMessageReader_UnsupportedWeatherMessage(header);
            }
        }

        private Message ReadSetWindAngleMessage(string s)
        {
            int angle = LiveData.ParseInt32(s);
            // The feed, as of 2010-03-12, has started to send through wind angles greater than 360.
            if(!SetWindAngleMessage.IsValidAngle(angle))
            {
                Log.WarnFormat("received invalid wind angle: {0}", angle);
                return Message.Empty;
            }
            return new SetWindAngleMessage(angle);
        }

        private Message ReadSetElapsedSessionTimeMessage(LiveMessageHeader header)
        {
            ReadAndDecryptBytes(2);

            int seconds = Buffer[1] << 8 & 0xFF00 | Buffer[0] & 0xFF | header.Value << 16 & 0xFF0000;

            return new SetElapsedSessionTimeMessage(TimeSpan.FromSeconds(seconds));
        }

        private Message ReadSetRemainingSessionTimeMessage(LiveMessageHeader header)
        {
            ReadAndDecryptBytes(header.DataLength);

            var remaining = LiveData.ParseTime(DecodeLatin1(header.DataLength));

            return new CompositeMessage(StopSessionTimeCountdownMessage.Instance,
                new SetRemainingSessionTimeMessage(remaining));
        }

        private Message ReadAddCommentaryMessage(LiveMessageHeader header)
        {
            ReadAndDecryptBytes(header.Value);
            return new AddCommentaryMessage(DecodeUtf8(2, header.Value - 2));
        }

        private Message ReadSpeedCaptureMessage(LiveMessageHeader header)
        {
            ReadAndDecryptBytes(header.Value);
            var location = Buffer[0] - 1;
            if(location < 0 || location > 3)
            {
                // The fastest lap is also published as an apex speed but we ignore them as they
                // can easily be computed by a model.
                return Message.Empty;
            }
            var speeds = new List<KeyValuePair<string, int>>(6);
            foreach(Match match in Regex.Matches(DecodeLatin1(1, header.Value - 1), "([^\r]+)(?:\r)([^\r]+)(?:\r)?"))
            {
                speeds.Add(new KeyValuePair<string, int>(match.Groups[1].Value, LiveData.ParseInt32(match.Groups[2].Value)));
            }
            return new SpeedCaptureMessage((SpeedCaptureLocation)location, speeds.ToArray());
        }

        private Message ReadSetSessionTypeMessage(LiveMessageHeader header)
        {
            ReadBytes(header.DataLength);
            return new SetSessionTypeMessage(LiveData.ToSessionType(header.Colour), DecodeLatin1(1, header.DataLength - 1));
        }

        private Message ReadSetSessionStatusMessage(LiveMessageHeader header)
        {
            ReadAndDecryptBytes(header.DataLength);
            return new SetSessionStatusMessage(LiveData.ToSessionStatus(DecodeLatin1(header.DataLength)));
        }

        private Message ReadSetMinRequiredQuallyTimeMessage(LiveMessageHeader header)
        {
            if(header.DataLength == 0)
            {
                return Message.Empty;
            }
            ReadAndDecryptBytes(header.DataLength);
            return new SetMinRequiredQuallyTimeMessage(LiveData.ParseTime(DecodeLatin1(header.DataLength)));
        }

        private Message ReadSetKeyframeMessage(LiveMessageHeader header)
        {
            if(header.DataLength != 2)
            {
                Log.ErrorFormat("invalid keyframe data length: {0}", header.DataLength);
                throw Guard.MessageReader_InvalidMessage();
            }
            ReadBytes(header.DataLength);
            return new SetKeyframeMessage(Buffer[1] << 8 & 0xFF00 | Buffer[0] & 0xFF);
        }

        private static Message ReadSetStreamValidityMessage(LiveMessageHeader header)
        {
            return new SetStreamValidityMessage(header.Colour != 0);
        }

        private static Message ReadRefrehRateMessage(LiveMessageHeader header)
        {
            return new SetPingIntervalMessage(new TimeSpan(0, 0, header.Value));
        }

        private Message ReadSetSystemMessageMessage(LiveMessageHeader header)
        {
            ReadAndDecryptBytes(header.Value);
            return new SetSystemMessageMessage(DecodeUtf8(header.Value));
        }

        private Message ReadSetCopyrightMessage(LiveMessageHeader header)
        {
            ReadBytes(header.Value);
            return new SetCopyrightMessage(DecodeUtf8(header.Value));
        }

        private IDisposable StreamAndBufferBackup()
        {
            var buffer = Buffer;
            var stream = MessageStream;

            Buffer = CreateBuffer();
            return new DisposableCallback(() =>
            {
                Buffer = buffer;
                MessageStream = stream;
            });
        }

        private static byte[] CreateBuffer()
        {
            return new byte[BufferSize];
        }

        private LiveMessageHeader ReadHeader()
        {
            ReadBytes(2);

            int b0 = Buffer[0];
            int b1 = Buffer[1];

            return new LiveMessageHeader
            {
                DriverId = (byte)(b0 & 0x1F),
                MessageType = (byte)((b0 & 0xE0) >> 5 & 0x7 | (b1 & 0x1) << 3),
                Colour = (byte)((b1 & 0xE) >> 1),
                DataLength = (byte)((b1 & 0xF0) >> 4),
                Value = (byte)((b1 & 0xFE) >> 1)
            };
        }

        private void ReadBytes(int count)
        {
            if(!MessageStream.Fill(Buffer, 0, count))
            {
                throw Guard.LiveMessageReader_UnexpectedEndOfStream();
            }
        }

        private void ReadAndDecryptBytes(int count)
        {
            ReadBytes(count);
            Decrypter.Decrypt(Buffer, 0, count);
        }

        private string DecodeUtf8(int count)
        {
            return DecodeUtf8(0, count);
        }

        private string DecodeUtf8(int offset, int count)
        {
            return Utf8Encoding.GetString(Buffer, offset, count);
        }

        private string DecodeLatin1(int count)
        {
            return DecodeLatin1(0, count);
        }

        private string DecodeLatin1(int offset, int count)
        {
            return Latin1Encoding.GetString(Buffer, offset, count);
        }

        private byte[] Buffer { get; set; }

        private Queue<Message> QueuedMessages { get; set; }

        private IMessageStreamEndpoint MessageStreamEndpoint { get; set; }

        private IMessageProcessor StateEngine { get; set; }

        private LiveMessageTranslator MessageTranslator { get; set; }

        #endregion
    }
}