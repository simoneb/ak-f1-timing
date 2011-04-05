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

using System.IO;
using AK.F1.Timing.Serialization;

namespace AK.F1.Timing.Recording
{
    /// <summary>
    /// Provides An <see cref="AK.F1.Timing.Recording.IRecordedMessageReader"/> which reads
    /// <see cref="AK.F1.Timing.Message"/>s which have been recorded by a
    /// <see cref="AK.F1.Timing.Recording.RecordingMessageReader"/>.
    /// </summary>
    public class RecordedMessageReader : MessageReaderBase, IRecordedMessageReader
    {
        #region Private Fields.

        private double _playbackSpeed = 1d;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="RecordedMessageReader"/> class and specifies
        /// the input file <paramref name="path"/>.
        /// </summary>
        /// <param name="path">The input file path.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="path"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.IO.IOException">
        /// Thrown when an IO error occurs whilst creating the internal
        /// <see cref="System.IO.FileStream"/> using the supplied arguments.
        /// </exception>
        public RecordedMessageReader(string path)
        {
            Initialise(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read), true);
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="RecordedMessageReader"/> class and specifies
        /// the <paramref name="input"/> stream and a value indicating if the stream is owned by the
        /// reader.
        /// </summary>
        /// <param name="input">The input stream.</param>
        /// <param name="ownsInput"><see langword="true"/> if the reader owns the specified
        /// <paramref name="input"/> stream, otherwise; <see langword="false"/>.</param>
        public RecordedMessageReader(Stream input, bool ownsInput)
        {
            Guard.NotNull(input, "output");

            Initialise(input, ownsInput);
        }

        /// <inheritdoc/>
        public double PlaybackSpeed
        {
            get { return _playbackSpeed; }
            set
            {
                Guard.InRange(value > 0d, "value");
                _playbackSpeed = value;
            }
        }

        #endregion

        /// <inheritdoc/>
        protected override Message ReadImpl()
        {
            Message message;

            if((message = Reader.Read<Message>()) != null)
            {
                if(DelayEngine.Process(message))
                {
                    // The engine processed a delay message, let our base class handle the re-read.
                    // See MessageReaderBase#Read.
                    message = Message.Empty;
                }
            }

            return message;
        }

        /// <inheritdoc/>
        protected override void DisposeOfManagedResources()
        {
            DisposeOf(Reader);
            if(OwnsInput)
            {
                DisposeOf(Input);
            }
        }

        #region Private Impl.

        private void Initialise(Stream input, bool ownsInput)
        {
            Input = input;
            OwnsInput = ownsInput;
            Reader = new DecoratedObjectReader(input);
            DelayEngine = new RecordedMessageDelayEngine(this);
        }

        private Stream Input { get; set; }

        private IObjectReader Reader { get; set; }

        private bool OwnsInput { get; set; }

        private RecordedMessageDelayEngine DelayEngine { get; set; }

        #endregion
    }
}