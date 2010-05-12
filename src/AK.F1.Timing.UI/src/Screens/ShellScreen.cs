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

using System.Windows;
using AK.F1.Timing.UI.Utility;
using Caliburn.Core.IoC;
using Caliburn.PresentationFramework.Screens;
using Microsoft.Practices.ServiceLocation;

namespace AK.F1.Timing.UI.Screens
{
    /// <summary>
    /// The <see cref="AK.F1.Timing.UI.Screens.IShellScreen"/>.
    /// </summary>
    [Singleton(typeof(IShellScreen))]
    public class ShellScreen : Navigator<IScreen>, IShellScreen
    {
        #region Fields.

        private IScreen _dialogueModel;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="ShellScreen"/> class.
        /// </summary>
        /// <param name="serviceLocator">The
        /// <see cref="Microsoft.Practices.ServiceLocation.IServiceLocator"/>.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="serviceLocator"/> is <see langword="null"/>.
        /// </exception>
        public ShellScreen(IServiceLocator serviceLocator)
        {
            Guard.NotNull(serviceLocator, "serviceLocator");

            Container = serviceLocator;
        }

        /// <inheritdoc/>
        public void Exit()
        {
            Application.Current.Shutdown();
        }

        /// <inheritdoc/>
        public void Open<T>() where T : IScreen
        {
            this.OpenScreen(Container.GetInstance<T>());
        }

        /// <inheritdoc/>
        public void ShowDialogue<T>(T screen) where T : IScreenEx
        {
            Guard.NotNull(screen, "screen");

            screen.WasShutdown += delegate { DialogueModel = null; };
            DialogueModel = screen;
        }

        /// <summary>
        /// Opens the <see cref="AK.F1.Timing.UI.Screens.IHomeScreen"/>
        /// </summary>
        public void OpenHome()
        {
            Open<IHomeScreen>();
        }

        /// <inheritdoc/>
        public IScreen DialogueModel
        {
            get { return _dialogueModel; }
            private set
            {
                _dialogueModel = value;
                NotifyOfPropertyChange(() => DialogueModel);
            }
        }

        /// <summary>
        /// Gets the application's title.
        /// </summary>
        public string ApplicationTitle
        {
            get { return AssemblyInfoHelper.VersionedTitle; }
        }

        #endregion

        #region Protected Interface.

        /// <inheritdoc/>
        protected override void OnActivate()
        {
            base.OnActivate();

            OpenHome();
        }

        #endregion

        #region Private Impl.

        private IServiceLocator Container { get; set; }

        #endregion
    }
}