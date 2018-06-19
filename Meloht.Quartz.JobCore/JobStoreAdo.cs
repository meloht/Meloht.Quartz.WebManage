using System;
using System.Collections.Generic;
using System.Text;
using Meloht.Quartz.JobCore.Model;

namespace Meloht.Quartz.JobCore
{
    public class JobStoreAdo : IJobStore
    {
        public bool AddAssemblyJob(JobParamAssemblyModel job)
        {
            throw new NotImplementedException();
        }

        public bool AddHttpJob(JobParamHttpModel job)
        {
            throw new NotImplementedException();
        }

        public bool DeleteJob(JobParamBaseModel job)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public bool PauseJob(string jobName, string jobGroupName)
        {
            throw new NotImplementedException();
        }

        public bool ResumeJob(string jobName, string jobGroupName)
        {
            throw new NotImplementedException();
        }

        public bool RunAtOnce(string jobName, string jobGroupName)
        {
            throw new NotImplementedException();
        }

        public bool UpdateHttpJob(JobParamHttpModel job)
        {
            throw new NotImplementedException();
        }
    }
}
