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

using System;
using Caliburn.PresentationFramework.Screens;

namespace AK.F1.Timing.UI.Screens
{
    /// <summary>
    /// The <see cref="AK.F1.Timing.UI.Screens.IHomeScreen"/>.
    /// </summary>
    public interface IHomeScreen : IScreenEx
    {
        /// <summary>
        /// Gets or sets the user live-timing password.
        /// </summary>
        string Password { get; set; }
    }
}
