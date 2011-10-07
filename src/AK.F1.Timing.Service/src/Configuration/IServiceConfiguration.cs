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

using System.Collections.Generic;

namespace AK.F1.Timing.Service.Configuration
{
    /// <summary>
    /// Provides configuration information for the F1 live-timing service.
    /// </summary>
    public interface IServiceConfiguration : IEnumerable<IRaceConfiguration>
    {
        /// <summary>
        /// Gets the www.formula1.com username of the user to authenticate as.
        /// </summary>        
        string Username { get; }

        /// <summary>
        /// Gets corresponding password.
        /// </summary>        
        string Password { get; }

        /// <summary>
        /// Gets the <see cref="AK.F1.Timing.Service.Configuration.IServerConfiguration"/>.
        /// </summary>        
        IServerConfiguration Server { get; }

        /// <summary>
        /// Gets the <see cref="AK.F1.Timing.Service.Configuration.IRecorderConfiguration"/>.
        /// </summary>        
        IRecorderConfiguration Recorder { get; }
    }
}
