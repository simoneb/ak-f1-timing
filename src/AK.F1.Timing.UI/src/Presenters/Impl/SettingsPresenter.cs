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
using System.Windows;
using Caliburn.Core.Metadata;
using Caliburn.PresentationFramework.ApplicationModel;
using Microsoft.Practices.ServiceLocation;

namespace AK.F1.Timing.UI.Presenters
{
    /// <summary>
    /// The <see cref="AK.F1.Timing.UI.Presenters.ISettingsPresenter"/>
    /// </summary>
    [PerRequest(typeof(ISettingsPresenter))]
    public class SettingsPresenter : Presenter, ISettingsPresenter
    {
        #region Fields.

        private readonly IShellPresenter _shellPresenter;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="SettingsPresenter"/> class.
        /// </summary>
        /// <param name="shellPresenter">The
        /// <see cref="AK.F1.Timing.UI.Presenters.IShellPresenter"/>.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="shellPresenter"/> is <see langword="null"/>.
        /// </exception>
        public SettingsPresenter(IShellPresenter shellPresenter) {

            Guard.NotNull(shellPresenter, "shellPresenter");

            _shellPresenter = shellPresenter;
        }

        /// <summary>
        /// Saves the settings.
        /// </summary>
        public void Save() {            

            
        }

        /// <summary>
        /// Cancels any changes made to the settings and return home.
        /// </summary>
        public void Cancel() {

            
        }

        #endregion

        #region Protected Interface.

        /// <inheritdoc/>
        protected override void OnActivate() {

            base.OnActivate();

            this.DisplayName = "Settings";
        }

        #endregion
    }
}
