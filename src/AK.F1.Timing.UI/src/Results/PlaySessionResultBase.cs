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
using Caliburn.PresentationFramework.RoutedMessaging;

using AK.F1.Timing.UI.Screens;
using AK.F1.Timing.UI.Services.Session;

namespace AK.F1.Timing.UI.Results
{
    /// <summary>
    /// Defines the base class for results which play a session. This class is
    /// <see langword="abstract"/>.
    /// </summary>
    public abstract class PlaySessionResultBase : IResult
    {
        #region Public Interface.

        /// <inheritdoc/>
        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

        /// <inheritdoc/>
        public void Execute(ResultExecutionContext context) {

            Guard.NotNull(context, "context");

            var shellScreen = context.ServiceLocator.GetInstance<IShellScreen>();
            var sessionPresenter = context.ServiceLocator.GetInstance<ISessionScreen>();

            sessionPresenter.Player = CreateSessionPlayer();
            shellScreen.OpenScreen(sessionPresenter, delegate { });

            Completed(this, new ResultCompletionEventArgs());
        }

        #endregion

        #region Protected Interface.

        /// <summary>
        /// When overriden in a derived class; creates the
        /// <see cref="AK.F1.Timing.UI.Services.Session.ISessionPlayer"/> used to play the session
        /// specified by the derived action.
        /// </summary>
        /// <returns>The <see cref="AK.F1.Timing.UI.Services.Session.ISessionPlayer"/>.</returns>
        protected abstract ISessionPlayer CreateSessionPlayer();

        #endregion
    }
}
