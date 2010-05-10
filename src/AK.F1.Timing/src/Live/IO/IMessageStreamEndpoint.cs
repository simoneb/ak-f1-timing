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

namespace AK.F1.Timing.Live.IO
{
    /// <summary>
    /// Defines the means of opening message
    /// <see cref="AK.F1.Timing.Live.IO.IMessageStream"/>s.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A message stream contains one or more serialized
    /// <see cref="AK.F1.Timing.Message"/>s.
    /// </para>
    /// <para>
    /// Keyframe streams are used to synchronise clients that connect after a live-timing session
    /// has started. The stream contains all the message needed to update clients to the most
    /// current state. Once the keyframe stream has been processed, a normal message stream
    /// is used to receive instantaneous updates.
    /// </para>
    /// </remarks>
    public interface IMessageStreamEndpoint
    {
        /// <summary>
        /// Opens a new message <see cref="AK.F1.Timing.Live.IO.IMessageStream"/>.
        /// </summary>        
        /// <returns>An opened message <see cref="AK.F1.Timing.Live.IO.IMessageStream"/>.</returns>
        /// <exception cref="System.IO.IOException">
        /// Thrown when the message stream could not be opened.
        /// </exception>
        IMessageStream Open();

        /// <summary>
        /// Opens a new keyframe <see cref="AK.F1.Timing.Live.IO.IMessageStream"/> for
        /// the keyframe with the specified <paramref name="keyframe"/> number.
        /// </summary>
        /// <param name="keyframe">The keyframe number.</param>
        /// <returns>A new keyframe <see cref="AK.F1.Timing.Live.IO.IMessageStream"/> for
        /// the keyframe with the specified <paramref name="keyframe"/> number.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="keyframe"/> is negative.
        /// </exception>
        /// <exception cref="System.IO.IOException">
        /// Thrown when the keyframe stream could not be opened.
        /// </exception>
        IMessageStream OpenKeyframe(int keyframe);
    }
}
