// Copyright 2010 Andy Kernahan
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

using AK.F1.Timing.Model.Driver;

namespace AK.F1.Timing.Model.Session
{
    /// <summary>
    /// Defines the means of locating the <see cref="AK.F1.Timing.Model.Driver.DriverModel"/> for
    /// a given identifier.
    /// </summary>
    public interface IDriverModelLocator
    {
        /// <summary>
        /// Gets the driver with the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The identifier of the driver to get.</param>
        /// <returns>The driver with the speciifed <paramref name="id"/>.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Thrown when <paramref name="id"/> is not positive.
        /// </exception>
        DriverModel GetDriver(int id);
    }
}