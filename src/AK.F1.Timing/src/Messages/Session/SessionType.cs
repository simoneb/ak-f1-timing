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

namespace AK.F1.Timing.Messages.Session
{
    /// <summary>
    /// Defines the various types of F1 live-timing sessions.
    /// </summary>
    [Serializable]
    public enum SessionType
    {
        /// <summary>
        /// Indicates that there is currently no session.
        /// </summary>
        None,
        /// <summary>
        /// Indicates the practice session.
        /// </summary>
        Practice,
        /// <summary>
        /// Indicates the qualification session.
        /// </summary>
        Qually,
        /// <summary>
        /// Indicates the race session.
        /// </summary>
        Race,
    }
}