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
        public HomeView()
        {
            InitializeComponent();

            DataContextChanged += OnDataContextChanged;
            Password.PasswordChanged += OnPasswordChanged;
        }

        #endregion

        #region Private Impl.

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            IgnoreScreenPropertyChanges = true;
            Screen.Password = ((PasswordBox)e.Source).Password;
            IgnoreScreenPropertyChanges = false;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(Screen != null)
            {
                Screen.PropertyChanged -= OnScreenPropertyChanged;
            }
            if(e.NewValue != null)
            {
                Screen = (IHomeScreen)e.NewValue;
                Screen.PropertyChanged += OnScreenPropertyChanged;
                Password.Password = Screen.Password;                
            }            
            SetFocus();
        }

        private void OnScreenPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(!IgnoreScreenPropertyChanges && e.PropertyName.Equals("Password", StringComparison.Ordinal))
            {
                Password.Password = Screen.Password;
            }
        }

        private void SetFocus()
        {
            if(!IsLoaded)
            {
                Loaded += (s, e) => SetFocus();
                return;
            }
            if(string.IsNullOrEmpty(Email.Text))
            {
                Email.Focus();
            }
            else if(string.IsNullOrEmpty(Password.Password))
            {
                Password.Focus();
            }
            else
            {
                Login.Focus();
            }
        }

        private IHomeScreen Screen { get; set; }

        private bool IgnoreScreenPropertyChanges { get; set; }

        #endregion
    }
}