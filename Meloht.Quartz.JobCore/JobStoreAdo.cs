using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using log4net;
using Meloht.Quartz.JobCore.Model;
using Quartz;
using Quartz.Impl.Matchers;
using Quartz.Impl.Triggers;

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
            List<JobViewModel> list = new List<JobViewModel>();
            try
            {
                var jobs= _scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup()).GetAwaiter().GetResult().ToList();
                foreach (var item in jobs)
                {
                    var jobDetail = _scheduler.GetJobDetail(item).GetAwaiter().GetResult();
                  
                    JobViewModel model = new JobViewModel();
                    var jobTriggers= _scheduler.GetTriggersOfJob(item).GetAwaiter().GetResult().ToList();
                    if (jobTriggers.Count > 0)
                    {
                        var tr = jobTriggers.First();
                        if (tr is CronTriggerImpl)
                        {
                            var cron = tr as CronTriggerImpl;
                            model.CronExpression = cron.CronExpressionString;
                        }

                        var state = JobUtils.GetTriggerState(tr.Key.Name, tr.Key.Group);
                        model.JobState = JobUtils.GetTriggerStateValue(state);

                    }

                    model.JobType = jobDetail.JobType.ToString();
                    model.JobData =JobUtils.GetJobDataString(jobDetail.JobDataMap);
                    model.JobGroupName = jobDetail.Key.Group;
                    model.JobName = jobDetail.Key.Name;
                   

                    list.Add(model);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return list;
        }

        public List<KeyValueModel> GetJobParaList(string jobName, string jobGroupName)
        {
            List<KeyValueModel> list = new List<KeyValueModel>();
            try
            {
                JobKey jobKey = CreateJobKey(jobName, jobGroupName);

                var jobData = _scheduler.GetJobDetail(jobKey).GetAwaiter().GetResult();
                if (jobData == null||jobData.JobDataMap==null)
                    return list;

                foreach (var item in jobData.JobDataMap)
                {
                    KeyValueModel model = new KeyValueModel();
                    model.key = item.Key;
                    model.value = item.Value.ToString();
                    list.Add(model);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return list;
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
            try
            {
                JobKey jobKey = CreateJobKey(job.JobName, job.JobGroupName);
                var jobData = _scheduler.GetJobDetail(jobKey).GetAwaiter().GetResult();
                JobDataModel model = new JobDataModel();
                model.CallbackUrl = jobData.JobDataMap[JobConfig.CallbackUrl].ToString();
                model.CallbackParams = JobUtils.GetDictFromString(jobData.JobDataMap[JobConfig.CallbackParams].ToString());

                bool isSameParams = IsSameParam(job, model);
                if (isSameParams)
                {
                    return ModifyJobCron(job);
                }
                else
                {
                    bool bl = DeleteJob(job);
                    if (bl)
                    {
                        return AddHttpJob(job);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return false;
        }
    }
}
