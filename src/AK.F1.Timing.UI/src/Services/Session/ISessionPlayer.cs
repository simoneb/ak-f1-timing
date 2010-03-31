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
using System.ComponentModel;

using AK.F1.Timing.Model.Session;

namespace AK.F1.Timing.UI.Services.Session
{
    public interface ISessionPlayer
    {
        /// <summary>
        /// Occurs when the player has started.
        /// </summary>
        event EventHandler Started;

        /// <summary>
        /// Occurs when the player has stopped.
        /// </summary>
        event EventHandler Stopped;

        /// <summary>
        /// Occurs when an exception was thrown whilst playing the session.
        /// </summary>
        event EventHandler<ExceptionEventArgs> Exception;

        /// <summary>
        /// Starts the player.
        /// </summary>
        void Start();

        /// <summary>
        /// Gets the <see cref="AK.F1.Timing.Model.Session.SessionModel"/> being played.
        /// </summary>
        SessionModel Session { get; }
    }
}
