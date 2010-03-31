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
using Microsoft.Win32;
using Caliburn.Core.Metadata;
using Caliburn.PresentationFramework;
using Caliburn.PresentationFramework.Actions;
using Caliburn.PresentationFramework.ApplicationModel;
using Caliburn.PresentationFramework.Filters;

using AK.F1.Timing.UI.Actions;
using AK.F1.Timing.UI.Services.Settings;

namespace AK.F1.Timing.UI.Presenters
{
    /// <summary>
    /// The <see cref="AK.F1.Timing.UI.Presenters.IHomePresenter"/>.
    /// </summary>
    [PerRequest(typeof(IHomePresenter))]
    public class HomePresenter : Presenter, IHomePresenter
    {
        #region Fields.

        private string _username;
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

        public void Playback() {

            var fd = new OpenFileDialog();

            fd.Filter = "Timing Message Store (*.tms)|*.tms";
            if(fd.ShowDialog() != true) {
                return;
            }

            var player = new Services.Session.DefaultSessionPlayer(F1Timing.Playback.Read(fd.FileName));
            var presenter = _shellPresenter.Container.GetInstance<ISessionPresenter>();

            presenter.Player = player;

            _shellPresenter.Open(presenter, delegate { });
        }

        /// <summary>
        /// Logs into the live-timing feed.
        /// </summary>
        [AsyncAction(BlockInteraction = true)]
        public IEnumerable<IResult> Login() {

            this.LoginErrorMessage = null;

            var login = new LiveLoginAction(this.Username, this.Password);

            yield return login;

            if(!login.Success) {
                this.LoginErrorMessage = login.Exception.Message;
                yield break;
            }

            SaveCredentials();

            yield return new WatchLiveSessionAction(_shellPresenter, login.AuthenticationToken);
        }

        /// <summary>
        /// Returns a value indicating if the user can login.
        /// </summary>
        public bool CanLogin {

            get { return this.IsUsernameValid && this.IsPasswordValid; }
        }

        /// <summary>
        /// Gets or sets the user's live-timing username.
        /// </summary>
        public string Username {

            get { return _username; }
            set {
                _username = value;
                NotifyOfPropertyChange("Username");
                NotifyOfPropertyChange("IsUsernameValid");
                NotifyOfPropertyChange("CanLogin");
            }
        }

        /// <summary>
        /// Gets a value indicating if the user's username is valid.
        /// </summary>
        public bool IsUsernameValid {

            get { return !string.IsNullOrEmpty(_username); }
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

        /// <summary>
        /// Gets the current login error message.
        /// </summary>
        public string LoginErrorMessage {

            get { return _loginErrorMessage; }
            private set {
                _loginErrorMessage = value;
                NotifyOfPropertyChange("LoginErrorMessage");
                NotifyOfPropertyChange("HasLoginErrorMessage");
            }
        }

        /// <summary>
        /// Gets a value indicating if there is a login message.
        /// </summary>
        public bool HasLoginErrorMessage  {

            get { return !string.IsNullOrEmpty(_loginErrorMessage); }            
        }

        #endregion

        #region Protected Interface.

        /// <inheritdoc/>
        protected override void OnActivate() {

            base.OnActivate();

            this.Username = _settings.Username;
            this.Password = _settings.Password;
        }

        #endregion

        #region Private Impl.

        private void SaveCredentials() {

            _settings.Username = this.Username;
            _settings.Password = this.Password;
            _settings.Save();
        }

        #endregion
    }
}
