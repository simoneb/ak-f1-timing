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

using AK.F1.Timing.Messages.Driver;

namespace AK.F1.Timing.Model.Driver
{
    /// <summary>
    /// Defines the times posted during a complete lap.
    /// </summary>
    [Serializable]
    public class LapHistoryEntry
    {
        #region Public Interface.

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s1">The sector one time set during the lap.</param>
        /// <param name="s2">The sector two time set during the lap.</param>
        /// <param name="s3">The sector three time set during the lap.</param>
        /// <param name="lap">The overall lap time set</param>
        /// <param name="pitted"><see langword="true"/> if the driver pitted at the end of the lap,
        /// otherwise; <see langword="false"/>.</param>
        public LapHistoryEntry(PostedTime s1, PostedTime s2, PostedTime s3, PostedTime lap, bool pitted) {

            S1 = s1;
            S2 = s2;
            S3 = s3;
            Lap = lap;
            Pitted = pitted;
        }

        /// <summary>
        /// Gets the lap number of this entry.
        /// </summary>
        public int LapNumber {

            get { return (S1 ?? S2 ?? S3).LapNumber; }
        }

        /// <summary>
        /// Gets the sector one time set during the lap.
        /// </summary>
        public PostedTime S1 { get; private set; }

        /// <summary>
        /// Gets the sector two time set during the lap.
        /// </summary>
        public PostedTime S2 { get; private set; }

        /// <summary>
        /// Gets the sector three time set during the lap.
        /// </summary>
        public PostedTime S3 { get; private set; }

        /// <summary>
        /// Gets the overall lap time set. This can be null if the driver pitted.
        /// </summary>
        public PostedTime Lap { get; private set; }

        /// <summary>
        /// Gets a value indicating if the driver pitted during this lap.
        /// </summary>
        public bool Pitted { get; private set; }

        #endregion
    }
}
