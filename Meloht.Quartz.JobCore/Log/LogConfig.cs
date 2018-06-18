using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Meloht.Quartz.JobCore.Log
{
    public class LogConfig
    {
        /// <summary>
        /// 任务运行异常日志
        /// </summary>
        public const string ErrDir = "JobErrLog";
        /// <summary>
        /// 任务运行结果日志
        /// </summary>
        public const string RunDir = "JobRunLog";

        public const string ExecuteResultSuccess = "Success";
        public const string ExecuteResultFailed = "Failed";

        public static string AppBase = AppDomain.CurrentDomain.BaseDirectory;

        public const string LogSplit = "|$|";


        public static string GetErrDir()
        {
            string path = System.IO.Path.Combine(AppBase, ErrDir);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        public static string GetRunDir()
        {
            string path = System.IO.Path.Combine(AppBase, RunDir);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        public static string GetErrFileName()
        {
            string file = string.Format("{0}.log", DateTime.Now.ToString("yyyy-MM-dd"));
            string path = Path.Combine(GetErrDir(), file);

            return path;

        }

        public static string GetRunFileName()
        {
            string file = string.Format("{0}.log", DateTime.Now.ToString("yyyy-MM-dd"));
            string path = Path.Combine(GetRunDir(), file);

            return path;
        }
    }
}
