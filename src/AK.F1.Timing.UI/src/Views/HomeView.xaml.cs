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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

using AK.F1.Timing.UI.Screens;

namespace AK.F1.Timing.UI.Views
{
    /// <summary>
    /// The application home view.
    /// </summary>
    public partial class HomeView : UserControl
    {
        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="HomeView"/> class.
        /// </summary>
        public HomeView() {

            InitializeComponent();

            this.DataContextChanged += OnDataContextChanged;
            this.Password.PasswordChanged += OnPasswordChanged;
        }

        #endregion

        #region Private Impl.

        private void OnPasswordChanged(object sender, RoutedEventArgs e) {

            this.IgnorePresenterPropertyChanges = true;
            this.Presenter.Password = ((PasswordBox)e.Source).Password;
            this.IgnorePresenterPropertyChanges = false;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {            

            if(this.Presenter != null) {
                this.Presenter.PropertyChanged -= OnPresenterPropertyChanged;
            }
            if(e.NewValue != null) {
                this.Presenter = (IHomeScreen)e.NewValue;
                this.Presenter.PropertyChanged += OnPresenterPropertyChanged;
                this.Password.Password = this.Presenter.Password;
            }
        }

        private void OnPresenterPropertyChanged(object sender, PropertyChangedEventArgs e) {

            if(!this.IgnorePresenterPropertyChanges) {
                if(e.PropertyName.Equals("Password", StringComparison.Ordinal)) {
                    this.Password.Password = this.Presenter.Password;
                }
            }
        }

        private IHomeScreen Presenter { get; set; }

        private bool IgnorePresenterPropertyChanges { get; set; }

        #endregion
    }
}
