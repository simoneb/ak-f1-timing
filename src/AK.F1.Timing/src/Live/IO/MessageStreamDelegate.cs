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
using AK.F1.Timing.Extensions;
using AK.F1.Timing.Utility;

namespace AK.F1.Timing.Live.IO
{
    /// <summary>
    /// A <see cref="AK.F1.Timing.Live.IO.IMessageStream"/> implementation which
    /// delegates to an underlying <see cref="System.IO.Stream"/>. This implementation does
    /// not support pinging. This class cannot be inherited.
    /// </summary>
    public sealed class MessageStreamDelegate : DisposableBase, IMessageStream
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="MessageStreamDelegate"/> class.
        /// </summary>
        /// <param name="inner">The inner message stream.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="inner"/> is <see langword="null"/>.
        /// </exception>
        public MessageStreamDelegate(Stream inner)
        {
            Guard.NotNull(inner, "inner");

            Inner = inner;
        }

        /// <inheritdoc/>
        public bool Fill(byte[] buffer, int offset, int count)
        {
            CheckDisposed();

            return Inner.Fill(buffer, offset, count);
        }

        /// <inheritdoc/>
        public TimeSpan PingInterval { get; set; }

        #endregion

        #region Protected Interface.

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if(disposing && !IsDisposed)
            {
                DisposeOf(Inner);
                Inner = null;
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Private Impl.

        private Stream Inner { get; set; }

        #endregion
    }
}