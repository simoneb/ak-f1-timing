// Copyright 2010 Andy Kernahan
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

namespace AK.F1.Timing.Model.Session
{
    public partial class MessageModel
    {
        /// <summary>
        /// An <see cref="AK.F1.Timing.IMessageProcessor"/> which builds a
        /// <see cref="AK.F1.Timing.Model.Session.MessageModel"/> as it processes
        /// <see cref="AK.F1.Timing.Message"/>s. This class cannot be inherited.
        /// </summary>
        [Serializable]
        private sealed class MessageModelBuilder : MessageVisitorBase, IMessageProcessor
        {
            #region Public Interface.

            /// <summary>
            /// Initialises a new instance of the <see cref="MessageModelBuilder"/> class and specifies
            /// the <paramref name="model"/> to build.
            /// </summary>
            /// <param name="model">The model to build.</param>
            /// <exception cref="T:System.ArgumentNullException">
            /// Thrown when <paramref name="model"/> is <see langword="null"/>.
            /// </exception>
            public MessageModelBuilder(MessageModel model)
            {
                Guard.NotNull(model, "model");

                Model = model;
            }

            /// <inheritdoc/>
            public void Process(Message message)
            {
                message.Accept(this);
            }

            /// <inheritdoc/>
            public override void Visit(AddCommentaryMessage message)
            {
                Model.AddCommentary(message.Commentary);
            }

            /// <inheritdoc/>
            public override void Visit(SetSystemMessageMessage message)
            {
                Model.System = message.Message;
            }

            #endregion

            #region Private Impl.

            private MessageModel Model { get; set; }

            #endregion
        }
    }
}