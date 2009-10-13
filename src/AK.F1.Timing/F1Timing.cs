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

using AK.F1.Timing.Messaging;
using AK.F1.Timing.Messaging.Live;
using AK.F1.Timing.Messaging.Live.Recording;
using AK.F1.Timing.Messaging.Live.Encryption;
using AK.F1.Timing.Messaging.Live.IO;
using AK.F1.Timing.Messaging.Playback;

namespace AK.F1.Timing
{
    /// <summary>
    /// Provides a facade interface to the entire <see cref="AK.F1.Timing"/> library. This class is
    /// <see langword="static"/>.
    /// </summary>
    public static class F1Timing
    {
        #region Public Interface.

        /// <summary>
        /// 
        /// </summary>
        public static class Live
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="username">A user's F1 live timing username.</param>
            /// <param name="password">The user's F1 live timing password.</param>
            /// <returns></returns>
            /// <exception cref="System.ArgumentNullException">
            /// Thrown when <paramref name="username"/> or <paramref name="password"/> is 
            /// <see langword="null"/>.
            /// </exception>
            /// <exception cref="System.ArgumentException">
            /// Thrown when <paramref name="username"/> or <paramref name="password"/> is empty.
            /// </exception>
            public static IMessageReader CreateReader(string username, string password) {

                return new LiveMessageReader(
                    new LiveMessageStreamEndpoint(),
                    new LiveDecryptorFactory(username, password));
            }

            public static IMessageReader CreateRecordingReader(string username, string password,
                string path, FileMode mode) {

                return new RecordingMessageReader(CreateReader(username, password), path, mode);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static class Playback
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="directory">The directory which contains the persisted message streams and
            /// encryption seeds.</param>
            /// <returns></returns>
            /// <exception cref="System.ArgumentNullException">
            /// Thrown when <paramref name="directory"/> is <see langword="null"/>.
            /// </exception>
            /// <exception cref="System.IO.DirectoryNotFoundException">
            /// Thrown when <paramref name="directory"/> does not exist.
            /// </exception>
            public static IMessageReader CreateReader(string directory) {

                return new LiveMessageReader(
                    new RecordedMessageStreamEndpoint(directory),
                    new RecordedDecryptorFactory(directory));
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="path">The path of the recorded message stream.</param>
            /// <returns></returns>
            /// <exception cref="System.ArgumentNullException">
            /// Thrown when <paramref name="path"/> is <see langword="null"/>.
            /// </exception>
            /// <exception cref="System.IO.FileNotFoundException">
            /// Thrown when <paramref name="path"/> does not exist.
            /// </exception>
            public static IRecordedMessageReader CreateReader2(string path) {                

                return new AK.F1.Timing.Messaging.Playback.RecordedMessageReader(path);
            }
        }

        #endregion
    }
}
