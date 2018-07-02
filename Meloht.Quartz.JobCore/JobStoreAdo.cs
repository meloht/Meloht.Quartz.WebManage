using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using log4net;
using Meloht.Quartz.JobCore.Model;
using Quartz;

namespace Meloht.Quartz.JobCore
{
    public class JobStoreAdo : JobStoreBase, IJobStore
    {
        private readonly ILog logger = LogManager.GetLogger(typeof(JobStoreAdo));
        public bool AddAssemblyJob(JobParamAssemblyModel job)
        {
            try
            {
                string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, job.AssemblyDllName);
                Assembly assembly = Assembly.LoadFile(path);

                var type = assembly.GetType(job.TypeFullName);
                JobKey jobKey = CreateJobKey(job.JobName, job.JobGroupName);
                var bl = _scheduler.CheckExists(jobKey).GetAwaiter().GetResult();
                if (!bl)
                {
                    IJobDetail jobDetail = JobBuilder.Create(type)
                        .WithIdentity(jobKey)
                        .Build();

                    ITrigger trigger = CreateTrigger(job, jobKey);

                    _scheduler.ScheduleJob(jobDetail, trigger).GetAwaiter().GetResult();
                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return false;
        }

        public bool AddHttpJob(JobParamHttpModel job)
        {
            try
            {
                JobKey jobKey = CreateJobKey(job.JobName, job.JobGroupName);
                var bl = _scheduler.CheckExists(jobKey).GetAwaiter().GetResult();
                if (!bl)
                {
                    var jobbuilder = JobBuilder.Create<HttpJob>()
                        .WithIdentity(jobKey)
                        .UsingJobData(JobConfig.CallbackUrl, job.CallbackUrl);

                    var param = JobUtils.GetDictToString(job.CallbackParams);
                    if (!string.IsNullOrEmpty(param))
                    {
                        jobbuilder.UsingJobData(JobConfig.CallbackParams, JobUtils.GetDictToString(job.CallbackParams));
                    }

                    var jobDetail = jobbuilder.Build();

                    ITrigger trigger = CreateTrigger(job, jobKey);

                    _scheduler.ScheduleJob(jobDetail, trigger).GetAwaiter().GetResult();
                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return false;
        }

        public bool DeleteJob(JobParamBaseModel job)
        {
            try
            {
                var jobKey = CreateJobKey(job.JobName, job.JobGroupName);

                return _scheduler.DeleteJob(jobKey).GetAwaiter().GetResult();

            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return false;
        }

        public List<JobViewModel> GetJobList()
        {
            throw new NotImplementedException();
        }

        public List<KeyValueModel> GetJobParaList(string jobName, string jobGroupName)
        {
            throw new NotImplementedException();
        }

        public bool ModifyJobCron(JobParamBaseModel job)
        {
            try
            {
                JobKey jobKey = CreateJobKey(job.JobName, job.JobGroupName);
                CronScheduleBuilder scheduleBuilder = CronScheduleBuilder.CronSchedule(job.CronExpression);
                var triggerKey = CreateTriggerKey(job.TriggerName, job.TriggerGroupName);

                ITrigger trigger = TriggerBuilder.Create().StartNow()
                        .WithIdentity(job.TriggerName, job.TriggerGroupName)
                        .ForJob(jobKey)
                        .WithSchedule(scheduleBuilder.WithMisfireHandlingInstructionDoNothing())
                        .Build();

                _scheduler.RescheduleJob(triggerKey, trigger);
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return false;
        }

        public bool PauseJob(string jobName, string jobGroupName)
        {
            try
            {
                var jobKey = CreateJobKey(jobName, jobGroupName);

                _scheduler.PauseJob(jobKey).GetAwaiter().GetResult();
                return true;

            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return false;
        }

        public bool ResumeJob(string jobName, string jobGroupName)
        {
            try
            {
                var jobKey = CreateJobKey(jobName, jobGroupName);

                _scheduler.ResumeJob(jobKey).GetAwaiter().GetResult();
                return true;

            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return false;
        }

        public bool RunAtOnce(string jobName, string jobGroupName)
        {
            try
            {
                JobKey jobKey = CreateJobKey(jobName, jobGroupName);

                var jobData = _scheduler.GetJobDetail(jobKey).GetAwaiter().GetResult();

                JobKey jobKeyOnce = CreateJobKey(jobName, JobConfig.JobGroupNameOnce);

                var bl = _scheduler.CheckExists(jobKeyOnce).GetAwaiter().GetResult();
                if (bl)
                {
                    _scheduler.DeleteJob(jobKeyOnce);
                }
                IJobDetail jobDetail = JobBuilder.Create(jobData.JobType)
                    .WithIdentity(jobKeyOnce)
                    .UsingJobData(JobConfig.CallbackUrl, jobData.JobDataMap[JobConfig.CallbackUrl].ToString())
                    .UsingJobData(JobConfig.CallbackParams, jobData.JobDataMap[JobConfig.CallbackParams].ToString())
                    .Build();

                DateTimeOffset startTime = DateBuilder.NextGivenSecondDate(null, 2);
                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity(JobConfig.GetTriggerNameOnce(jobName), JobConfig.TriggerGroupNameOnce)
                    .StartAt(startTime)
                    .Build();

                _scheduler.ScheduleJob(jobDetail, trigger).GetAwaiter().GetResult();
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return false;
        }

        public bool UpdateHttpJob(JobParamHttpModel job)
        {
            throw new NotImplementedException();
        }
    }
}
