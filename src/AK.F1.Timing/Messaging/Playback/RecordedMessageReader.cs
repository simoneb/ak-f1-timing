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
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

using AK.F1.Timing.Extensions;

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
        /// <param name="path"></param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="path"/> is <see langword="null"/>.
        /// </exception>
        public RecordedMessageReader(string path) {

            Guard.NotNull(path, "path");

            Initialise(File.OpenRead(path), true);
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

            if(this.Inflater == null) {
                return null;
            }
            
            Message message;

            try {
                message = (Message)this.Formatter.Deserialize(this.Inflater);
                //if(this.DelayEngine.Process(message)) {
                //    // The engine processed a delay message, let our base class handle the re-read.
                //    // see MessageReaderBase#Read.
                //    message = Message.Empty;
                //}
            } catch(SerializationException exc) {
                DisposeOfResources();
                throw Guard.RecordedMessageReader_DeserializationFailed(exc);
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

            InitialiseInput(input, ownsInput);            
            this.Formatter = new BinaryFormatter();
            this.Formatter.TypeFormat = FormatterTypeStyle.TypesWhenNeeded;
            this.Formatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
            this.DelayEngine = new RecordedMessageDelayEngine(this);
        }

        private void InitialiseInput(Stream input, bool ownsInput) {

            this.Input = input;
            this.OwnsInput = ownsInput;
            this.Inflater = new GZipStream(input, CompressionMode.Decompress, true);            
        }

        private void Serialize(Message message) {

            this.Formatter.Serialize(this.Inflater, message);
        }

        private void DisposeOfResources() {

            DisposeOf(this.Inflater);
            this.Inflater = null;
            if(this.OwnsInput) {                
                DisposeOf(this.Input);
            }            
            this.Input = null;
        }        

        private Stream Inflater { get; set; }

        private Stream Input { get; set; }

        private BinaryFormatter Formatter { get; set; }

        private bool OwnsInput { get; set; }

        private RecordedMessageDelayEngine DelayEngine { get; set; }

        #endregion
    }
}
