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
using System.IO;

using AK.F1.Timing.Messaging.Serialization;

namespace AK.F1.Timing.Messaging.Playback
{
    /// <summary>
    /// Provides a <see cref="AK.F1.Timing.Messaging.IMessageReader"/>
    /// </summary>
    public class RecordedMessageReader : MessageReaderBase, IRecordedMessageReader
    {
        #region Private Fields.

        private double _playbackSpeed = 1d;

        #endregion

        #region Public Interface.

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">The input file path.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="path"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.IO.IOException">
        /// Thrown when an IO error occurs whilst creating the internal
        /// <see cref="System.IO.FileStream"/> using the supplied arguments.
        /// </exception>
        public RecordedMessageReader(string path) {

            Initialise(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read), true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="ownsInput"></param>
        public RecordedMessageReader(Stream input, bool ownsInput) {            

            Guard.NotNull(input, "output");

            Initialise(input, ownsInput);
        }

        /// <inheritdoc />
        public double PlaybackSpeed {

            get { return _playbackSpeed; }
            set {
                Guard.InRange(value > 0d, "value");
                _playbackSpeed = value;
            }
        }

        #endregion

        /// <inheritdoc />
        protected override Message ReadImpl() {

            Message message;

            try {
                if((message = (Message)this.Reader.Read()) != null) {
                    if(this.DelayEngine.Process(message)) {
                        // The engine processed a delay message, let our base class handle the re-read.
                        // See MessageReaderBase#Read.
                        message = Message.Empty;
                    }
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

        private void Initialise(Stream input, bool ownsInput) {

            this.Input = input;
            this.OwnsInput = ownsInput;
            this.Reader = new DecoratedObjectReader(input);
            this.DelayEngine = new RecordedMessageDelayEngine(this);
        }

        private void DisposeOfResources() {

            DisposeOf(this.Reader);
            this.Reader = null;
            if(this.OwnsInput) {                
                DisposeOf(this.Input);
            }            
            this.Input = null;
        }

        private Stream Input { get; set; }

        private IObjectReader Reader { get; set; }

        private bool OwnsInput { get; set; }

        private RecordedMessageDelayEngine DelayEngine { get; set; }

        #endregion
    }
}
