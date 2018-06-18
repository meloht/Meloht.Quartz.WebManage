using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.Quartz.JobCore.Model
{
    public class JobParamHttpModel : JobParamBaseModel
    {
        public string CallbackUrl { get; set; }

        public Dictionary<string, string> CallbackParams { get; set; }

        public override string GetJobGroupName()
        {
            return JobConfig.JobGroupNameHttp;
        }

        public override string GetTriggerGroupName()
        {
            return JobConfig.TriggerGroupNameHttp;
        }

        public override string GetTriggerName()
        {
            return JobConfig.GetTriggerNameHttp(JobName);
        }
    }
}
