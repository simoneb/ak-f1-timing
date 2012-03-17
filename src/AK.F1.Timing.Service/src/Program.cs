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
using AK.CmdLine;
using AK.F1.Timing.Service.Configuration;
using AK.F1.Timing.Service.Configuration.Impl;
using AK.F1.Timing.Service.Jobs;
using AK.F1.Timing.Service.Utility;
using log4net;
using log4net.Config;
using Quartz;

namespace AK.F1.Timing.Service
{
    /// <summary>
    /// F1 Live-Timing Service for .NET 4.
    /// </summary>
    public sealed class Program
    {
        #region Fields.

        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        #endregion

        #region Public Inteface.

        static Program()
        {
            XmlConfigurator.Configure();
        }

        /// <summary>
        /// Application entry point.
        /// </summary>
        /// <param name="args">The application arguments.</param>
        public static void Main(string[] args)
        {
            new CmdLineDriver(new Program(), Console.Out).TryProcess(args);
        }

        /// <summary>
        /// Reads the schedule as specified by the configuration and starts the scheduler.
        /// </summary>
        public void Start()
        {
            Log.Info("starting");
            try
            {
                var scheduler = CreateScheduler();
                Log.Info("started, press ctrl+c to exit");
                CmdLineCancelKey.WaitFor().Dispose();
                scheduler.Shutdown();
                SessionServerJob.Stop();
            }
            catch (Exception exc)
            {
                Log.Error(exc);
            }
            finally
            {
                Log.Info("stopped");
            }
        }

        /// <summary>
        /// Validates the configuration and reports any errors.
        /// </summary>        
        public void Validate()
        {
            try
            {
                GetConfiguration();
                Log.Info("configuration is valid");
            }
            catch (Exception exc)
            {
                Log.Error(exc.Message);
            }
        }

        /// <summary>
        /// Converts the specified BBC ICS file to a race XML document expected by the service.
        /// </summary>
        /// <param name="path">The path of the ICS file.</param>
        /// <param name="startTimeOffset">The start time offset (in minutes).</param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="path"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when <paramref name="path"/> is of zero length.
        /// </exception>
        public void ConvertIcs(string path, int startTimeOffset = -2)
        {
            try
            {
                IcsConverter.ToXDocument(path, startTimeOffset).Save(Console.Out);
            }
            catch (Exception exc)
            {
                Log.Error(exc);
            }
        }

        #endregion

        #region Private Impl.

        private Program() { }

        private static IScheduler CreateScheduler()
        {
            var scheduler = SchedulerFactory.Create(GetConfiguration());
            scheduler.Start();
            return scheduler;
        }

        private static IServiceConfiguration GetConfiguration()
        {
            return ServiceConfigurationSection.GetSection();
        }

        #endregion
    }
}
