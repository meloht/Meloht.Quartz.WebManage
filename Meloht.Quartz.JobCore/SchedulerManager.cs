using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.Quartz.JobCore
{
    public class SchedulerManager
    {

        static IScheduler _scheduler;

        public static IScheduler Instance
        {
            get
            {
                return _scheduler;
            }
        }

        internal static void InitScheduler()
        {
            var properties = JobConfig.GetJobConfig();

            ISchedulerFactory sf = new StdSchedulerFactory(properties);
            _scheduler = sf.GetScheduler().GetAwaiter().GetResult();

        }

        public static IJobStore JobFactory()
        {
            var jobStoreType = JobConfig.JobStoreType;
            if (jobStoreType == JobStoreType.AdoJobStore)
            {
                return new JobStoreAdo();
            }
            return new JobStoreRAM();
        }
    }
}
