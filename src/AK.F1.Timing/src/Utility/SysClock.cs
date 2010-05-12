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
using System.Diagnostics;

namespace AK.F1.Timing.Utility
{
    /// <summary>
    /// Provides uniform access to the system's clock. This class is <see langword="static"/>.
    /// </summary>
    public static class SysClock
    {
        #region Public Interface.

        /// <summary>
        /// Returns the current system time.
        /// </summary>
        /// <returns>The current system time.</returns>
        public static DateTime Now()
        {
            return DateTime.UtcNow;
        }

        /// <summary>
        /// Gets the current tick count.
        /// </summary>
        /// <returns>The current tick count.</returns>
        public static TimeSpan Ticks()
        {
            return TimeSpan.FromTicks(Stopwatch.GetTimestamp());
        }

        #endregion
    }
}