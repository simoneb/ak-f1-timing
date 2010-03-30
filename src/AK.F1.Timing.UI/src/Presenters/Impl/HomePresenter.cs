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
using System.Collections.Generic;
using System.IO;
using System.Security.Authentication;
using System.ComponentModel;
using System.Windows;
using Caliburn.Core.Metadata;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.Actions;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Filters;

using AK.F1.Timing.UI.Actions;
using AK.F1.Timing.UI.Settings;

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
        private string _loginErrorMessage;
        private readonly ISettings _settings;
        private readonly IShellPresenter _shellPresenter;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="HomePresenter"/> class.
        /// </summary>
        /// <param name="shellPresenter">The
        /// <see cref="AK.F1.Timing.UI.Presenters.IShellPresenter"/>.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="shellPresenter"/> or <paramref name="settings"/>
        /// is <see langword="null"/>.
        /// </exception>
        public HomePresenter(IShellPresenter shellPresenter, ISettings settings) {

            Guard.NotNull(shellPresenter, "shellPresenter");
            Guard.NotNull(settings, "settings");

            _shellPresenter = shellPresenter;
            _settings = settings;      
        }

        /// <summary>
        /// Logs into the live-timing feed.
        /// </summary>
        [AsyncAction(BlockInteraction = true)]
        public IEnumerable<IResult> Login() {

            this.LoginErrorMessage = null;

            var login = new LoginAction(this.Email, this.Password);

            yield return login;

            if(!login.Success) {
                this.LoginErrorMessage = login.Exception.Message;
                yield break;
            }

            // Save the last known valid credentials.
            _settings.Email = this.Email;
            _settings.Password = this.Password;
            _settings.Save();

            yield return new WatchSessionAction(_shellPresenter, login.AuthenticationToken);
        }

        public bool CanLogin {

            get { return this.IsEmailValid && this.IsPasswordValid; }
        }

        /// <summary>
        /// Gets or sets the user live-timing email address.
        /// </summary>
        public string Email {

            get { return _email; }
            set {
                _email = value;
                NotifyOfPropertyChange("Email");
                NotifyOfPropertyChange("IsEmailValid");
                NotifyOfPropertyChange("CanLogin");
            }
        }

        /// <summary>
        /// Gets a value indicating if the users email address is valid.
        /// </summary>
        public bool IsEmailValid {

            get { return !string.IsNullOrEmpty(_email); }
        }

        /// <summary>
        /// Gets or sets the user live-timing password.
        /// </summary>
        public string Password {

            get { return _password; }
            set {
                _password = value;
                NotifyOfPropertyChange("Password");
                NotifyOfPropertyChange("IsPasswordValid");
                NotifyOfPropertyChange("CanLogin");
            }
        }

        /// <summary>
        /// Gets a value indicating if the users password is valid.
        /// </summary>
        public bool IsPasswordValid {

            get { return !string.IsNullOrEmpty(_password); }
        }

        public string LoginErrorMessage {

            get { return _loginErrorMessage; }
            set {
                _loginErrorMessage = value;
                NotifyOfPropertyChange("LoginErrorMessage");
                NotifyOfPropertyChange("HasLoginErrorMessage");
            }
        }

        public bool HasLoginErrorMessage  {

            get { return !string.IsNullOrEmpty(_loginErrorMessage); }            
        }

        #endregion

        #region Protected Interface.

        /// <inheritdoc/>
        protected override void OnActivate() {

            base.OnActivate();

            this.DisplayName = "Home";            
            this.Email = _settings.Email;
            this.Password = _settings.Password;
        }

        #endregion
    }
}
