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
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;

using AK.F1.Timing.Messaging.Messages.Feed;
using AK.F1.Timing.Messaging.Serialization;

namespace AK.F1.Timing.Messaging.Playback
{
    /// <summary>
    /// Provides a <see cref="AK.F1.Timing.Messaging.IMessageReader"/> decorator which records
    /// the <see cref="AK.F1.Timing.Messaging.Message"/>s read from an inner
    /// <see cref="AK.F1.Timing.Messaging.IMessageReader"/>.
    /// </summary>
    public class RecordingMessageReader : MessageReaderBase
    {
        #region Private Fields.

        /// <summary>
        /// The minimum delay to insert between messages. Delays smaller than this value are
        /// ignored. This field is <see langword="readonly"/>.
        /// </summary>
        private static readonly TimeSpan MIN_MESSAGE_DELAY = TimeSpan.FromMilliseconds(5);

        private static readonly log4net.ILog _log =
            log4net.LogManager.GetLogger(typeof(RecordingMessageReader));

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="RecordingMessageReader"/> class and
        /// specified the inner message reader and the output file path.
        /// </summary>
        /// <param name="inner">The inner message reader.</param>
        /// <param name="path">The output file path.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="inner"/> or <paramref name="path"/> is
        /// <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.IO.IOException">
        /// Thrown when an IO error occurs whilst creating the internal
        /// <see cref="System.IO.FileStream"/> using the supplied arguments.
        /// </exception>
        public RecordingMessageReader(IMessageReader inner, string path, FileMode mode) {

            Guard.NotNull(inner, "inner");

            Initialise(inner, new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None), true);
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="RecordingMessageReader"/> class and
        /// specified the inner message reader, the output stream and a value indicating if the
        /// decorator owns the output.
        /// </summary>
        /// <param name="inner">The inner message reader.</param>
        /// <param name="output">The output stream.</param>
        /// <param name="ownsOutput"><see langword="true"/> if the decorator owns the specified output
        /// stream, otherwise; <see langword="false"/>.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="inner"/> or <paramref name="output"/> is
        /// <see langword="null"/>.
        /// </exception>
        public RecordingMessageReader(IMessageReader inner, Stream output, bool ownsOutput) {

            Guard.NotNull(inner, "inner");
            Guard.NotNull(output, "output");

            Initialise(inner, output, ownsOutput);
        }

        #endregion

        /// <inheritdoc />
        protected override Message ReadImpl() {

            TimeSpan delay;
            TimeSpan elapsed;
            Message message;

            try {
                if((message = this.Inner.Read()) != null) {
                    if(this.Stopwatch.IsRunning) {
                        elapsed = this.Stopwatch.Elapsed;
                        delay = elapsed - this.LastElapsed;
                        this.LastElapsed = elapsed;
                        if(delay >= MIN_MESSAGE_DELAY) {
                            _log.DebugFormat("inserting delay of {0}", delay);
                            Serialize(new SetNextMessageDelayMessage(delay));                            
                        }
                    } else {
                        this.Stopwatch.Start();
                    }
                    Serialize(message);
                } else {
                    DisposeOfResources();
                }
            } catch {
                DisposeOfResources();
                throw;
            }

            return message;
        }     

        /// <inheritdoc />
        protected override void Dispose(bool disposing) {

            if(disposing && !this.IsDisposed) {
                DisposeOfResources();
            }
            base.Dispose(disposing);
        }

        #region Private Impl.

        private void Initialise(IMessageReader inner, Stream output, bool ownsOutput) {

            this.Inner = inner;
            this.Output = output;
            this.OwnsOutput = ownsOutput;
            this.Writer = new ObjectWriter(output);
            this.Stopwatch = new Stopwatch();            
        }

        private void DisposeOfResources() {            

            DisposeOf(this.Inner);
            this.Inner = null;            
            DisposeOf(this.Writer);
            this.Writer = null;
            if(this.OwnsOutput) {                
                DisposeOf(this.Output);
            }
            this.Output = null;
        }

        private void Serialize(Message message) {

            this.Writer.Write(message);
        }   

        private IMessageReader Inner { get; set; }

        private Stream Output { get; set; }

        private ObjectWriter Writer { get; set; }

        private Stopwatch Stopwatch { get; set; }

        private TimeSpan LastElapsed { get; set; }

        private bool OwnsOutput { get; set; }

        #endregion
    }
}
