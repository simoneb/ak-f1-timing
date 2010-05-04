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

namespace AK.F1.Timing.Model.Driver
{
    /// <summary>
    /// Contains information relating to a pit time.
    /// </summary>
    [Serializable]
    public class PitTimeModel
    {       
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="PitTimeModel"/> class.
        /// </summary>
        /// <param name="time">The pit time. Note: this time includes the time taken to travel
        /// the pit lane.</param>
        /// <param name="lapNumber">The lap number on which the pit time was set.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="time"/> or <paramref name="lapNumber"/> is negative.
        /// </exception>
        public PitTimeModel(TimeSpan time, int lapNumber) {

            Guard.InRange(time >= TimeSpan.Zero, "time");
            Guard.InRange(lapNumber >= 0, "lapNumber");

            Time = time;
            LapNumber = lapNumber;
        }

        /// <inheritdoc/>        
        public override string ToString() {

            return string.Format("LapNumber={0}, Time='{1}'", LapNumber, Time);
        }

        /// <summary>
        /// Gets the time spent in the pit. Note: this time includes the time taken to travel
        /// the pit lane.
        /// </summary>
        public TimeSpan Time { get; private set; }

        /// <summary>
        /// Gets the lap number on which the pit time was set.
        /// </summary>
        public int LapNumber { get; private set; }

        #endregion
    }
}
