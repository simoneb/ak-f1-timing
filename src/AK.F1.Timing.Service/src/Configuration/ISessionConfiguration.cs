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

namespace AK.F1.Timing.Service.Configuration
{
    /// <summary>
    /// Provides configuration information for an individual session.
    /// </summary>
    public interface ISessionConfiguration
    {
        /// <summary>
        /// Gets session identifier.
        /// </summary>        
        string Id { get; }

        /// <summary>
        /// Gets the session name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets session UTC start time.
        /// </summary>        
        DateTime StartTimeUtc { get; }
    }
}
