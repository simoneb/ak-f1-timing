// Copyright 2011 Andy Kernahan
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
using System.Diagnostics;
using AK.F1.Timing.Service.Configuration;
using log4net;
using Quartz;

namespace AK.F1.Timing.Service.Jobs
{
    /// <summary>
    /// Defines a useful base class for session related <see cref="Quartz.IJob"/>s. This class is
    /// <see langword="abstract"/>.
    /// </summary>
    public abstract class SessionJobBase : IJob
    {
        #region Fields.

        private ILog _log;

        /// <summary>
        /// Defines the name of the <see cref="Quartz.JobExecutionContext"/> property which contains
        /// the <see cref="AK.F1.Timing.Service.Configuration.IServiceConfiguration"/>.
        /// </summary>
        public const string ServiceConfigurationContextPropertyName = "ServiceConfiguration";

        /// <summary>
        /// Defines the name of the <see cref="Quartz.JobExecutionContext"/> property which contains the
        /// <see cref="AK.F1.Timing.Service.Configuration.IRaceConfiguration"/>.
        /// </summary>
        public const string RaceConfigurationContextPropertyName = "RaceConfiguration";

        /// <summary>
        /// Defines the name of the <see cref="Quartz.JobExecutionContext"/> property which contains the
        /// <see cref="AK.F1.Timing.Service.Configuration.ISessionConfiguration"/>.
        /// </summary>
        public const string SessionConfigurationContextPropertyName = "SessionConfiguration";

        #endregion

        #region Public Interface.

        /// <inheritdoc/>        
        public void Execute(JobExecutionContext context)
        {
            Log.Info("running");
            try
            {
                ApplyConfiguration(context.JobDetail.JobDataMap);
                ExecuteCore();
            }
            catch(Exception exc)
            {
                Log.Error(exc);
            }
            finally
            {
                Log.Info("complete");
            }
        }

        #endregion

        #region Protected Interface.

        /// <summary>
        /// When overriden in a derived class; executes the job.
        /// </summary>
        protected abstract void ExecuteCore();

        /// <summary>
        /// Creates a live <see cref="AK.F1.Timing.IMessageReader"/> using the credentials specified in the
        /// <see cref="AK.F1.Timing.Service.Configuration.IServiceConfiguration"/>.
        /// </summary>
        /// <returns>An <see cref="AK.F1.Timing.IMessageReader"/>.</returns>
        protected IMessageReader CreateReader()
        {
            var token = F1Timing.Live.Login(ServiceConfiguration.Username, ServiceConfiguration.Password);
            return F1Timing.Live.Read(token);
        }

        /// <summary>
        /// Gets the <see cref="log4net.ILog"/> for this type.
        /// </summary>        
        protected ILog Log
        {
            [DebuggerStepThrough]
            get
            {
                if(_log == null)
                {
                    _log = LogManager.GetLogger(GetType());
                }
                return _log;
            }
        }

        /// <summary>
        /// Gets the <see cref="AK.F1.Timing.Service.Configuration.IServiceConfiguration"/>.
        /// </summary>
        protected IServiceConfiguration ServiceConfiguration { get; private set; }

        /// <summary>
        /// Gets the <see cref="AK.F1.Timing.Service.Configuration.IRaceConfiguration"/>.
        /// </summary>
        protected IRaceConfiguration RaceConfiguration { get; private set; }

        /// <summary>
        /// Gets the <see cref="AK.F1.Timing.Service.Configuration.ISessionConfiguration"/>.
        /// </summary>
        protected ISessionConfiguration SessionConfiguration { get; private set; }

        #endregion

        #region Private Impl.

        private void ApplyConfiguration(JobDataMap data)
        {
            ServiceConfiguration = (IServiceConfiguration)data[ServiceConfigurationContextPropertyName];
            RaceConfiguration = (IRaceConfiguration)data[RaceConfigurationContextPropertyName];
            SessionConfiguration = (ISessionConfiguration)data[SessionConfigurationContextPropertyName];
        }

        #endregion
    }
}
