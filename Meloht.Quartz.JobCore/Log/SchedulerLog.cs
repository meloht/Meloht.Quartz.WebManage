using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Meloht.Quartz.JobCore.Log
{
    public class SchedulerLog
    {
        private readonly Queue<LogErrModel> _queueErrInfo = new Queue<LogErrModel>();
        private readonly Queue<LogRunModel> _queueRunInfo = new Queue<LogRunModel>();

        // 日志列表同步锁
        private static readonly object LockHelper = new object();

        // 记录日志的线程
        private Thread _threadLog = null;
        // 线程起止标识
        private bool _isStop = false;

        public static readonly SchedulerLog Instance = new SchedulerLog();


        public SchedulerLog()
        {
            _threadLog = new Thread(SaveLog);
            _threadLog.IsBackground = true;
            _threadLog.Start();
        }


        private void SaveLog()
        {
            while (!_isStop)
            {
                WriteLog();
                Thread.Sleep(1000);
            }
        }

        private void WriteLog()
        {
            #region 输出异常日志
            if (_queueErrInfo.Count > 0)
            {
                List<LogErrModel> logs = new List<LogErrModel>();
                lock (LockHelper)
                {
                    while (_queueErrInfo.Count > 0)
                    {
                        var item = _queueErrInfo.Dequeue();
                        logs.Add(item);
                    }
                }
                if (logs.Count > 0)
                {
                    LogUtil.WriteErrLog(logs);
                }
            }
            #endregion

            #region 输出运行日志
            if (_queueRunInfo.Count > 0)
            {
                List<LogRunModel> logs = new List<LogRunModel>();
                lock (LockHelper)
                {
                    while (_queueRunInfo.Count > 0)
                    {
                        var item = _queueRunInfo.Dequeue();
                        logs.Add(item);
                    }
                }
                if (logs.Count > 0)
                {
                    LogUtil.WriteRunLog(logs);
                }
            }
            #endregion
        }



        public void LogErr(IJobExecutionContext context, string err)
        {
            LogErrModel model = new LogErrModel();
            if (context.Trigger is ICronTrigger)
            {
                var cTrigger = (ICronTrigger)context.Trigger;
                model.CronExpression = cTrigger.CronExpressionString ?? "-";

            }
            else
            {
                model.CronExpression = "-";
            }
            model.ErrDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            model.JobName = context.JobDetail.Key.Name;
            model.JobGroupName = context.JobDetail.Key.Group;
            model.JobType = context.JobDetail.JobType.FullName;
            model.JobData = JobUtils.GetJobDataString(context.JobDetail.JobDataMap);
            model.ErrMessage = err;

            lock (LockHelper)
            {
                _queueErrInfo.Enqueue(model);
            }
        }

        public void LogRun(IJobExecutionContext context, string result)
        {
            LogRunModel model = new LogRunModel();
            if (context.Trigger is ICronTrigger)
            {
                var cTrigger = (ICronTrigger)context.Trigger;
                model.CronExpression = cTrigger.CronExpressionString ?? "-";

            }
            else
            {
                model.CronExpression = "-";
            }

            model.ExecuteTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            model.JobName = context.JobDetail.Key.Name;
            model.JobGroupName = context.JobDetail.Key.Group;
            model.JobType = context.JobDetail.JobType.FullName;
            model.JobData = JobUtils.GetJobDataString(context.JobDetail.JobDataMap);

            model.ExecuteResult = result;

            lock (LockHelper)
            {
                _queueRunInfo.Enqueue(model);
            }
        }
    }
}
