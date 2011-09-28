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
using AK.F1.Timing.UI.Results;
using AK.F1.Timing.UI.Services.Settings;
using Caliburn.Core.IoC;
using Caliburn.PresentationFramework.Actions;
using Caliburn.PresentationFramework.RoutedMessaging;
using Caliburn.PresentationFramework.Screens;
using Caliburn.ShellFramework.Results;
using Microsoft.Win32;

namespace AK.F1.Timing.UI.Screens
{
    /// <summary>
    /// The <see cref="AK.F1.Timing.UI.Screens.IHomeScreen"/>.
    /// </summary>
    [PerRequest(typeof(IHomeScreen))]
    public class HomeScreen : Screen, IHomeScreen
    {
        #region Fields.

        private string _username;
        private string _password;
        private string _proxyHostName;
        private string _errorMessage;
        private readonly ISettings _settings;
        private readonly IShellScreen _shellScreen;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="HomeScreen"/> class.
        /// </summary>
        /// <param name="shellScreen">The <see cref="AK.F1.Timing.UI.Screens.IShellScreen"/>.</param>
        /// <param name="settings">The <see cref="AK.F1.Timing.UI.Services.Settings.ISettings"/>.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="shellScreen"/> or <paramref name="settings"/>
        /// is <see langword="null"/>.
        /// </exception>
        public HomeScreen(IShellScreen shellScreen, ISettings settings)
        {
            Guard.NotNull(shellScreen, "shellScreen");
            Guard.NotNull(settings, "settings");

            _shellScreen = shellScreen;
            _settings = settings;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IResult> Playback()
        {
            var fd = new OpenFileDialog
            {
                Filter = "Timing Message Store (*.tms)|*.tms",
                InitialDirectory = Environment.CurrentDirectory
            };

            yield return new ShowCommonDialogResult(fd);
            yield return new PlayRecordedSessionResult(fd.FileName);
        }

        /// <summary>
        /// Reads the live-timing proxy.
        /// </summary>
        [AsyncAction(BlockInteraction = true)]
        public IEnumerable<IResult> ReadProxy()
        {
            ErrorMessage = null;

            var resolveResult = new ResolveHostNameResult(ProxyHostName);

            yield return resolveResult;

            if(!resolveResult.Success)
            {
                ErrorMessage = resolveResult.Exception.Message;
                yield break;
            }

            SaveProxyHostName();

            yield return new PlayProxiedSessionResult(resolveResult.Address);
        }

        /// <summary>
        /// Gets a value indicating if the proxy can be read.
        /// </summary>
        public bool CanReadProxy
        {
            get { return IsProxyEnabled && IsProxyHostNameValid; }
        }

        /// <summary>
        /// Gets a value indicating if the proxy functionality is enabled.
        /// </summary>
        public bool IsProxyEnabled
        {
            get { return true; }
        }

        /// <summary>
        /// Gets or sets the proxy host name.
        /// </summary>
        public string ProxyHostName
        {
            get { return _proxyHostName; }
            set
            {
                _proxyHostName = value;
                NotifyOfPropertyChange(() => IsProxyHostNameValid);
                NotifyOfPropertyChange(() => CanReadProxy);
            }
        }

        /// <summary>
        /// Gets a value indicating if the proxy host name is valid.
        /// </summary>
        public bool IsProxyHostNameValid
        {
            get { return !string.IsNullOrEmpty(_proxyHostName); }
        }

        /// <summary>
        /// Logs into the live-timing feed.
        /// </summary>
        [AsyncAction(BlockInteraction = true)]
        public IEnumerable<IResult> Login()
        {
            ErrorMessage = null;

            var login = new LiveLoginResult(Username, Password);

            yield return login;

            if(!login.Success)
            {
                ErrorMessage = login.Exception.Message;
                yield break;
            }

            SaveCredentials();

            yield return new PlayLiveSessionResult(login.AuthenticationToken);
        }

        /// <summary>
        /// Returns a value indicating if the user can login.
        /// </summary>
        public bool CanLogin
        {
            get { return IsUsernameValid && IsPasswordValid; }
        }

        /// <summary>
        /// Gets or sets the user's live-timing username.
        /// </summary>
        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                NotifyOfPropertyChange(() => Username);
                NotifyOfPropertyChange(() => IsUsernameValid);
                NotifyOfPropertyChange(() => CanLogin);
            }
        }

        /// <summary>
        /// Gets a value indicating if the user's username is valid.
        /// </summary>
        public bool IsUsernameValid
        {
            get { return !string.IsNullOrEmpty(_username); }
        }

        /// <summary>
        /// Gets or sets the user live-timing password.
        /// </summary>
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                NotifyOfPropertyChange(() => Password);
                NotifyOfPropertyChange(() => IsPasswordValid);
                NotifyOfPropertyChange(() => CanLogin);
            }
        }

        /// <summary>
        /// Gets a value indicating if the users password is valid.
        /// </summary>
        public bool IsPasswordValid
        {
            get { return !string.IsNullOrEmpty(_password); }
        }

        /// <summary>
        /// Gets the current error message.
        /// </summary>
        public string ErrorMessage
        {
            get { return _errorMessage; }
            private set
            {
                _errorMessage = value;
                NotifyOfPropertyChange(() => ErrorMessage);
                NotifyOfPropertyChange(() => HasErrorMessage);
            }
        }

        /// <summary>
        /// Gets a value indicating if there is an message.
        /// </summary>
        public bool HasErrorMessage
        {
            get { return !string.IsNullOrEmpty(_errorMessage); }
        }

        #endregion

        #region Protected Interface.

        /// <inheritdoc/>
        protected override void OnActivate()
        {
            Username = _settings.Username;
            Password = _settings.Password;
            ProxyHostName = _settings.ProxyHostName;

            base.OnActivate();
        }

        #endregion

        #region Private Impl.

        private void SaveCredentials()
        {
            _settings.Username = Username;
            _settings.Password = Password;
            _settings.Save();
        }

        private void SaveProxyHostName()
        {
            _settings.ProxyHostName = ProxyHostName;
            _settings.Save();
        }

        #endregion
    }
}