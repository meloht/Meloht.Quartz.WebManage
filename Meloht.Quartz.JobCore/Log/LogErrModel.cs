using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.Quartz.JobCore.Log
{
    public class LogErrModel
    {
        public string JobName { get; set; }
        public string JobGroupName { get; set; }
        /// <summary>
        /// dll 程序集名称
        /// </summary>
        public string JobType { get; set; }

        public string JobData { get; set; }

        public string CronExpression { get; set; }

        public string ErrDateTime { get; set; }

        public string ErrMessage { get; set; }
    }
}
