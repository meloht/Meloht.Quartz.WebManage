using Meloht.Quartz.JobCore.Model;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.Quartz.JobCore
{
    public class JobStoreBase
    {
        protected readonly IScheduler _scheduler;
        public JobStoreBase()
        {
            _scheduler = SchedulerManager.Instance;
        }

        protected JobKey CreateJobKey(string jobName, string jobGroupName)
        {
            return new JobKey(jobName, jobGroupName);

        }
        protected TriggerKey CreateTriggerKey(string triggerName, string triggerGroupName)
        {
            return new TriggerKey(triggerName, triggerGroupName);
        }

        protected ITrigger CreateTrigger(JobParamBaseModel job, JobKey jobKey)
        {
            CronScheduleBuilder scheduleBuilder = CronScheduleBuilder.CronSchedule(job.CronExpression);
            ITrigger trigger = TriggerBuilder.Create().StartNow()
                .WithIdentity(job.TriggerName, job.TriggerGroupName)
                .ForJob(jobKey)
                .WithSchedule(scheduleBuilder.WithMisfireHandlingInstructionDoNothing())
                .Build();

            return trigger;
        }

        /// <summary>
        /// 参数是否相同
        /// </summary>
        /// <returns></returns>
        protected bool IsSameParam(JobParamHttpModel job, JobDataModel data)
        {
            string map = JobUtils.GetDictToString(data.CallbackParams);

            string newParam = JobUtils.GetDictToString(job.CallbackParams);

            if (data.CallbackUrl != job.CallbackUrl)
                return false;

            if (map.ToString() != newParam)
                return false;

            return true;
        }
    }
}
