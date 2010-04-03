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

            e.Handled = true;
            this.IgnoreScreenPropertyChanges = true;
            this.Screen.Password = ((PasswordBox)e.Source).Password;            
            this.IgnoreScreenPropertyChanges = false;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {            

            if(this.Screen != null) {
                this.Screen.PropertyChanged -= OnScreenPropertyChanged;
            }
            if(e.NewValue != null) {
                this.Screen = (IHomeScreen)e.NewValue;
                this.Screen.PropertyChanged += OnScreenPropertyChanged;
                this.Password.Password = this.Screen.Password;
            }
        }

        private void OnScreenPropertyChanged(object sender, PropertyChangedEventArgs e) {

            if(!this.IgnoreScreenPropertyChanges && e.PropertyName.Equals("Password", StringComparison.Ordinal)) {
                this.Password.Password = this.Screen.Password;
            }
        }

        private IHomeScreen Screen { get; set; }

        private bool IgnoreScreenPropertyChanges { get; set; }

        #endregion
    }
}
