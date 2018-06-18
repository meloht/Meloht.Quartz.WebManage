using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.Quartz.JobCore
{
    public class SchedulerManager
    {
        private static readonly object Locker = new object();
        static IScheduler _scheduler;

        public static IScheduler Instance
        {
            get
            {
                if (_scheduler == null)
                {
                    lock (Locker)
                    {
                        if (_scheduler == null)
                        {
                            _scheduler = GetScheduler();
                        }
                    }
                }
                return _scheduler;
            }
        }

        private static IScheduler GetScheduler()
        {
            var properties = JobConfig.GetJobXmlCfg();

            ISchedulerFactory sf = new StdSchedulerFactory(properties);
            IScheduler sched = sf.GetScheduler().GetAwaiter().GetResult();
            return sched;
        }
    }
}
