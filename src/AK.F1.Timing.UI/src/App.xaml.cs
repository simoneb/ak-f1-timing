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
using AK.F1.Timing.UI.Screens;
using AK.F1.Timing.UI.Utility;
using Caliburn.PresentationFramework.ApplicationModel;
using log4net;
using log4net.Config;
using LogManager = Caliburn.Core.Logging.LogManager;

namespace AK.F1.Timing.UI
{
    /// <summary>
    /// The application entry point.
    /// </summary>
    public partial class App : CaliburnApplication
    {
        #region Fields.

        private static readonly ILog Log;

        #endregion

        #region Public Interface.

        /// <summary>
        /// Initialises a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
        {
            InitializeComponent();
        }

        #endregion

        #region Protected Interface.

        /// <inheritdoc/>
        protected override object CreateRootModel()
        {
            return Container.GetInstance<IShellScreen>();
        }

        #endregion

        #region Private Impl.

        static App()
        {
            XmlConfigurator.Configure();
            LogManager.Initialize(Log4NetAdapter.GetLog);
            Log = log4net.LogManager.GetLogger(typeof(App));
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // TODO create an exception screen.
            Log.Error(e.ExceptionObject);
        }

        #endregion
    }
}