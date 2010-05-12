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
using AK.F1.Timing.Model.Driver;

namespace AK.F1.Timing.Model.Session
{
    /// <summary>
    /// Contains information relating to the a fastest time set in a session. This class is 
    /// <see langword="sealed"/>.
    /// </summary>
    [Serializable]
    public sealed class FastestTimeModel
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="FastestTimeModel"/> class.
        /// </summary>
        /// <param name="driver">The driver which posted the time. Can be <see langword="null"/>.</param>
        /// <param name="time">The time.</param>
        /// <param name="lapNumber">The lap number on which the time was set.</param>
        /// <param name="delta">The delta from the previous fastest time.</param>
        public FastestTimeModel(DriverModel driver, TimeSpan time, int lapNumber, TimeSpan? delta)
        {
            Time = time;
            Delta = delta;
            Driver = driver;
            LapNumber = lapNumber;
        }

        /// <summary>
        /// Gets the time.
        /// </summary>
        public TimeSpan Time { get; private set; }

        /// <summary>
        /// Gets the delta from the previous fastest time.
        /// </summary>
        public TimeSpan? Delta { get; private set; }

        /// <summary>
        /// Gets the driver which posted the time. Can be <see langword="null"/>.
        /// </summary>
        public DriverModel Driver { get; private set; }

        /// <summary>
        /// Gets the lap number on which the time was set.
        /// </summary>
        public int LapNumber { get; private set; }

        #endregion
    }
}