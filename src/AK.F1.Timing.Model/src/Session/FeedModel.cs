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

namespace AK.F1.Timing.Model.Session
{
    /// <summary>
    /// 
    /// </summary>
    public partial class FeedModel : ModelBase, IMessageProcessor
    {
        #region Private Fields.

        private string _copyright;
        private int _messageCount;
        private int _keyframeNumber;
        private TimeSpan _pingInterval;
        private DateTime? _lastMessageReceivedOn;
        private DateTime? _timestamp;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="FeedModel"/> class.
        /// </summary>
        public FeedModel()
        {
            Builder = new FeedModelBuilder(this);
        }

        /// <inheritdoc/>        
        public void Process(Message message)
        {
            Guard.NotNull(message, "message");

            Builder.Process(message);
        }

        /// <summary>
        /// Resets this feed model.
        /// </summary>
        public void Reset()
        {
            Copyright = null;
            KeyframeNumber = 0;
            MessageCount = 0;
            PingInterval = TimeSpan.Zero;
            LastMessageReceivedOn = null;
        }

        /// <summary>
        /// Gets the current feed ping interval.
        /// </summary>
        public TimeSpan PingInterval
        {
            get { return _pingInterval; }
            private set { SetProperty("PingInterval", ref _pingInterval, value); }
        }

        /// <summary>
        /// Gets the number of read from the feed.
        /// </summary>
        public int MessageCount
        {
            get { return _messageCount; }
            private set { SetProperty("MessageCount", ref _messageCount, value); }
        }

        /// <summary>
        /// Gets date and time at which the last message was received.
        /// </summary>
        public DateTime? LastMessageReceivedOn
        {
            get { return _lastMessageReceivedOn; }
            private set { SetProperty("LastMessageReceivedOn", ref _lastMessageReceivedOn, value); }
        }

        /// <summary>
        /// Gets stream timestamp.
        /// </summary>
        public DateTime? Timestamp
        {
            get { return _timestamp; }
            private set { SetProperty("Timestamp", ref _timestamp, value); }
        }

        /// <summary>
        /// Gets the current keyframe number.
        /// </summary>
        public int KeyframeNumber
        {
            get { return _keyframeNumber; }
            private set { SetProperty("KeyframeNumber", ref _keyframeNumber, value); }
        }

        /// <summary>
        /// Gets the copyright message.
        /// </summary>
        public string Copyright
        {
            get { return _copyright; }
            private set { SetProperty("Copyright", ref _copyright, value); }
        }

        #endregion

        #region Private Impl.

        private IMessageProcessor Builder { get; set; }

        #endregion
    }
}