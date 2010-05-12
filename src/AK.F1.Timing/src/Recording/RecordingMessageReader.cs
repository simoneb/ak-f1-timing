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
using AK.F1.Timing.Messages;
using AK.F1.Timing.Messages.Feed;
using AK.F1.Timing.Serialization;

namespace AK.F1.Timing.Recording
{
    /// <summary>
    /// Provides a <see cref="AK.F1.Timing.IMessageReader"/> decorator which records
    /// the <see cref="AK.F1.Timing.Message"/>s read from an inner
    /// <see cref="AK.F1.Timing.IMessageReader"/>.
    /// </summary>
    public class RecordingMessageReader : MessageReaderBase
    {
        #region Private Fields.

        /// <summary>
        /// The minimum delay to insert between messages. Delays smaller than this value are
        /// ignored. This field is <see langword="readonly"/>.
        /// </summary>
        private static readonly TimeSpan MinMessageDelay = TimeSpan.FromMilliseconds(5);

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
        public RecordingMessageReader(IMessageReader inner, string path)
        {
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
        public RecordingMessageReader(IMessageReader inner, Stream output, bool ownsOutput)
        {
            Guard.NotNull(inner, "inner");
            Guard.NotNull(output, "output");

            Initialise(inner, output, ownsOutput);
        }

        #endregion

        /// <inheritdoc />
        protected override Message ReadImpl()
        {
            Message message;

            if((message = Inner.Read()) != null)
            {
                WriteDelay();
                Write(message);
            }
            else
            {
                // An end message delay is not required.
                Write(null);
            }

            return message;
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if(IsDisposed)
            {
                return;
            }
            if(disposing)
            {
                DisposeOf(Writer);
                if(OwnsOutput)
                {
                    DisposeOf(Output);
                }
                DisposeOf(Inner);
            }
            Writer = null;
            Output = null;
            Inner = null;
            base.Dispose(disposing);
        }

        #region Private Impl.

        private void Initialise(IMessageReader inner, Stream output, bool ownsOutput)
        {
            Inner = inner;
            Output = output;
            OwnsOutput = ownsOutput;
            Writer = new DecoratedObjectWriter(output);
            Stopwatch = new Stopwatch();
        }

        private void WriteDelay()
        {
            if(Stopwatch.IsRunning)
            {
                var elapsed = Stopwatch.Elapsed;
                var delay = elapsed - LastElapsed;

                LastElapsed = elapsed;
                if(delay >= MinMessageDelay)
                {
                    Write(new SetNextMessageDelayMessage(delay));
                }
            }
            else
            {
                Stopwatch.Start();
            }
        }

        private void Write(Message message)
        {
            var composite = message as CompositeMessage;

            if(composite != null)
            {
                foreach(var component in composite.Messages)
                {
                    Write(component);
                }
            }
            else
            {
                Writer.Write(message);
            }
        }

        private IMessageReader Inner { get; set; }

        private Stream Output { get; set; }

        private IObjectWriter Writer { get; set; }

        private Stopwatch Stopwatch { get; set; }

        private TimeSpan LastElapsed { get; set; }

        private bool OwnsOutput { get; set; }

        #endregion
    }
}