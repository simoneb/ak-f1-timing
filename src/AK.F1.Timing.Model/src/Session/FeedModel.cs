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
using System.Text;

namespace AK.F1.Timing.Model.Session
{
    /// <summary>
    /// 
    /// </summary>
    public class FeedModel : ModelBase
    {
        #region Private Fields.

        private string _copyright;
        private int _messageCount;
        private int _keyframeNumber;
        private TimeSpan _pingInterval;        

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="FeedModel"/> class.
        /// </summary>
        public FeedModel() { }

        /// <summary>
        /// Resets this feed model.
        /// </summary>
        public void Reset() {

            Copyright = null;
            KeyframeNumber = 0;
            MessageCount = 0;
            PingInterval = TimeSpan.Zero;            
        }

        /// <summary>
        /// Gets the current feed ping interval.
        /// </summary>
        public TimeSpan PingInterval {

            get { return _pingInterval; }
            protected internal set { SetProperty("PingInterval", ref _pingInterval, value); }
        }

        /// <summary>
        /// Gets the number of read from the feed.
        /// </summary>
        public int MessageCount {

            get { return _messageCount; }
            protected internal set { SetProperty("MessageCount", ref _messageCount, value); }
        }

        /// <summary>
        /// Gets the current keyframe number.
        /// </summary>
        public int KeyframeNumber {

            get { return _keyframeNumber; }
            protected internal set { SetProperty("KeyframeNumber", ref _keyframeNumber, value); }
        }

        /// <summary>
        /// Gets the copyright message.
        /// </summary>
        public string Copyright {

            get { return _copyright; }
            protected internal set { SetProperty("Copyright", ref _copyright, value); }
        }

        #endregion
    }
}
