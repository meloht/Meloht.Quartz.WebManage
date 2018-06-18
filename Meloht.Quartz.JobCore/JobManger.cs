using log4net;
using Meloht.Quartz.JobCore.Model;
using Quartz;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Meloht.Quartz.JobCore
{
    public class JobManger
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(JobManger));

        private readonly IScheduler _scheduler;
        public JobManger()
        {
            _scheduler = SchedulerManager.Instance;
        }

        public static bool RunJob()
        {
            try
            {
                AddListener();
                SchedulerManager.Instance.Start().GetAwaiter().GetResult();
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return false;

        }

        private static void AddListener()
        {
            var listener = SchedulerManager.Instance.ListenerManager.GetJobListener(JobConfig.JobGroupNameAssembly);
            if (listener == null)
            {
                IMatcher<JobKey> matcher = GroupMatcher<JobKey>.GroupEquals(JobConfig.JobGroupNameAssembly);
                SchedulerManager.Instance.ListenerManager.AddJobListener(new JobAssemblyListener(), matcher);
            }
        }



        public bool DeleteJob(JobParamBaseModel job)
        {
            try
            {
                var jobKey = CreateJobKey(job.JobName, job.JobGroupName);
                var triggerKey = CreateTriggerKey(job.TriggerName, job.TriggerGroupName);

                bool blxml = Xml.XmlJobManage.DeleteJobToXml(jobKey, triggerKey);
                if (blxml)
                {
                    return _scheduler.DeleteJob(jobKey).GetAwaiter().GetResult();
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return false;

        }

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
                    bool blxml = Xml.XmlJobManage.AddJobToXml(jobDetail, trigger);
                    if (blxml)
                    {
                        _scheduler.ScheduleJob(jobDetail, trigger).GetAwaiter().GetResult();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return false;

        }

        private ITrigger CreateTrigger(JobParamBaseModel job, JobKey jobKey)
        {
            CronScheduleBuilder scheduleBuilder = CronScheduleBuilder.CronSchedule(job.CronExpression);
            ITrigger trigger = TriggerBuilder.Create().StartNow()
                .WithIdentity(job.TriggerName, job.TriggerGroupName)
                .ForJob(jobKey)
                .WithSchedule(scheduleBuilder.WithMisfireHandlingInstructionDoNothing())
                .Build();

            return trigger;
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

                    bool blxml = Xml.XmlJobManage.AddJobToXml(jobDetail, trigger);
                    if (blxml)
                    {
                        _scheduler.ScheduleJob(jobDetail, trigger).GetAwaiter().GetResult();
                        return true;
                    }
                }
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
                JobDataModel jobData = Xml.XmlJobManage.GetJobDataByKey(jobKey);
                bool isSameParams = IsSameParam(job, jobData);
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



        /// <summary>
        /// 参数是否相同
        /// </summary>
        /// <returns></returns>
        private bool IsSameParam(JobParamHttpModel job, JobDataModel data)
        {
            string map = JobUtils.GetDictToString(data.CallbackParams);

            string newParam = JobUtils.GetDictToString(job.CallbackParams);

            if (data.CallbackUrl != job.CallbackUrl)
                return false;

            if (map.ToString() != newParam)
                return false;

            return true;
        }
        public List<KeyValueModel> GetJobParaList(string jobName, string jobGroupName)
        {
            List<KeyValueModel> list = new List<KeyValueModel>();
            try
            {
                JobKey jobKey = CreateJobKey(jobName, jobGroupName);

                JobDataModel jobData = Xml.XmlJobManage.GetJobDataByKey(jobKey);
                if (jobData == null || jobData.CallbackParams == null)
                    return list;

                foreach (var item in jobData.CallbackParams)
                {
                    KeyValueModel model = new KeyValueModel();
                    model.key = item.Key;
                    model.value = item.Value;
                    list.Add(model);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return list;
        }
        /// <summary>
        /// 运行一次任务
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="jobGroupName"></param>
        /// <returns></returns>
        public bool RunAtOnce(string jobName, string jobGroupName)
        {
            try
            {
                JobKey jobKey = CreateJobKey(jobName, jobGroupName);

                JobDataModel jobData = Xml.XmlJobManage.GetJobDataByKey(jobKey);
                if (jobData == null)
                    return false;

                JobKey jobKeyOnce = CreateJobKey(jobName, JobConfig.JobGroupNameOnce);

                var type = typeof(HttpJob);

                if (jobData.JobType == JobType.Assembly)
                {
                    string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, jobData.AssemblyDllName);
                    Assembly assembly = Assembly.LoadFile(path);
                    type = assembly.GetType(jobData.TypeFullName);
                }

                var bl = _scheduler.CheckExists(jobKeyOnce).GetAwaiter().GetResult();
                if (bl)
                {
                    _scheduler.DeleteJob(jobKeyOnce);
                }
                IJobDetail jobDetail = JobBuilder.Create(type)
                    .WithIdentity(jobKeyOnce)
                    .UsingJobData(JobConfig.CallbackUrl, jobData.CallbackUrl)
                    .UsingJobData(JobConfig.CallbackParams, JobUtils.GetDictToString(jobData.CallbackParams))
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

        public bool PauseJob(string jobName, string jobGroupName)
        {
            try
            {
                JobKey jobKey = CreateJobKey(jobName, jobGroupName);
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
                JobKey jobKey = CreateJobKey(jobName, jobGroupName);
                _scheduler.ResumeJob(jobKey).GetAwaiter().GetResult();
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return false;
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

                bool blxml = Xml.XmlJobManage.ModifyJobCronToXml(triggerKey, jobKey, trigger);
                if (blxml)
                {
                    _scheduler.RescheduleJob(triggerKey, trigger);
                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return false;
        }



        public List<JobViewModel> GetJobList()
        {
            return Xml.XmlJobManage.GetJobList();
        }

        private JobKey CreateJobKey(string jobName, string jobGroupName)
        {
            return new JobKey(jobName, jobGroupName);

        }
        private TriggerKey CreateTriggerKey(string triggerName, string triggerGroupName)
        {
            return new TriggerKey(triggerName, triggerGroupName);
        }


    }
}
