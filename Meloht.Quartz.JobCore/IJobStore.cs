using Meloht.Quartz.JobCore.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.Quartz.JobCore
{
    public interface IJobStore
    {
        bool DeleteJob(JobParamBaseModel job);
        bool AddAssemblyJob(JobParamAssemblyModel job);
        bool AddHttpJob(JobParamHttpModel job);
        bool UpdateHttpJob(JobParamHttpModel job);

        List<KeyValueModel> GetJobParaList(string jobName, string jobGroupName);

        bool RunAtOnce(string jobName, string jobGroupName);

        bool PauseJob(string jobName, string jobGroupName);
        bool ResumeJob(string jobName, string jobGroupName);

        bool ModifyJobCron(JobParamBaseModel job);

        List<JobViewModel> GetJobList();
        
    }
}
