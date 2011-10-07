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

using System;
using AK.F1.Timing.Messages;
using AK.F1.Timing.Model.Driver;

namespace AK.F1.Timing.Model.Session
{
    /// <summary>
    /// Contains information relating to speed captured during a session. This class cannot be inherited.
    /// </summary>
    [Serializable]
    public sealed class SpeedCaptureModel
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="SpeedCaptureModel"/> class.
        /// </summary>
        /// <param name="driver">The driver which posted the speed.</param>
        /// <param name="location">The capture location.</param>
        /// <param name="speed">The captured speed.</param>        
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="driver"/> is <see langword="null"/>.
        /// </exception>
        public SpeedCaptureModel(DriverModel driver, SpeedCaptureLocation location, int speed)
        {
            Guard.NotNull(driver, "driver");

            Driver = driver;
            Location = location;
            Speed = speed;
        }

        /// <summary>
        /// Gets the driver which posted the speed.
        /// </summary>
        public DriverModel Driver { get; private set; }

        /// <summary>
        /// Gets the capture location.
        /// </summary>
        public SpeedCaptureLocation Location { get; private set; }

        /// <summary>
        /// Gets the captured speed.
        /// </summary>
        public int Speed { get; private set; }

        #endregion
    }
}