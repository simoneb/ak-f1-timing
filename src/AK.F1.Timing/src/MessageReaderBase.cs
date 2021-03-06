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
using AK.F1.Timing.Extensions;
using AK.F1.Timing.Utility;
using log4net;

namespace AK.F1.Timing
{
    /// <summary>
    /// Defines a useful base class for <see cref="AK.F1.Timing.IMessageReader"/>
    /// implementations. This class is <see langword="abstract"/>.
    /// </summary>
    public abstract class MessageReaderBase : DisposableBase, IMessageReader
    {
        #region Private Fields.

        private ILog _log;

        #endregion

        #region Public Interface.

        /// <inheritdoc/>        
        public Message Read()
        {
            CheckDisposed();
            ThrowReadException();

            if(EndOfStreamReached)
            {
                return null;
            }

            Message message;

            try
            {
                while((message = ReadImpl()) == Message.Empty)
                {
                    // Void.
                }
            }
            catch(Exception exc)
            {
                if(!exc.IsFatal())
                {
                    ProcessReadException(exc);
                }
                throw;
            }

            EndOfStreamReached = message == null;

            return message;
        }

        #endregion

        #region Protected Interface.

        /// <summary>
        /// When overriden in a derived class; reads the next <see cref="AK.F1.Timing.Message"/>
        /// from the underlying data stream.
        /// </summary>
        /// <returns>The next <see cref="AK.F1.Timing.Message"/>.</returns>
        /// <remarks>
        /// This method is only invoked when this instance has not been disposed of and
        /// <see cref="MessageReaderBase.ReadException"/> is <see langword="null"/> and a previous
        /// invocation did not return <see langword="null"/>.
        /// </remarks>
        protected abstract Message ReadImpl();

        /// <summary>
        /// Gets the <see cref="log4net.ILog"/> for this type.
        /// </summary>        
        protected ILog Log
        {
            [DebuggerStepThrough]
            get
            {
                if(_log == null)
                {
                    _log = LogManager.GetLogger(GetType());
                }
                return _log;
            }
        }

        #endregion

        #region Private Impl.

        private void ThrowReadException()
        {
            if(ReadException != null)
            {
                throw ReadException;
            }
        }

        private void ProcessReadException(Exception exc)
        {
            Debug.Assert(ReadException == null);

            Log.Error(exc);
            ReadException = exc;
            ReadException.PreserveStackTrace();
        }

        private bool EndOfStreamReached { get; set; }

        private Exception ReadException { get; set; }

        #endregion
    }
}