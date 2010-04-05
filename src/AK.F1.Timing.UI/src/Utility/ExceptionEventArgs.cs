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

namespace AK.F1.Timing.UI.Utility
{
    /// <summary>
    /// Contains the exception raised by an event.
    /// </summary>
    [Serializable]
    public class ExceptionEventArgs : EventArgs
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="ExceptionEventArgs"/> class and specifies
        /// the <paramref name="exception"/>.
        /// </summary>
        /// <param name="exception">The <see cref="System.Exception"/>.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="exception"/> is <see langword="null"/>.
        /// </exception>
        public ExceptionEventArgs(Exception exception) {

            Guard.NotNull(exception, "exception");

            Exception = exception;
        }

        /// <summary>
        /// Gets the <see cref="System.Exception"/>.
        /// </summary>
        public Exception Exception { get; private set; }

        #endregion
    }
}
