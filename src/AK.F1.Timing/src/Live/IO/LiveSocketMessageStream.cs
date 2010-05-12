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
using log4net;

namespace AK.F1.Timing.Live.IO
{
    /// <summary>
    /// A <see cref="AK.F1.Timing.Live.IO.IMessageStream"/> implementation which
    /// delegates to an underlying <see cref="System.Net.Sockets.Socket"/>. This implementation
    /// supports pinging. This class cannot be inherited.
    /// </summary>
    public sealed class LiveSocketMessageStream : Disposable, IMessageStream
    {
        #region Private Fields.

        private TimeSpan _pingInterval = TimeSpan.Zero;

        private static readonly byte[] PingPacket = {16};
        private static readonly ILog Log = LogManager.GetLogger(typeof(LiveSocketMessageStream));

        #endregion

        /// <summary>
        /// Initialises a new instance of the <see cref="LiveSocketMessageStream"/> class.
        /// </summary>
        /// <param name="socket">The stream socket.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="socket"/> is <see langword="null"/>.
        /// </exception>
        public LiveSocketMessageStream(Socket socket)
        {
            Guard.NotNull(socket, "socket");

            Socket = socket;
            Socket.NoDelay = true;
            Input = new BufferedStream(new NetworkStream(socket, FileAccess.Read, true));
            PingTimer = new Timer(s => MaybePing());
        }

        /// <inheritdoc />
        public bool FullyRead(byte[] buffer, int offset, int count)
        {
            CheckDisposed();

            if(!Input.FullyRead(buffer, offset, count))
            {
                return false;
            }
            LastRead = SysClock.Ticks();
            return true;
        }

        /// <inheritdoc />
        public TimeSpan PingInterval
        {
            get
            {
                CheckDisposed();
                return _pingInterval;
            }
            set
            {
                CheckDisposed();
                Guard.InRange(value >= TimeSpan.Zero, "value");
                _pingInterval = value;
                ChangePingTimerInterval();
            }
        }

        #region Protected Interface.

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if(disposing && !IsDisposed)
            {
                DisposeOf(PingTimer);
                DisposeOf(Input);
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Private Impl.

        private void ChangePingTimerInterval()
        {
            if(PingInterval > TimeSpan.Zero)
            {
                PingTimer.Change(PingInterval, PingInterval);
            }
            else
            {
                PingTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
            Log.InfoFormat("ping interval set: {0}", PingInterval);
        }

        private void MaybePing()
        {
            try
            {
                if(SysClock.Ticks() - LastRead >= PingInterval)
                {
                    Socket.Send(PingPacket);
                }
            }
            catch(ObjectDisposedException) {}
            catch(IOException exc)
            {
                Log.Info("failed to ping", exc);
            }
        }

        private Socket Socket { get; set; }

        private Stream Input { get; set; }

        private Timer PingTimer { get; set; }

        private TimeSpan LastRead { get; set; }

        #endregion
    }
}