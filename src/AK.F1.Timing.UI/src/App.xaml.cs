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
using Caliburn.PresentationFramework.ApplicationModel;

using AK.F1.Timing.UI.Presenters;

namespace AK.F1.Timing.UI
{
    /// <summary>
    /// The application entry point.
    /// </summary>
    public partial class App : CaliburnApplication
    {
        #region Fields.

        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(App));

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="App"/> class.
        /// </summary>
        public App() {

            InitializeComponent();
        }

        #endregion

        #region Protected Interface.

        /// <inheritdoc/>        
        protected override object CreateRootModel() {

            var binder = (DefaultBinder)this.Container.GetInstance<IBinder>();

            binder.EnableMessageConventions();
            binder.EnableBindingConventions();

            return this.Container.GetInstance<IShellPresenter>();
        }

        #endregion

        #region Private Impl.        

        static App() {

            log4net.Config.XmlConfigurator.Configure();

            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e) {

            // TODO create an exception presenter.
            _log.Error(e.ExceptionObject);
        }

        #endregion
    }
}
