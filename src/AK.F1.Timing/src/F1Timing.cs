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

using AK.F1.Timing;
using AK.F1.Timing.Live;
using AK.F1.Timing.Live.Encryption;
using AK.F1.Timing.Live.IO;
using AK.F1.Timing.Recording;

namespace AK.F1.Timing
{
    /// <summary>
    /// Provides a simple interface to the entire <see cref="AK.F1.Timing"/> library. This class is
    /// <see langword="static"/>.
    /// </summary>
    public static class F1Timing
    {
        #region Public Interface.

        /// <summary>
        /// Provides methods for creating <see cref="AK.F1.Timing.IMessageReader"/>s which
        /// read from the live message stream.
        /// </summary>
        public static class Live
        {
            /// <summary>
            /// Creates a live message reader using the specified credentials.
            /// </summary>
            /// <param name="username">The user's F1 live-timing username.</param>
            /// <param name="password">The user's F1 live-timing password.</param>
            /// <returns>A message reader which reads live messages.</returns>
            /// <exception cref="System.ArgumentNullException">
            /// Thrown when <paramref name="username"/> or <paramref name="password"/> is 
            /// <see langword="null"/>.
            /// </exception>
            /// <exception cref="System.ArgumentException">
            /// Thrown when <paramref name="username"/> or <paramref name="password"/> is empty.
            /// </exception>
            /// <exception cref="System.IO.IOException">
            /// Thrown when an IO error whilst connecting to the live messages stream.
            /// </exception>
            /// <exception cref="System.Security.Authentication.AuthenticationException">
            /// Thrown when the supplied credentials were rejected by the live-timing site.
            /// </exception>
            public static IMessageReader Read(string username, string password) {

                return new LiveMessageReader(
                    new LiveMessageStreamEndpoint(),
                    new LiveDecryptorFactory(username, password));
            }

            /// <summary>
            /// Creates a live message reader using the specified credentials and records the
            /// messages to the specified <paramref name="path"/>.
            /// </summary>
            /// <param name="username">The user's F1 live-timing username.</param>
            /// <param name="password">The user's F1 live-timing password.</param>            
            /// <param name="path">The path to save the messages to.</param>
            /// <returns>A message reader which reads and records live messages.</returns>
            /// <exception cref="System.ArgumentNullException">
            /// Thrown when <paramref name="username"/> or <paramref name="password"/> or
            /// <paramref name="path"/> is <see langword="null"/>.
            /// </exception>
            /// <exception cref="System.ArgumentException">
            /// Thrown when <paramref name="username"/> or <paramref name="password"/> or
            /// <paramref name="path"/> is empty.
            /// </exception>
            /// <exception cref="System.IO.IOException">
            /// Thrown when an IO error occurs whilst creating the output file or connecting to the live
            /// messages stream.
            /// </exception>
            /// <exception cref="System.Security.Authentication.AuthenticationException">
            /// Thrown when the supplied credentials were rejected by the live-timing site.
            /// </exception>
            public static IMessageReader ReadAndRecord(string username, string password, string path) {

                return new RecordingMessageReader(Read(username, password), path);
            }
        }

        /// <summary>
        /// Provides methods for creating <see cref="AK.F1.Timing.Recording.IRecordedMessageReader"/>s
        /// which read from persisted live message streams.
        /// </summary>
        public static class Recording
        {
            /// <summary>
            /// Creates a playback reader which reads the messages persisted in the specified file <paramref name="path"/>.
            /// </summary>
            /// <param name="path">The path of the recorded message stream.</param>
            /// <returns></returns>
            /// <exception cref="System.ArgumentNullException">
            /// Thrown when <paramref name="path"/> is <see langword="null"/>.
            /// </exception>
            /// <exception cref="System.IO.FileNotFoundException">
            /// Thrown when <paramref name="path"/> does not exist.
            /// </exception>
            /// <exception cref="System.IO.IOException">
            /// Thrown when an IO error occurs whilst opening the specified <paramref name="path"/>.
            /// </exception>
            public static IRecordedMessageReader Read(string path) {

                return new RecordedMessageReader(path);
            }
        }

        #endregion
    }
}
