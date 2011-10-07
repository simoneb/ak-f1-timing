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
using AK.F1.Timing.Service.Configuration;
using AK.F1.Timing.Service.Jobs;
using AK.F1.Timing.Utility;
using log4net;
using Quartz;
using Quartz.Impl;

namespace AK.F1.Timing.Service
{
    /// <summary>
    /// Provides the means of creating and configuring an <see cref="Quartz.IScheduler"/> with F1 service
    /// jobs. This class is <see langword="static"/>.
    /// </summary>
    internal static class SchedulerFactory
    {
        #region Fields.

        private static readonly ILog Log = LogManager.GetLogger(typeof(SchedulerFactory));

        #endregion

        #region Public Inteface.

        /// <summary>
        /// Defines the F1 service job group name. This field is constant.
        /// </summary>
        public const string JobGroupName = "f1service";

        /// <summary>
        /// Creates and configures a new <see cref="Quartz.IScheduler"/>.
        /// </summary>
        /// <param name="configuration">The
        /// <see cref="AK.F1.Timing.Service.Configuration.IServiceConfiguration"/></param>
        /// <returns>A configured <see cref="Quartz.IScheduler"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when <paramref name="configuration"/> is <see langword="null"/>.
        /// </exception>
        public static IScheduler Create(IServiceConfiguration configuration)
        {
            Guard.NotNull(configuration, "configuration");

            var scheduler = new StdSchedulerFactory().GetScheduler();
            ScheduleJobs(scheduler, configuration);
            return scheduler;
        }

        #endregion

        #region Private Impl.

        private static void ScheduleJobs(IScheduler scheduler, IServiceConfiguration configuration)
        {
            foreach(var race in configuration)
            {
                foreach(var session in race)
                {
                    if(session.StartTimeUtc < SysClock.Now())
                    {
                        Log.WarnFormat("skipped past session, name={0}\\{1}, startTimeUtc={2:o}", race.Id,
                            session.Id, session.StartTimeUtc);
                        continue;
                    }
                    if(configuration.Server.IsEnabled)
                    {
                        ScheduleJob<SessionServerJob>(scheduler, configuration, race, session);
                    }
                    if(configuration.Recorder.IsEnabled)
                    {
                        ScheduleJob<SessionRecorderJob>(scheduler, configuration, race, session);
                    }
                }
            }
        }

        private static void ScheduleJob<TJob>(IScheduler scheduler, IServiceConfiguration service,
            IRaceConfiguration race, ISessionConfiguration session) where TJob : SessionJobBase
        {
            var jobType = typeof(TJob);
            var jobName = String.Format(@"{0}\{1}\{2}", race.Id, session.Id, jobType.Name);
            var jobDetail = new JobDetail(jobName, JobGroupName, jobType);
            jobDetail.JobDataMap.Add(SessionJobBase.ServiceConfigurationContextPropertyName, service);
            jobDetail.JobDataMap.Add(SessionJobBase.RaceConfigurationContextPropertyName, race);
            jobDetail.JobDataMap.Add(SessionJobBase.SessionConfigurationContextPropertyName, session);
            scheduler.ScheduleJob(jobDetail, new SimpleTrigger(jobName, JobGroupName, session.StartTimeUtc));
            Log.InfoFormat("scheduled job, name={0}, startTimeUtc={1:o}", jobName, session.StartTimeUtc);
        }

        #endregion
    }
}
