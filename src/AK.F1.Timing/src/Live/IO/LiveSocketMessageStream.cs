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
using System.Net.Sockets;
using System.Threading;

using AK.F1.Timing.Extensions;
using AK.F1.Timing.Utility;

namespace AK.F1.Timing.Messaging.Live.IO
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class LiveSocketMessageStream : Disposable, IMessageStream
    {
        #region Private Fields.

        private TimeSpan _pingInterval = DEFAULT_PING_INTERVAL;

        private const int BUFFER_SIZE = 256;
        private static readonly byte[] MAGIC_PING_PACKET = { 16 };        
        private static readonly TimeSpan DEFAULT_PING_INTERVAL = TimeSpan.FromSeconds(1);

        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(LiveSocketMessageStream));

        #endregion        

        /// <summary>
        /// Initialises a new instance of the <see cref="LiveSocketMessageStream"/> class.
        /// </summary>
        /// <param name="socket">The stream socket.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="socket"/> is <see langword="null"/>.
        /// </exception>
        public LiveSocketMessageStream(Socket socket) {

            Guard.NotNull(socket, "socket");
            
            this.Socket = socket;            
            this.BufferedStream = new BufferedStream(
                new NetworkStream(socket, FileAccess.Read, true), BUFFER_SIZE);
            this.PingTimer = new Timer((state) => MaybePing());
            ChangePingTimerInterval();
        }

        /// <inheritdoc />
        public bool FullyRead(byte[] buffer, int offset, int count) {

            CheckDisposed();

            if(!this.BufferedStream.FullyRead(buffer, offset, count)) {
                return false;
            }
            this.LastRead = SysClock.Now();
            return true;
        }

        /// <inheritdoc />
        public TimeSpan PingInterval {

            get {
                CheckDisposed();
                return _pingInterval;
            }
            set {
                CheckDisposed();
                Guard.InRange(value >= TimeSpan.Zero, "value");                
                _pingInterval = value;
                ChangePingTimerInterval();
            }
        }

        #region Protected Interface.

        /// <inheritdoc />
        protected override void Dispose(bool disposing) {

            if(disposing && !this.IsDisposed) {
                DisposeOf(this.PingTimer);
                DisposeOf(this.BufferedStream);
            }
        }

        #endregion

        #region Private Impl.

        private void ChangePingTimerInterval() {

            this.PingTimer.Change(this.PingInterval, this.PingInterval);
            _log.InfoFormat("ping interval set: {0}", this.PingInterval);
        }

        private void MaybePing() {

            try {
                if(SysClock.Now() - this.LastRead >= this.PingInterval) {                    
                    this.Socket.Send(MAGIC_PING_PACKET);                    
                }
            } catch(ObjectDisposedException) {
            } catch(IOException exc) {
                _log.Info("failed to ping", exc);                
            }
        }

        private Socket Socket { get; set; }

        private BufferedStream BufferedStream { get; set; }

        private Timer PingTimer { get; set; }

        private DateTime LastRead { get; set; }

        #endregion
    }
}
