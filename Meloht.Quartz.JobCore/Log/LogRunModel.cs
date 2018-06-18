using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.Quartz.JobCore.Log
{
    public class LogRunModel
    {
        public string JobName { get; set; }
        public string JobGroupName { get; set; }
        /// <summary>
        /// url 或者 dll 程序集名称
        /// </summary>
        public string JobType { get; set; }

        public string JobData { get; set; }

        public string CronExpression { get; set; }

        public string ExecuteTime { get; set; }

        public string ExecuteResult { get; set; }
    }
}
