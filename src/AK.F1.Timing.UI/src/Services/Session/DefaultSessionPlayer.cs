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
using AK.F1.Timing.UI.Utility;

namespace AK.F1.Timing.UI.Services.Session
{
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
        public event EventHandler Stopped;

        /// <inheritdoc/>
        public event EventHandler<ExceptionEventArgs> Exception;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageReader"></param>
        public DefaultSessionPlayer(IMessageReader messageReader) {

            Guard.NotNull(messageReader, "messageReader");

            this.Reader = messageReader;
            this.Session = new SessionModel();
            this.Dispatcher = Application.Current.Dispatcher;
            this.Worker = new Thread(ReadAndDispatchMessages);
            this.Worker.IsBackground = true;
            this.CachedDispatchMessageCallback = (Action<Message>)DispatchMessageCallback;
        }

        /// <inheritdoc/>
        public void Start() {

            lock(this.Worker) {
                // TODO this should throw if worker is not unstarted or running.
                this.Worker.Start();
            }
        }

        /// <inheritdoc/>
        public SessionModel Session { get; private set; }

        #endregion

        #region Private Impl.

        private void ReadAndDispatchMessages() {

            Message message;

            try {
                message = this.Reader.Read();
                DispatchEvent(this.Started);
                if(message != null) {
                    do {
                        DispatchMessage(message);
                    } while((message = this.Reader.Read()) != null);
                }
            } catch(IOException exc) {
                LogAndDispatchException(exc);
            } catch(SerializationException exc) {
                LogAndDispatchException(exc);
            } catch(Exception exc) {
                _log.Fatal(exc);
                throw;
            }

            DispatchEvent(this.Stopped);
        }

        private void DispatchMessage(Message message) {

            this.Dispatcher.BeginInvoke(this.CachedDispatchMessageCallback, message);
        }

        private void DispatchMessageCallback(Message message) {

            this.Session.Process(message);
        }

        private void LogAndDispatchException(Exception exc) {

            _log.Error(exc);
            DispatchEvent(this.Exception, new ExceptionEventArgs(exc));
        }

        private void DispatchEvent(EventHandler @event) {

            if(@event != null) {
                this.Dispatcher.BeginInvoke(@event, this, EventArgs.Empty);
            }
        }

        private void DispatchEvent<T>(EventHandler<T> @event, T e) where T: EventArgs {

            if(@event != null) {
                this.Dispatcher.BeginInvoke(@event, this, e);
            }
        }

        private IMessageReader Reader { get; set; }

        private Dispatcher Dispatcher { get; set; }

        private Thread Worker { get; set; }

        private Delegate CachedDispatchMessageCallback { get; set; }

        #endregion
    }
}
