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
using Caliburn.Core.Metadata;
using Caliburn.PresentationFramework.ApplicationModel;

namespace AK.F1.Timing.UI.Presenters
{
    /// <summary>
    /// The <see cref="AK.F1.Timing.UI.Presenters.IHomePresenter"/>.
    /// </summary>
    [PerRequest(typeof(IHomePresenter))]
    public class HomePresenter : Presenter, IHomePresenter
    {
        #region Fields.

        private string _email;
        private string _password;
        private readonly IShellPresenter _shellPresenter;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="HomePresenter"/> class.
        /// </summary>
        /// <param name="shellPresenter">The
        /// <see cref="AK.F1.Timing.UI.Presenters.IShellPresenter"/>.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="shellPresenter"/> is <see langword="null"/>.
        /// </exception>
        public HomePresenter(IShellPresenter shellPresenter) {

            Guard.NotNull(shellPresenter, "shellPresenter");

            _shellPresenter = shellPresenter;
        }

        /// <inheritdoc/>
        public void Login() {


        }

        /// <inheritdoc/>
        public void Exit() {

            _shellPresenter.Exit();
        }

        /// <inheritdoc/>
        public string Email {

            get { return _email; }
            set {
                _email = value;
                NotifyOfPropertyChange("Email");
            }
        }

        /// <inheritdoc/>
        public string Password {

            get { return _password; }
            set {
                _password = value;
                NotifyOfPropertyChange("Password");
            }
        }

        #endregion

        #region Protected Interface.

        /// <inheritdoc/>
        protected override void OnActivate() {

            base.OnActivate();

            this.DisplayName = "Home";
            this.Email = "andrew.kernahan@gmail.com";
            this.Password = "password";
        }

        #endregion
    }
}
