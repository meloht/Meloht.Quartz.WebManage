using log4net;
using Meloht.Quartz.JobCore.Log;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Meloht.Quartz.JobCore
{
    public class JobAssemblyListener : IJobListener
    {
        ILog log = LogManager.GetLogger(Log4NetHelper.Instance.RepositoryName, typeof(JobAssemblyListener));
        public string Name => JobConfig.JobGroupNameAssembly;

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {

            return JobUtils.CompletedTask;
        }

        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            return JobUtils.CompletedTask;
        }

        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default(CancellationToken))
        {

            if (jobException != null)
            {
                SchedulerLog.Instance.LogRun(context, LogConfig.ExecuteResultFailed);
                log.Error(jobException);

                StringBuilder sb = new StringBuilder();
                JobUtils.GetErrMessage(jobException, sb);
                SchedulerLog.Instance.LogErr(context, sb.ToString());
            }
            else
            {
                SchedulerLog.Instance.LogRun(context, LogConfig.ExecuteResultSuccess);
            }
            return JobUtils.CompletedTask;
        }
    }
}
