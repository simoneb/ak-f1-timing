// Copyright 2008 Andy Kernahan
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
using System.Runtime.Serialization;

namespace AK.F1.Timing.Messaging
{
    /// <summary>
    /// The exception that is thrown when the end of a message stream was unexpectedly reached.
    /// </summary>
    [Serializable]
    public class UnexpectedEndOfStreamException : IOException
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="UnexpectedEndOfStreamException"/> class.
        /// </summary>
        public UnexpectedEndOfStreamException()
            : base(Resource.UnexpectedEndOfStreamException_Ctor) {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="UnexpectedEndOfStreamException"/> class and
        /// specifies a message describing the error.
        /// </summary>
        /// <param name="message">A message describing the error.</param>
        public UnexpectedEndOfStreamException(string message)
            : base(message) {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="UnexpectedEndOfStreamException"/> class and
        /// specifies a message describing the error and an inner exception which is the cause of this
        /// exception.
        /// </summary>
        /// <param name="message">A message describing the error.</param>
        /// <param name="innerException">The inner exception which is the cause of this exception.</param>
        public UnexpectedEndOfStreamException(string message, Exception innerException)
            : base(message, innerException) {
        }

        #endregion

        #region Protected Interface.

        /// <summary>
        /// De-serialization constructor.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The serialization context.</param>
        protected UnexpectedEndOfStreamException(SerializationInfo info, StreamingContext context)
            : base(info, context) {
        }

        #endregion
    }
}
