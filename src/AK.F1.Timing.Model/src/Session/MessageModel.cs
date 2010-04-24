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
    /// A container for messages generated during a timing session.
    /// </summary>
    public partial class MessageModel : ModelBase, IMessageProcessor
    {
        #region Private Fields.

        private string _system;
        private StringBuilder _commentary;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="MessageModel"/> class.
        /// </summary>
        public MessageModel() {

            Builder = new MessageModelBuilder(this);
        }

        /// <inheritdoc/>        
        public void Process(Message message) {

            Guard.NotNull(message, "message");

            Builder.Process(message);
        }

        /// <summary>
        /// Resets this message model.
        /// </summary>
        public void Reset() {

            System = null;
            _commentary = null;
            OnCommentaryChanged();
        }

        /// <summary>
        /// Gets the running commentary.
        /// </summary>
        public string Commentary {

            get { return _commentary != null ? _commentary.ToString() : null; }
        }

        /// <summary>
        /// Gets the system message.
        /// </summary>
        public string System {

            get { return _system; }
            private set { SetProperty("System", ref _system, value); }
        }

        #endregion

        #region Private Impl.

        private void AddCommentary(string commentary) {

            if(_commentary == null) {
                _commentary = new StringBuilder();
            }
            _commentary.Append(commentary);
            // TODO is this correct?
            if(commentary.EndsWith(".")) {
                _commentary.AppendLine().AppendLine();
            }
            OnCommentaryChanged();
        }

        private void OnCommentaryChanged() {

            OnPropertyChanged("Commentary");
        }

        private MessageModelBuilder Builder { get; set; }

        #endregion
    }
}
