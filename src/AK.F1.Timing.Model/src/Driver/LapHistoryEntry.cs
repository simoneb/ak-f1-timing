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
    /// Contains the times posted during a complete lap. This class cannot be inherited.
    /// </summary>
    [Serializable]
    public sealed class LapHistoryEntry
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="LapHistoryEntry"/> class.
        /// </summary>
        /// <param name="s1">The sector one time set.</param>
        /// <param name="s2">The sector two time set.</param>
        /// <param name="s3">The sector three time set.</param>
        /// <param name="lap">The overall lap time set.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when all arguments are <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when specified times are not all for the same lap.
        /// </exception>
        public LapHistoryEntry(PostedTime s1, PostedTime s2, PostedTime s3, PostedTime lap) {

            CheckOneIsNotNull(s1, s2, s3, lap);
            CheckLapNumbersAreEqual(s1, s2, s3, lap);

            S1 = s1;
            S2 = s2;
            S3 = s3;
            Lap = lap;
        } 

        /// <summary>
        /// Gets the lap number of this entry.
        /// </summary>
        public int LapNumber {

            get { return (S1 ?? S2 ?? S3 ?? Lap).LapNumber; }
        }

        /// <summary>
        /// Gets the sector one time set. Can be <see langword="null"/>.
        /// </summary>
        public PostedTime S1 { get; private set; }

        /// <summary>
        /// Gets the sector two time set. Can be <see langword="null"/>.
        /// </summary>
        public PostedTime S2 { get; private set; }

        /// <summary>
        /// Gets the sector three time set. Can be <see langword="null"/>.
        /// </summary>
        public PostedTime S3 { get; private set; }

        /// <summary>
        /// Gets the lap time set. Can be <see langword="null"/>.
        /// </summary>
        public PostedTime Lap { get; private set; }

        #endregion

        #region Private Impl.

        private void CheckOneIsNotNull(PostedTime s1, PostedTime s2, PostedTime s3, PostedTime lap) {

            if(s1 == null && s2 == null && s3 == null && lap == null) {
                throw Guard.LapHistoryEntry_AtLeastOneTimeMustBeSpecified();                
            }
        }

        private void CheckLapNumbersAreEqual(PostedTime s1, PostedTime s2, PostedTime s3, PostedTime lap) {

            int lapNumber = (s1 ?? s2 ?? s3 ?? lap).LapNumber;

            CheckLapNumber(s1, lapNumber, "s1");
            CheckLapNumber(s2, lapNumber, "s2");
            CheckLapNumber(s3, lapNumber, "s3");
            CheckLapNumber(lap, lapNumber, "lap");
        }

        private static void CheckLapNumber(PostedTime time, int expectedLapNumber, string paramName) {

            if(time != null && time.LapNumber != expectedLapNumber) {
                throw Guard.LapHistoryEntry_AllTimesMustBeForSameLap(paramName);                
            }
        }

        #endregion
    }
}
