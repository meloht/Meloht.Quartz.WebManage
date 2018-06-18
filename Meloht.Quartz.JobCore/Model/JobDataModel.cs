using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.Quartz.JobCore.Model
{
    public class JobDataModel
    {
        public JobType JobType { get; set; }
        public string JobName { get; set; }

        public string JobGroupName { get; set; }

        public string CallbackUrl { get; set; }

        public Dictionary<string, string> CallbackParams { get; set; }

        public string CronExpression { get; set; }

        public string AssemblyDllName { get; set; }
        public string TypeFullName { get; set; }
    }
}
