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
using AK.F1.Timing.Messages.Feed;
using AK.F1.Timing.Messages.Session;

namespace AK.F1.Timing.Live
{
    /// <summary>
    /// An engine which updates the state information maintained by a
    /// <see cref="LiveMessageReader"/>. This class cannot be inherited.
    /// </summary>
    [Serializable]
    internal sealed class LiveMessageReaderStateEngine : MessageVisitorBase, IMessageProcessor
    {
        #region Private Fields.

        private readonly LiveMessageReader _reader;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="LiveMessageReaderStateEngine"/> class
        /// and specified the <paramref name="reader"/> to update.
        /// </summary>
        /// <param name="reader">The reader to update.</param>
        public LiveMessageReaderStateEngine(LiveMessageReader reader)
        {
            _reader = reader;
        }

        /// <inheritdoc />
        public void Process(Message message)
        {
            message.Accept(this);
        }

        /// <summary>
        /// Updates the ping internal on the current message reader.
        /// </summary>
        /// <param name="message">The message.</param>
        public override void Visit(SetPingIntervalMessage message)
        {
            var interval = TimeSpan.FromMilliseconds(250d);
            // I am not sure this the correct location for this logic but the quicker we ping the
            // message stream the quicker we get pushed the data.
            if(message.PingInterval < interval)
            {
                interval = message.PingInterval;
            }
            _reader.MessageStream.PingInterval = interval;
        }

        /// <summary>
        /// Updates the reader state to <see cref="LiveMessageReaderState.Closed"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        public override void Visit(EndOfSessionMessage message)
        {
            _reader.DisposeOfMessageStream();
            _reader.State = LiveMessageReaderState.Closed;
        }

        /// <summary>
        /// Sets the current reader session type and creates a new decrypter for the session
        /// identifier specified by the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public override void Visit(SetSessionTypeMessage message)
        {
            _reader.SessionType = message.SessionType;
            _reader.Decrypter = _reader.DecrypterFactory.Create(message.SessionId);
        }

        /// <summary>
        /// Resets the current decrypter.
        /// </summary>
        /// <param name="message">The message.</param>
        public override void Visit(SetKeyframeMessage message)
        {
            _reader.Decrypter.Reset();
        }

        #endregion
    }
}