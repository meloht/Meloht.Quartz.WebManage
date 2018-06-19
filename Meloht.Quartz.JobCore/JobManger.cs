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

        private IJobStore _jobStore;

     
        public JobManger()
        {
            _jobStore = new JobStoreRAM();
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
            return _jobStore.DeleteJob(job);

        }

        public bool AddAssemblyJob(JobParamAssemblyModel job)
        {
            return _jobStore.AddAssemblyJob(job);
        }

        public bool AddHttpJob(JobParamHttpModel job)
        {
            return _jobStore.AddHttpJob(job);
        }

        public bool UpdateHttpJob(JobParamHttpModel job)
        {
            return _jobStore.UpdateHttpJob(job);
        }


        public List<KeyValueModel> GetJobParaList(string jobName, string jobGroupName)
        {
            return _jobStore.GetJobParaList(jobName, jobGroupName);
           
        }
        /// <summary>
        /// 运行一次任务
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="jobGroupName"></param>
        /// <returns></returns>
        public bool RunAtOnce(string jobName, string jobGroupName)
        {
            return _jobStore.RunAtOnce(jobName, jobGroupName);
        }

        public bool PauseJob(string jobName, string jobGroupName)
        {
            return _jobStore.PauseJob(jobName, jobGroupName);
        }

        public bool ResumeJob(string jobName, string jobGroupName)
        {
            return _jobStore.ResumeJob(jobName, jobGroupName);
        }

        public bool ModifyJobCron(JobParamBaseModel job)
        {
            return _jobStore.ModifyJobCron(job);
        }

        public List<JobViewModel> GetJobList()
        {
            return _jobStore.GetJobList();
        }

    }
}
