using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.Quartz.JobCore.Model
{
    public abstract class JobParamBaseModel
    {
        public string JobName { get; set; }

        public string CronExpression { get; set; }


        public string JobGroupName
        {
            get { return GetJobGroupName(); }
        }

        public string TriggerName
        {
            get { return GetTriggerName(); }
        }

        public string TriggerGroupName
        {
            get { return GetTriggerGroupName(); }
        }




        public abstract string GetJobGroupName();

        public abstract string GetTriggerName();

        public abstract string GetTriggerGroupName();

    }
}
