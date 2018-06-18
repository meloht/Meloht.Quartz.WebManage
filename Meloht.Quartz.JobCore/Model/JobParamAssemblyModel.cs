using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.Quartz.JobCore.Model
{
    public class JobParamAssemblyModel : JobParamBaseModel
    {
        public string AssemblyDllName { get; set; }
        public string TypeFullName { get; set; }

        public override string GetJobGroupName()
        {
            return JobConfig.JobGroupNameAssembly;
        }

        public override string GetTriggerGroupName()
        {
            return JobConfig.TriggerGroupNameAssembly;
        }

        public override string GetTriggerName()
        {
            return JobConfig.GetTriggerNameAssembly(JobName);
        }
    }
}
