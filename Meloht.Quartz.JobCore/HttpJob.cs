using log4net;
using Meloht.Quartz.JobCore.Log;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Meloht.Quartz.JobCore
{
    public class HttpJob : IJob
    {
        ILog log = LogManager.GetLogger(Log4NetHelper.Instance.RepositoryName, typeof(HttpJob));
        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                JobDataMap dataMap = context.JobDetail.JobDataMap;

                string callbackUrl = dataMap.GetString(JobConfig.CallbackUrl);

                Dictionary<string, string> param = new Dictionary<string, string>();

                if (dataMap.ContainsKey(JobConfig.CallbackParams))
                {
                    string callbackParams = dataMap.GetString(JobConfig.CallbackParams);
                    param = JobUtils.GetDictFromString(callbackParams);
                }

                var result = JobUtils.GetHttpPost(callbackUrl, param);

                log.Info(result);

                string res = LogConfig.ExecuteResultSuccess;
                if (!string.IsNullOrEmpty(result))
                {
                    res = $"{res} {result}";
                }

                SchedulerLog.Instance.LogRun(context, res);

                if (context.JobDetail.Key.Group == JobConfig.JobGroupNameOnce)
                {
                    context.Scheduler.DeleteJob(context.JobDetail.Key).GetAwaiter().GetResult();
                }

            }
            catch (Exception ex)
            {
                SchedulerLog.Instance.LogRun(context, LogConfig.ExecuteResultFailed);

                StringBuilder sb = new StringBuilder();
                JobUtils.GetErrMessage(ex, sb);
                SchedulerLog.Instance.LogErr(context, sb.ToString());

                log.Error(ex);
            }

            return Task.FromResult(true);

        }


    }
}
