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

using AK.F1.Timing.Messaging.Messages.Feed;
using AK.F1.Timing.Messaging.Messages.Session;

namespace AK.F1.Timing.Messaging.Live
{
    /// <summary>
    /// An engine which updates the state information maintained by a
    /// <see cref="LiveMessageReader"/>. This class cannot be inherited.
    /// </summary>
    [Serializable]
    internal sealed class LiveMessageReaderStateEngine : MessageVisitor, IMessageProcessor
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
        public LiveMessageReaderStateEngine(LiveMessageReader reader) {            

            _reader = reader;
        }

        /// <inheritdoc />
        public void Process(Message message) {

            message.Accept(this);
        }

        /// <summary>
        /// Determines if the specified message is terminal and closes the reader's stream if so.
        /// </summary>
        /// <param name="message">The message.</param>
        public override void Visit(SetPingIntervalMessage message) {

            _reader.MessageStream.PingInterval = message.PingInterval;
        }

        /// <summary>
        /// Updates the reader state to <see cref="P:LiveMessageReaderState.Closed"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        public override void Visit(EndOfSessionMessage message) {

            _reader.DisposeOfMessageStream();
            _reader.State = LiveMessageReaderState.Closed;
        }

        /// <summary>
        /// Sets the current reader session type and creates a new decyptor for the session
        /// identifier specified by the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public override void Visit(SetSessionTypeMessage message) {

            _reader.SessionType = message.SessionType;
            _reader.Decryptor = _reader.DecryptorFactory.Create(message.SessionId);            
        }

        /// <summary>
        /// Resets the current decryptor.
        /// </summary>
        /// <param name="message">The message.</param>
        public override void Visit(SetKeyframeMessage message) {

            _reader.Decryptor.Reset();
        }

        #endregion
    }
}
