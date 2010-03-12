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

namespace AK.F1.Timing
{    
    /// <summary>
    /// Allows <see cref="AK.F1.Timing.Message"/>s to be read from an underlying data
    /// stream.
    /// </summary>
    public interface IMessageReader : IDisposable
    {
        /// <summary>
        /// Reads the next <see cref="AK.F1.Timing.Message"/> from the underlying data
        /// stream.
        /// </summary>
        /// <returns>The next <see cref="AK.F1.Timing.Message"/>, or <see langword="null"/>
        /// if the last message has been read.</returns>
        /// <exception cref="System.ObjectDisposedException">
        /// Thrown when the reader has been disposed of.
        /// </exception>
        /// <exception cref="System.IO.IOException">
        /// Thrown when an IO error occurs reading the next message from the stream.
        /// </exception>
        /// <exception cref="System.Runtime.Serialization.SerializationException">
        /// Thrown when the format of the next message of the stream is invalid or the message
        /// itself is not recognised.
        /// </exception>
        Message Read();        
    }
}
