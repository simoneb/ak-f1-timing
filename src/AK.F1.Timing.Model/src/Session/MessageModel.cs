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
    public class MessageModel : ModelBase
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

            _commentary = new StringBuilder();
        }

        /// <summary>
        /// Resets this message model.
        /// </summary>
        public void Reset() {

            System = null;
            _commentary.Length = 0;
            OnCommentaryChanged();
        }

        /// <summary>
        /// Adds the specified commentary line to the running commentary.
        /// </summary>
        /// <param name="commentary">The commentary to add.</param>
        public void AddCommentary(string commentary) {

            Guard.NotNullOrEmpty(commentary, "commentary");

            _commentary.Append(commentary);
            // TODO is this correct?
            if(commentary.EndsWith(".")) {
                _commentary.AppendLine().AppendLine();                
            }
            OnCommentaryChanged();
        }     

        /// <summary>
        /// Gets the running commentary.
        /// </summary>
        public string Commentary {

            get { return _commentary.ToString(); }
        }

        /// <summary>
        /// Gets or sets the system message.
        /// </summary>
        public string System {

            get { return _system; }
            set { SetProperty("System", ref _system, value); }
        }

        #endregion

        #region Private Impl.

        private void OnCommentaryChanged() {

            OnPropertyChanged("Commentary");
        }

        #endregion
    }
}
