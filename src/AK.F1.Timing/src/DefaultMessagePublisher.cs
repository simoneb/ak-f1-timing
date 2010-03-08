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
using System.IO;

using AK.F1.Timing.Utility;

namespace AK.F1.Timing
{
    public class DefaultMessagePublisher : Disposable, IMessagePublisher
    {
        #region Private Fields.

        private log4net.ILog _log;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Occurs when a <see cref="AK.F1.Timing.Message"/> is published.
        /// </summary>
        public event EventHandler<MessagePublishedEventArgs> MessagePublished;

        /// <summary>
        /// Initialises a new instance of the <see cref="DefaultMessagePublisher"/> class and
        /// specifies the underlying <see cref="AK.F1.Timing.IMessageReader"/> from which
        /// <see cref="AK.F1.Timing.Message"/> are read.
        /// </summary>
        /// <param name="reader">The underlying <see cref="AK.F1.Timing.IMessageReader"/> from
        /// which <see cref="AK.F1.Timing.Message"/> are read.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public DefaultMessagePublisher(IMessageReader reader) {

            Guard.NotNull(reader, "reader");

            this.MessageReader = reader;
        }

        /// <summary>
        /// Starts the publising messages.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the reader has been disposed of.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown when this publisher has already been started.
        /// </exception>
        public void Start() {

            CheckDisposed();
        }

        /// <summary>
        /// Stops publising messages.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the reader has been disposed of.
        /// </exception>
        public void Stop() {

            CheckDisposed();
        }

        #endregion

        #region Protected Interface.

        protected virtual void ProcessMessages() {

            while(true) {
                try {
                    ProcessMessage(this.MessageReader.Read());
                } catch(ObjectDisposedException) {
                    this.Log.Warn("reader disposed, stopping");
                    break;
                } catch(EndOfStreamException) {
                    this.Log.Info("end of stream, stopping");
                    break;
                } catch(IOException exc) {
                    this.Log.Info(exc);
                    // TODO do something more useful.
                    throw;
                } catch(FormatException exc) {
                    this.Log.Info(exc);
                    // TODO do something more useful.
                    throw;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        protected virtual void ProcessMessage(Message message) {

            Guard.NotNull(message, "message");

            this.Log.InfoFormat("publising message {0}", message);

            this.MessagePublished.RaiseAsync(this, new MessagePublishedEventArgs(message));
        }

        /// <summary>
        /// Gets the underlying <see cref="AK.F1.Timing.IMessageReader"/> from which
        /// <see cref="AK.F1.Timing.Message"/> are read.
        /// </summary>
        protected IMessageReader MessageReader { get; set; }

        /// <summary>
        /// Gets the <see cref="log4net.ILog"/> for this instance.
        /// </summary>
        protected log4net.ILog Log {

            get {
                if(_log == null)
                    _log = log4net.LogManager.GetLogger(GetType());
                return _log;
            }
        }

        #endregion

        #region Private Impl.

        

        #endregion
    }
}
