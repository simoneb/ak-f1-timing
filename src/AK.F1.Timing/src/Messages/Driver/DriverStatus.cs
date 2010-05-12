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

namespace AK.F1.Timing.Messages.Driver
{
    /// <summary>
    /// Defines the various statuses of a driver during a session.
    /// </summary>
    [Serializable]
    public enum DriverStatus
    {
        /// <summary>
        /// Indicates that the driver is currently out on track.
        /// </summary>
        OnTrack = 0,
        /// <summary>
        /// Indicates that the drivers is currently in the pits.
        /// </summary>
        InPits,
        /// <summary>
        /// Indicates that the driver is out of the current session.
        /// </summary>
        Out,
        /// <summary>
        /// Indicates that driver has stopped out on the track.
        /// </summary>
        Stopped,
        /// <summary>
        /// Indicates that the driver has retired from the race.
        /// </summary>
        Retired,
    }
}