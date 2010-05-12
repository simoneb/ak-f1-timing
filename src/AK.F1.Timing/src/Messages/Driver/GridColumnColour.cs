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
    /// Defines the various colours of an F1 timing grid
    /// <see cref="AK.F1.Timing.Messages.Driver.GridColumn"/>.
    /// </summary>
    [Serializable]
    public enum GridColumnColour
    {
        /// <summary>
        /// Black.
        /// </summary>
        Black,
        /// <summary>
        /// White.
        /// </summary>
        White,
        /// <summary>
        /// Red.
        /// </summary>
        Red,
        /// <summary>
        /// Green.
        /// </summary>
        Green,
        /// <summary>
        /// Magenta.
        /// </summary>
        Magenta,
        /// <summary>
        /// Blue.
        /// </summary>
        Blue,
        /// <summary>
        /// Yellow.
        /// </summary>
        Yellow,
        /// <summary>
        /// Grey.
        /// </summary>
        Grey
    }
}