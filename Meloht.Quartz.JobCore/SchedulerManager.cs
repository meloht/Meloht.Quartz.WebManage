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

        internal static IScheduler InitScheduler()
        {
            var properties = JobConfig.GetJobConfig();

            ISchedulerFactory sf = new StdSchedulerFactory(properties);
            IScheduler sched = sf.GetScheduler().GetAwaiter().GetResult();
            return sched;
        }
    }
}
