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
using System.Globalization;
using System.IO;
using System.Text;

using AK.F1.Timing.Live.IO;

namespace AK.F1.Timing.Live.Recording
{
    /// <summary>
    /// An <see cref="AK.F1.Timing.Live.IO.IMessageStreamEndpoint"/> implementation that
    /// opens messages streams that have been persisted on the file system. This class cannot be
    /// inherited.
    /// </summary>
    public sealed class RecordedMessageStreamEndpoint : IMessageStreamEndpoint
    {
        #region Internal Fields.

        private static readonly CultureInfo INV_CULTURE = CultureInfo.InvariantCulture;
        private static readonly log4net.ILog _log =
            log4net.LogManager.GetLogger(typeof(RecordedMessageStreamEndpoint));

        #endregion

        #region Internal Fields.

        internal const string STREAM_FILE_NAME = "stream.bin";
        internal const string KEYFRAME_FILE_NAME = "keyframe";
        internal const string KEYFRAME_FILE_EXT = ".bin";

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="RecordedMessageStreamEndpoint"/> class and
        /// specifies the directory which contains the persisted message streams.
        /// </summary>
        /// <param name="directory">The directory which contains the persisted message streams.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="directory"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.IO.DirectoryNotFoundException">
        /// Thrown when <paramref name="directory"/> does not exist.
        /// </exception>
        public RecordedMessageStreamEndpoint(string directory) {

            Guard.NotNull(directory, "directory");
            Guard.DirectoryExists(directory, "directory");

            Directory = directory;
            StreamFilePath = Path.Combine(directory, STREAM_FILE_NAME);
        }

        /// <inheritdoc />
        public IMessageStream Open() {

            _log.DebugFormat("opening stream {0}", StreamFilePath);

            FileStream fileStream = File.Open(StreamFilePath, FileMode.Open,
                FileAccess.Read, FileShare.None);

            return new MessageStreamDelegate(fileStream);
        }

        /// <inheritdoc />
        public IMessageStream OpenKeyframe(int keyframe) {

            Guard.InRange(keyframe >= 0, "keyframe");

            FileStream stream;
            string keyframePath = BuildKeyframePath(keyframe);

            _log.InfoFormat("opening keyframe {0}", keyframePath);
            stream = File.Open(keyframePath, FileMode.Open, FileAccess.Read, FileShare.None);
            _log.InfoFormat("opened keyframe, length: {0}", stream.Length);

            return new MessageStreamDelegate(stream);
        }

        #endregion

        #region Private Impl.

        private string BuildKeyframePath(int keyframe) {

            StringBuilder sb = new StringBuilder();

            sb.Append(KEYFRAME_FILE_NAME);
            if(keyframe == 0) {
                if(KeyframeCount > 0) {
                    sb.AppendFormat(INV_CULTURE, "_{0}", KeyframeCount);
                }
                sb.Append(KEYFRAME_FILE_EXT);
                ++KeyframeCount;
            } else {
                sb.Append("_").AppendFormat(INV_CULTURE, "{0:00000}", keyframe);
                sb.Append(KEYFRAME_FILE_EXT);
                KeyframeCount = 0;
            }

            return Path.Combine(Directory, sb.ToString());
        }

        private int KeyframeCount { get; set; }

        private string Directory { get; set; }

        private string StreamFilePath { get; set; }

        #endregion
    }
}
