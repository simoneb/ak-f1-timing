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

using System.Globalization;
using System.IO;
using System.Text;
using AK.F1.Timing.Live.IO;
using log4net;

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

        private const string StreamFileName = "stream.bin";
        private const string KeyframeFileName = "keyframe";
        private const string KeyframeFileExt = ".bin";

        private static readonly ILog Log = LogManager.GetLogger(typeof(RecordedMessageStreamEndpoint));

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
        public RecordedMessageStreamEndpoint(string directory)
        {
            Guard.NotNull(directory, "directory");
            Guard.DirectoryExists(directory, "directory");

            Directory = directory;
            StreamFilePath = Path.Combine(directory, StreamFileName);
        }

        /// <inheritdoc/>
        public IMessageStream Open()
        {
            Log.DebugFormat("opening stream {0}", StreamFilePath);

            FileStream fileStream = File.Open(StreamFilePath, FileMode.Open,
                FileAccess.Read, FileShare.None);

            return new MessageStreamDelegate(fileStream);
        }

        /// <inheritdoc/>
        public IMessageStream OpenKeyframe(int keyframe)
        {
            Guard.InRange(keyframe >= 0, "keyframe");

            FileStream stream;
            string keyframePath = BuildKeyframePath(keyframe);

            Log.InfoFormat("opening keyframe {0}", keyframePath);
            stream = File.Open(keyframePath, FileMode.Open, FileAccess.Read, FileShare.None);
            Log.InfoFormat("opened keyframe, length: {0}", stream.Length);

            return new MessageStreamDelegate(stream);
        }

        #endregion

        #region Private Impl.

        private string BuildKeyframePath(int keyframe)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(KeyframeFileName);
            var invCulture = CultureInfo.InvariantCulture;
            if(keyframe == 0)
            {
                if(KeyframeCount > 0)
                {
                    sb.AppendFormat(invCulture, "_{0}", KeyframeCount);
                }
                sb.Append(KeyframeFileExt);
                ++KeyframeCount;
            }
            else
            {
                sb.Append("_").AppendFormat(invCulture, "{0:00000}", keyframe);
                sb.Append(KeyframeFileExt);
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