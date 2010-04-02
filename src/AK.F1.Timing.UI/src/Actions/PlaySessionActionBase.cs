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
using Caliburn.PresentationFramework;
using Microsoft.Practices.ServiceLocation;

using AK.F1.Timing.UI.Presenters;
using AK.F1.Timing.UI.Services.Session;

namespace AK.F1.Timing.UI.Actions
{
    /// <summary>
    /// Defines the base class for actions which play a session. This class is
    /// <see langword="abstract"/>.
    /// </summary>
    public abstract class PlaySessionActionBase : IResult
    {
        /// <summary>
        /// Occurs when this <see cref="Caliburn.PresentationFramework.IResult"/> has completed.
        /// </summary>
        public event Action<IResult, Exception> Completed = delegate { };

        /// <inheritdoc/>
        public void Execute(IRoutedMessageWithOutcome message, IInteractionNode handlingNode) {

            var shellPresenter = this.Container.GetInstance<IShellPresenter>();
            var sessionPresenter = this.Container.GetInstance<ISessionPresenter>();

            sessionPresenter.Player = CreateSessionPlayer();
            shellPresenter.Open(sessionPresenter, delegate { });

            Completed(this, null);
        }

        #region Protected Interface.

        /// <summary>
        /// When overriden in a derived class; creates the
        /// <see cref="AK.F1.Timing.UI.Services.Session.ISessionPlayer"/> used to play the session
        /// specified by the derived action.
        /// </summary>
        /// <returns>The <see cref="AK.F1.Timing.UI.Services.Session.ISessionPlayer"/>.</returns>
        protected abstract ISessionPlayer CreateSessionPlayer();

        /// <summary>
        /// Gets the application's <see cref="Microsoft.Practices.ServiceLocation.IServiceLocator"/>.
        /// </summary>
        protected IServiceLocator Container {

            get { return ServiceLocator.Current; }
        }

        #endregion
    }
}
