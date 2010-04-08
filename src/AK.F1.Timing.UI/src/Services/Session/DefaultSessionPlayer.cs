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
using System.IO;
using System.Runtime.Serialization;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

using AK.F1.Timing.Model.Session;
using AK.F1.Timing.Recording;
using AK.F1.Timing.UI.Utility;

namespace AK.F1.Timing.UI.Services.Session
{
    /// <summary>
    /// A default <see cref="AK.F1.Timing.UI.Services.Session.ISessionPlayer"/> implementation.
    /// </summary>
    public class DefaultSessionPlayer : ISessionPlayer
    {
        #region Fields.

        private static readonly log4net.ILog _log =
            log4net.LogManager.GetLogger(typeof(DefaultSessionPlayer));

        #endregion

        #region Public Interface.

        /// <inheritdoc/>
        public event EventHandler Started;

        /// <inheritdoc/>
        public event EventHandler Paused;

        /// <inheritdoc/>
        public event EventHandler Stopped;

        /// <inheritdoc/>
        public event EventHandler<ExceptionEventArgs> Exception;

        /// <summary>
        /// Initialises a new instance of the <see cref="DefaultSessionPlayer"/> class.
        /// </summary>
        /// <param name="readerFactory">The message reader factory.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public DefaultSessionPlayer(Func<IMessageReader> readerFactory) {

            Guard.NotNull(readerFactory, "readerFactory");

            ReaderFactory = readerFactory;
            Session = new SessionModel();
            Dispatcher = Application.Current.Dispatcher;
            Worker = new Thread(ReadAndDispatchMessages) {
                IsBackground = true
            };
            CachedDispatchMessageCallback = (Action<Message>)DispatchMessageCallback;
        }

        /// <inheritdoc/>
        public void Start() {

            lock(Worker) {
                // TODO this should throw if worker is not unstarted or running.
                Worker.Start();
            }
        }

        /// <inheritdoc/>
        public void Pause() {

            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public SessionModel Session { get; private set; }

        /// <inheritdoc/>
        public bool SupportsPause { get; set; }

        #endregion

        #region Private Impl.

        private void ReadAndDispatchMessages() {

            Message message;

            try {
                using(var reader = ReaderFactory()) {
                    message = reader.Read();
                    DispatchEvent(Started);
                    if(message != null) {
                        do {
                            DispatchMessage(message);
                        } while((message = reader.Read()) != null);
                    }
                }
            } catch(IOException exc) {
                LogAndDispatchException(exc);
            } catch(SerializationException exc) {
                LogAndDispatchException(exc);
            } catch(Exception exc) {
                _log.Fatal(exc);
                throw;
            }

            DispatchEvent(Stopped);
        }

        private void DispatchMessage(Message message) {

            Dispatcher.BeginInvoke(CachedDispatchMessageCallback, message);
        }

        private void DispatchMessageCallback(Message message) {

            Session.Process(message);
        }

        private void LogAndDispatchException(Exception exc) {

            _log.Error(exc);
            DispatchEvent(Exception, new ExceptionEventArgs(exc));
        }

        private void DispatchEvent(EventHandler @event) {

            if(@event != null) {
                Dispatcher.BeginInvoke(@event, this, EventArgs.Empty);
            }
        }

        private void DispatchEvent<T>(EventHandler<T> @event, T e) where T: EventArgs {

            if(@event != null) {
                Dispatcher.BeginInvoke(@event, this, e);
            }
        }

        private Func<IMessageReader> ReaderFactory { get; set; }

        private Dispatcher Dispatcher { get; set; }

        private Thread Worker { get; set; }

        private Delegate CachedDispatchMessageCallback { get; set; }

        #endregion
    }
}
