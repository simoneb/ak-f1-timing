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

using System.Globalization;
using System.IO;
using AK.F1.Timing.Playback;

namespace AK.F1.Timing.Service.Jobs
{
    /// <summary>
    /// An <see cref="Quartz.IJob"/> which records the current session. This class cannot be inherited.
    /// </summary>
    public sealed class SessionRecorderJob : SessionJobBase
    {
        #region Protected Interface.

        /// <inheritdoc/>        
        protected override void ExecuteCore()
        {
            var path = Path.GetTempFileName();
            using(var reader = new RecordingMessageReader(CreateReader(), path))
            {
                while(reader.Read() != null) { }
            }
            MoveToTmsDirectory(path);
        }

        #endregion

        #region Private Impl.

        private void MoveToTmsDirectory(string path)
        {
            var tmsPath = GetSessionTmsPath();
            Directory.CreateDirectory(Path.GetDirectoryName(tmsPath));
            File.Move(path, tmsPath);
        }

        private string GetSessionTmsPath()
        {
            return Path.Combine(
                ServiceConfiguration.Recorder.TmsDirectory,
                SessionConfiguration.StartTimeUtc.Year.ToString(CultureInfo.InvariantCulture),
                RaceConfiguration.Id,
                SessionConfiguration.Id + ".tms");
        }

        #endregion
    }
}
