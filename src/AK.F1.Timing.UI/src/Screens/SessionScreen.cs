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
using AK.F1.Timing.Model.Driver;
using AK.F1.Timing.Model.Grid;
using AK.F1.Timing.Model.Session;
using AK.F1.Timing.UI.Services.Session;
using AK.F1.Timing.UI.Utility;
using Caliburn.Core.IoC;
using Caliburn.PresentationFramework.Screens;

namespace AK.F1.Timing.UI.Screens
{
    /// <summary>
    /// The <see cref="AK.F1.Timing.UI.Screens.ISessionScreen"/>.
    /// </summary>
    [PerRequest(typeof(ISessionScreen))]
    public class SessionScreen : Screen, ISessionScreen
    {
        #region Fields.

        private ISessionPlayer _player;
        private DriverModel _selectedDriver;
        private GridRowModelBase _selectedGridRow;
        private readonly IShellScreen _shellScreen;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="SessionScreen"/> class.
        /// </summary>
        /// <param name="shellScreen">The
        /// <see cref="AK.F1.Timing.UI.Screens.IShellScreen"/>.</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="shellScreen"/> is <see langword="null"/>.
        /// </exception>
        public SessionScreen(IShellScreen shellScreen)
        {
            Guard.NotNull(shellScreen, "shellScreen");

            _shellScreen = shellScreen;
        }

        /// <inheritdoc/>
        public ISessionPlayer Player
        {
            get { return _player; }
            set
            {
                if(_player != value)
                {
                    if(_player != null)
                    {
                        _player.Exception -= OnPlayerException;
                    }
                    _player = value;
                    _player.Exception += OnPlayerException;
                    NotifyOfPropertyChange(() => Player);
                    NotifyOfPropertyChange(() => Session);
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="AK.F1.Timing.Model.Session.SessionModel"/>
        /// </summary>
        public SessionModel Session
        {
            get { return Player.Session; }
        }

        /// <summary>
        /// Gets or sets the selected grid row.
        /// </summary>
        public GridRowModelBase SelectedGridRow
        {
            get { return _selectedGridRow; }
            set
            {
                if(_selectedGridRow != value)
                {
                    _selectedGridRow = value;
                    NotifyOfPropertyChange(() => SelectedGridRow);
                    SelectedDriver = value != null ? Session.GetDriver(value.Id) : null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected driver.
        /// </summary>
        public DriverModel SelectedDriver
        {
            get { return _selectedDriver; }
            private set
            {
                if(_selectedDriver != value)
                {
                    _selectedDriver = value;
                    NotifyOfPropertyChange(() => SelectedDriver);
                }
            }
        }

        #endregion

        #region Protected Interface.

        /// <inheritdoc/>
        protected override void OnActivate()
        {
            Player.Start();

            base.OnActivate();
        }

        #endregion

        #region Private Impl.

        private void OnPlayerException(object sender, ExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message, e.Exception.GetType().Name,
                MessageBoxButton.OK, MessageBoxImage.Error);
        }

        #endregion
    }
}