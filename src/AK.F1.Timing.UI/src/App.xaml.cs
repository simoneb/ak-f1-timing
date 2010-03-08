// Copyright 2009 Andy Kernahan
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
using System.Windows;
using AK.F1.Timing.UI.ViewModels;
using AK.F1.Timing.UI.Views;

namespace AK.F1.Timing.UI
{
    /// <summary>
    /// The application entry point.
    /// </summary>
    public partial class App : Application
    {
        #region Private Impl.

        static App() {

            log4net.Config.XmlConfigurator.Configure();
        }
        
        private void OnStartup(object sender, StartupEventArgs e) {            

            new MainView() {
                DataContext = new MainViewModel()
            }.Show();
        }

        #endregion
    }
}
