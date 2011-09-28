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

using Caliburn.Core.IoC;
using Caliburn.PresentationFramework.ApplicationModel;

namespace AK.F1.Timing.UI.Services.Settings
{
    /// <summary>
    /// An isolated storage backed <see cref="AK.F1.Timing.UI.Services.Settings.ISettings"/> implementation.
    /// </summary>
    [Singleton(typeof(ISettings))]
    public class IsolatedStorageSettings : IsolatedStorageStateManager, ISettings
    {
        #region Fields.

        private const string EmailProperty = "Email";
        private const string PasswordProperty = "Password";
        private const string ProxyHostNameProperty = "ProxyHostName";
        private const string FileName = "settings.xml";

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="IsolatedStorageSettings"/> class.
        /// </summary>
        public IsolatedStorageSettings()
        {
            Initialize(FileName);
        }

        /// <inheritdoc/>
        public string Username
        {
            get { return Get(EmailProperty); }
            set
            {
                InsertOrUpdate(EmailProperty, value);
                NotifyOfPropertyChange(EmailProperty);
            }
        }

        /// <inheritdoc/>
        public string Password
        {
            get { return Get(PasswordProperty); }
            set
            {
                InsertOrUpdate(PasswordProperty, value);
                NotifyOfPropertyChange(PasswordProperty);
            }
        }

        /// <inheritdoc/>
        public string ProxyHostName
        {
            get { return Get(ProxyHostNameProperty); }
            set
            {
                InsertOrUpdate(ProxyHostNameProperty, value);
                NotifyOfPropertyChange(ProxyHostNameProperty);
            }
        }

        /// <inheritdoc/>
        public void Save()
        {
            CommitChanges(FileName);
        }

        #endregion
    }
}