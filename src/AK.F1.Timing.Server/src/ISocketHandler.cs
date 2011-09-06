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
using System.Net.Sockets;

namespace AK.F1.Timing.Server
{
    /// <summary>
    /// Defines a <see cref="System.Net.Sockets.Socket"/> handler.
    /// </summary>
    public interface ISocketHandler : IDisposable
    {
        /// <summary>
        /// Handles the connection as represented by the specified <paramref name="client"/>
        /// <see cref="System.Net.Sockets.Socket"/>.
        /// </summary>
        /// <param name="client">The client socket.</param>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the handler has been disposed of.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="client"/> is <see langword="null"/>.
        /// </exception>
        void Handle(Socket client);
    }
}
