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
using System.Net.Sockets;
using AK.F1.Timing.Utility;
using log4net;

namespace AK.F1.Timing.Live.IO
{
    /// <summary>
    /// An <see cref="AK.F1.Timing.Live.IO.IMessageStream"/> implementation which
    /// delegates to an underlying <see cref="System.Net.Sockets.Socket"/>. This implementation
    /// supports pinging. This class cannot be inherited.
    /// </summary>
    public sealed class LiveSocketMessageStream : DisposableBase, IMessageStream
    {
        #region Private Fields.

        private int _length;
        private int _position;
        private TimeSpan _pingInterval = TimeSpan.Zero;
        private readonly byte[] _buffer = new byte[BufferSize];

        private const int BufferSize = 256;
        private static readonly byte[] PingPacket = { 16 };
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
        }

        /// <inheritdoc/>
        public bool Fill(byte[] buffer, int offset, int count)
        {
            CheckDisposed();
            Guard.CheckBufferArgs(buffer, offset, count);

            try
            {
                return FillCore(buffer, offset, count);
            }
            catch(SocketException exc)
            {
                throw Guard.LiveSocketMessageStream_ReadFailed(exc);
            }
        }

        /// <inheritdoc/>
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
                Log.InfoFormat("ping interval set: {0}", value);
            }
        }

        #region Protected Interface.

        /// <inheritdoc/>
        protected override void DisposeOfManagedResources()
        {
            DisposeOf(Socket);
        }

        #endregion

        #region Private Impl.

        private bool FillCore(byte[] buffer, int offset, int count)
        {
            int available;

            while(count > 0)
            {
                if((available = Math.Min(_length - _position, count)) > 0)
                {
                    Buffer.BlockCopy(_buffer, _position, buffer, offset, available);
                    _position += available;
                    offset += available;
                    count -= available;
                }
                else
                {
                    FillBuffer();
                }
            }

            return true;
        }

        private void FillBuffer()
        {
            Debug.Assert(_position == _length);

            int interval;

            do
            {
                if((interval = MicroSecondPingInterval) == 0)
                {
                    // Wait indefinitely as we are not pinging.
                    interval = -1;
                }
                if(Socket.Poll(interval, SelectMode.SelectRead))
                {
                    _length = Socket.Receive(_buffer);
                    _position = 0;
                    break;
                }
                Ping();
            } while(true);
        }

        private void Ping()
        {
            int sent = 0;

            do
            {
                sent += Socket.Send(PingPacket, sent, PingPacket.Length - sent, SocketFlags.None);
            } while(sent != PingPacket.Length);
        }

        private Socket Socket { get; set; }

        private int MicroSecondPingInterval
        {
            get { return (int)PingInterval.Ticks / 10; }
        }

        #endregion
    }
}