// Copyright 2011 Andy Kernahan
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using AK.F1.Timing.Extensions;
using AK.F1.Timing.Server.IO;
using AK.F1.Timing.Server.Threading;
using AK.F1.Timing.Utility;
using log4net;

namespace AK.F1.Timing.Server.Proxy
{
    /// <summary>
    /// This class cannot be inherited.
    /// </summary>
    /// <threadsafety static="true" instance="true"/>
    public sealed class ProxySession : DisposableBase
    {
        #region Fields.

        private readonly int _id;
        private readonly Socket _client;
        // 1 KiB is sufficient as the average message length is approximately 20 bytes and message
        // bursts (including composite speed captures) are still relatively small.
        private readonly byte[] _outputBuffer = new byte[1024];
        private readonly SocketAsyncEventArgs _socketOperation = new SocketAsyncEventArgs();
        private readonly AutoResetEventSlim _idleEvent = new AutoResetEventSlim();
        private readonly ManualResetEventSlim _completedEvent = new ManualResetEventSlim();
        private readonly ConcurrentQueue<ByteBufferSnapshot> _bufferQueue = new ConcurrentQueue<ByteBufferSnapshot>();

        private int _partiallySentBufferOffset;
        private ByteBufferSnapshot? _partiallySentBuffer;

        private static readonly ILog Log = LogManager.GetLogger(typeof(ProxySession));

        #endregion

        #region Public Interface.

        /// <summary>
        /// Occurs when this instance has been disposed of.
        /// </summary>
        public event EventHandler Disposed;

        /// <summary>
        /// Initialises a new instance of the <see cref="AK.F1.Timing.Server.Proxy.ProxySession"/>
        /// class and specifies the session identifier and  <paramref name="client"/> socket.
        /// </summary>
        /// <param name="id">The session identifier.</param>
        /// <param name="client">The client <see cref="System.Net.Sockets.Socket"/>.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="client"/> is <see langword="null"/>.
        /// </exception>
        public ProxySession(int id, Socket client)
        {
            Guard.NotNull(client, "client");

            _id = id;
            _client = client;
            _socketOperation.Completed += OnSocketOperationCompleted;
            _socketOperation.SetBuffer(_outputBuffer, 0, 0);
        }

        /// <summary>
        /// Begins an asynchronous operation to send the specified <paramref name="buffers"/>.
        /// </summary>
        /// <param name="buffers">The buffers to send.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="buffers"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the this instance has been disposed of.
        /// </exception>
        public void SendAsync(IEnumerable<byte[]> buffers)
        {
            CheckDisposed();
            Guard.NotNull(buffers, "buffers");

            foreach(var buffer in buffers)
            {
                _bufferQueue.Enqueue(new ByteBufferSnapshot(buffer, 0, buffer.Length));
            }
            SendNextBufferIfNotBusy();
        }

        /// <summary>
        /// Begins an asynchronous operation to send the specified <paramref name="buffer"/>.
        /// </summary>
        /// <param name="buffer">The buffer to send.</param>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the this instance has been disposed of.
        /// </exception>
        public void SendAsync(ByteBufferSnapshot buffer)
        {
            CheckDisposed();
            _bufferQueue.Enqueue(buffer);
            SendNextBufferIfNotBusy();
        }

        /// <summary>
        /// Signals that the session should complete when all pending buffers have been sent.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the this instance has been disposed of.
        /// </exception>
        public void CompleteAsync()
        {
            CheckDisposed();
            _completedEvent.Set();
            if(TrySetBusy())
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Gets the session identifier.
        /// </summary>
        public int Id
        {
            get { return _id; }
        }

        #endregion

        #region Protected Interface.

        /// <inheritdoc/>
        protected override void DisposeOfManagedResources()
        {
            _socketOperation.Completed -= OnSocketOperationCompleted;
            DisposeOf(_socketOperation);
            DisposeOf(_client);
            DisposeOf(_completedEvent);
            DisposeOf(_idleEvent);
            Disposed.RaiseAsync(this);
        }

        #endregion

        #region Private Impl.

        private void SendOutputBuffer(int count)
        {
            Guard.Assert(count > 0);
            _socketOperation.SetBuffer(0, count);
            if(!_client.SendAsync(_socketOperation))
            {
                SendOutputBufferCallback();
            }
        }

        private void SendOutputBufferCallback()
        {
            if(_socketOperation.BytesTransferred == 0 || _socketOperation.SocketError != SocketError.Success)
            {
                Dispose();
            }
            else if(_partiallySentBuffer != null)
            {
                SendNextPartialBufferSegment();
            }
            else
            {
                SendNextBuffer();
            }
        }

        private void SendNextBufferIfNotBusy()
        {
            if(TrySetBusy())
            {
                SendNextBuffer();
            }
        }

        private void SendNextBuffer()
        {
            Guard.Assert(_partiallySentBuffer == null);

            int count = 0;
            ByteBufferSnapshot buffer;
            while(count < _outputBuffer.Length && _bufferQueue.TryDequeue(out buffer))
            {
                int available = _outputBuffer.Length - count;
                if(available >= buffer.Count)
                {
                    buffer.CopyTo(0, _outputBuffer, count, buffer.Count);
                    count += buffer.Count;
                }
                else
                {
                    buffer.CopyTo(0, _outputBuffer, count, available);
                    count += available;
                    _partiallySentBuffer = buffer;
                    _partiallySentBufferOffset = available;
                }
            }
            if(count > 0)
            {
                SendOutputBuffer(count);
            }
            else if(_completedEvent.IsSet)
            {
                Disconnect();
            }
            else
            {
                _idleEvent.Set();
            }
        }

        private void SendNextPartialBufferSegment()
        {
            Guard.Assert(_partiallySentBuffer != null);

            var buffer = _partiallySentBuffer.Value;
            int remaining = buffer.Count - _partiallySentBufferOffset;
            int count = Math.Min(remaining, _outputBuffer.Length);
            buffer.CopyTo(_partiallySentBufferOffset, _outputBuffer, 0, count);
            _partiallySentBufferOffset += count;
            if(_partiallySentBufferOffset == buffer.Count)
            {
                _partiallySentBuffer = null;
            }
            SendOutputBuffer(count);
        }

        private void Disconnect()
        {
            if(!_client.DisconnectAsync(_socketOperation))
            {
                DisconnectCallback();
            }
        }

        private void DisconnectCallback()
        {
            Dispose();
        }

        private void OnSocketOperationCompleted(object sender, SocketAsyncEventArgs e)
        {
            switch(e.LastOperation)
            {
                case SocketAsyncOperation.Disconnect:
                    DisconnectCallback();
                    break;
                case SocketAsyncOperation.Send:
                    SendOutputBufferCallback();
                    break;
                default:
                    Guard.Fail("Invalid LastOperation: " + e.LastOperation);
                    break;
            }
        }

        private bool TrySetBusy()
        {
            return _idleEvent.Wait(0);
        }

        private void Dispose()
        {
            ((IDisposable)this).Dispose();
        }

        #endregion
    }
}
