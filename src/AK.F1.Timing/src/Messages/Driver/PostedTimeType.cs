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
    /// Defines the type of time posted by a driver.
    /// </summary>
    [Serializable]
    public enum PostedTimeType
    {
        /// <summary>
        /// A normal time.
        /// </summary>
        Normal = 0,
        /// <summary>
        /// A personal best time. The time is only relevant in the current session.
        /// </summary>
        PersonalBest = 1,
        /// <summary>
        /// A this best time. The time is only relevant in the current session.
        /// </summary>
        SessionBest = 2
    }
}