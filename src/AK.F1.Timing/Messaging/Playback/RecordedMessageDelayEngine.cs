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
using System.Threading;

using AK.F1.Timing.Messaging.Messages.Feed;

namespace AK.F1.Timing.Messaging.Playback
{
    /// <summary>
    /// Provides an engine which causes the current thread to sleep by the amount specified in a
    /// <see cref="AK.F1.Timing.Messaging.Messages.Feed.SetNextMessageDelayMessage"/>. This class
    /// is <see langword="sealed"/>.
    /// </summary>
    [Serializable]
    internal sealed class RecordedMessageDelayEngine : MessageVisitor
    {
        #region Private Fields.

        private static readonly log4net.ILog _log = 
            log4net.LogManager.GetLogger(typeof(RecordedMessageDelayEngine));

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="RecordedMessageDelayEngine"/> class.
        /// </summary>
        /// <param name="reader">The owning reader.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public RecordedMessageDelayEngine(IRecordedMessageReader reader) {

            Guard.NotNull(reader, "reader");

            this.Reader = reader;
        }

        /// <summary>
        /// Processes the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message to process.</param>        
        /// <returns><see langword="true"/> if a delay message was processed, otherwise;
        /// <see langword="false"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="message"/> is <see langword="null"/>.
        /// </exception>
        public bool Process(Message message) {

            Guard.NotNull(message, "message");

            this.Delayed = false;

            message.Accept(this);

            return this.Delayed;
        }

        /// <inheritdoc />
        public override void Visit(SetNextMessageDelayMessage message) {
            
            TimeSpan delay = ScaleDelay(message.Delay);

            if(delay >= TimeSpan.Zero) {
                _log.DebugFormat("delaying for {0}", delay);
                Thread.Sleep(delay);                
            }

            this.Delayed = true;
        }

        #endregion

        #region Private Impl.

        private TimeSpan ScaleDelay(TimeSpan delay) {

            double millis = delay.TotalMilliseconds * 1d / this.Reader.PlaybackSpeed;
            
            return TimeSpan.FromMilliseconds(millis);
        }

        private bool Delayed { get; set; }

        private IRecordedMessageReader Reader { get; set; }

        #endregion
    }
}
