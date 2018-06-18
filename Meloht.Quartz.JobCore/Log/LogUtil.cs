using log4net;
using Meloht.Quartz.JobCore.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Meloht.Quartz.JobCore.Log
{
    public class LogUtil
    {
        private static ILog log = LogManager.GetLogger(Log4NetHelper.Instance.RepositoryName, typeof(LogUtil));


        private static void WriteLog(List<string> listLog, string fileName)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(fileName, true, Encoding.UTF8))
                {
                    foreach (string msg in listLog)
                    {
                        sw.WriteLine(msg);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

        }

        public static void WriteErrLog(List<LogErrModel> list)
        {
            string fileName = LogConfig.GetErrFileName();

            List<string> listLog = new List<string>(list.Count);

            foreach (LogErrModel item in list)
            {
                listLog.Add(GetStringLineErr(item));
            }
            WriteLog(listLog, fileName);
        }
        private static string GetStringLineErr(LogErrModel model)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(model.JobName).Append(LogConfig.LogSplit);
            sb.Append(model.JobGroupName).Append(LogConfig.LogSplit);
            sb.Append(model.JobType).Append(LogConfig.LogSplit);
            sb.Append(model.JobData).Append(LogConfig.LogSplit);
            sb.Append(model.CronExpression).Append(LogConfig.LogSplit);

            sb.Append(model.ErrDateTime).Append(LogConfig.LogSplit);
            sb.Append(model.ErrMessage);

            return sb.ToString();
        }

        public static LogErrModel GetLogErrModelByRow(string line)
        {
            if (String.IsNullOrEmpty(line))
                return null;

            var arr = line.Split(new[] { LogConfig.LogSplit }, StringSplitOptions.RemoveEmptyEntries);

            if (arr.Length != 7)
                return null;

            LogErrModel model = new LogErrModel();

            model.JobName = arr[0];
            model.JobGroupName = arr[1];
            model.JobType = arr[2];
            model.JobData = arr[3];
            model.CronExpression = arr[4];
            model.ErrDateTime = arr[5];
            model.ErrMessage = arr[6];

            return model;
        }

        private static string GetStringLineRun(LogRunModel model)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(model.JobName).Append(LogConfig.LogSplit);
            sb.Append(model.JobGroupName).Append(LogConfig.LogSplit);
            sb.Append(model.JobType).Append(LogConfig.LogSplit);
            sb.Append(model.JobData).Append(LogConfig.LogSplit);
            sb.Append(model.CronExpression).Append(LogConfig.LogSplit);

            sb.Append(model.ExecuteTime).Append(LogConfig.LogSplit);
            sb.Append(model.ExecuteResult);

            return sb.ToString();
        }

        public static LogRunModel GetLogRunModelByRow(string line)
        {
            if (String.IsNullOrEmpty(line))
                return null;

            var arr = line.Split(new[] { LogConfig.LogSplit }, StringSplitOptions.RemoveEmptyEntries);

            if (arr.Length != 7)
                return null;

            LogRunModel model = new LogRunModel();
            model.JobName = arr[0];
            model.JobGroupName = arr[1];
            model.JobType = arr[2];
            model.JobData = arr[3];
            model.CronExpression = arr[4];
            model.ExecuteTime = arr[5];
            model.ExecuteResult = arr[6];

            return model;
        }

        public static void WriteRunLog(List<LogRunModel> list)
        {
            string fileName = LogConfig.GetRunFileName();

            List<string> listLog = new List<string>(list.Count);

            foreach (LogRunModel item in list)
            {
                listLog.Add(GetStringLineRun(item));
            }
            WriteLog(listLog, fileName);
        }



        public static PageData<LogRunModel> GetRunLogListPage(QueryParamModel model)
        {
            PageData<LogRunModel> result = new PageData<LogRunModel>();

            result.Rows = new List<LogRunModel>();
            try
            {
                var filePaths = GetLogFileListByDateRange(model.BeginDate, model.EndDate, LogConfig.GetRunDir());

                int total = 0;
                int limit = 0;
                foreach (string filePath in filePaths)
                {
                    var listRows = GetLogRowLine(filePath, model.KeyWord);
                    foreach (string row in listRows)
                    {
                        var item = GetLogRunModelByRow(row);
                        if (item != null)
                        {
                            total++;
                            if (total > model.Offset)
                            {
                                if (model.Limit > limit)
                                {
                                    limit++;
                                    result.Rows.Add(item);
                                }
                            }
                        }
                    }
                }
                result.Total = total;
            }
            catch (Exception e)
            {
                log.Error(e);
            }
            return result;
        }


        public static PageData<LogErrModel> GetErrLogListPage(QueryParamModel model)
        {
            PageData<LogErrModel> result = new PageData<LogErrModel>();
            result.Rows = new List<LogErrModel>();
            try
            {
                int total = 0;
                int limit = 0;

                var filePaths = GetLogFileListByDateRange(model.BeginDate, model.EndDate, LogConfig.GetErrDir());

                foreach (string filePath in filePaths)
                {
                    var listRows = GetLogRowLine(filePath, model.KeyWord);
                    foreach (string row in listRows)
                    {
                        var item = GetLogErrModelByRow(row);
                        if (item != null)
                        {
                            total++;
                            if (total > model.Offset)
                            {
                                if (model.Limit > limit)
                                {
                                    limit++;
                                    result.Rows.Add(item);
                                }
                            }
                        }
                    }
                }
                result.Total = total;
            }
            catch (Exception e)
            {
                log.Error(e);
            }
            return result;
        }


        public static List<string> GetLogRowLine(string path, string key)
        {
            List<string> list = new List<string>();
            try
            {
                if (!File.Exists(path))
                {
                    return list;
                }
                using (StreamReader sr = new StreamReader(path, Encoding.UTF8))
                {

                    string s;
                    while ((s = sr.ReadLine()) != null)
                    {
                        if (!String.IsNullOrEmpty(s) && s.Contains(key))
                        {
                            list.Add(s);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e);
            }
            return list;
        }

        public static List<string> GetLogFileListByDateRange(DateTime begin, DateTime end, string dir)
        {
            List<string> list = new List<string>();
            try
            {
                if (!Directory.Exists(dir))
                    return list;

                var files = Directory.GetFiles(dir);
                if (files == null || files.Length == 0)
                    return list;

                foreach (string file in files)
                {
                    string fileName = Path.GetFileNameWithoutExtension(file);
                    DateTime date = GetDateFormName(fileName, DateTime.MinValue);
                    if (date >= begin && date <= end)
                    {
                        list.Add(file);
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e);
            }
            return list;
        }

        public static DateTime GetDateFormName(string dateString, DateTime defaultTime)
        {
            DateTime date = defaultTime;
            bool bl = DateTime.TryParse(dateString, out date);
            if (bl)
            {
                return date;
            }
            return defaultTime;
        }

        /// <summary>  
        /// 获取文件大小  
        /// </summary>  
        /// <param name="sFullName"></param>  
        /// <returns></returns>  
        public static long GetFileSize(string sFullName)
        {
            long lSize = 0;
            if (File.Exists(sFullName))
                lSize = new FileInfo(sFullName).Length;
            return lSize;
        }
        /// <summary>  
        /// 计算文件大小函数(保留两位小数),Size为字节大小  
        /// </summary>  
        /// <param name="size">初始文件大小</param>  
        /// <returns></returns>  
        public static string CountSize(long size)
        {
            string mStrSize = "";
            long factSize = 0;
            factSize = size;
            if (factSize < 1024.00)
                mStrSize = factSize.ToString("F2") + " Byte";
            else if (factSize >= 1024.00 && factSize < 1048576)
                mStrSize = (factSize / 1024.00).ToString("F2") + " K";
            else if (factSize >= 1048576 && factSize < 1073741824)
                mStrSize = (factSize / 1024.00 / 1024.00).ToString("F2") + " M";
            else if (factSize >= 1073741824)
                mStrSize = (factSize / 1024.00 / 1024.00 / 1024.00).ToString("F2") + " G";
            return mStrSize;
        }

        public static PageData<Log4NetModel> GetListPage(QueryParamModel model)
        {
            PageData<Log4NetModel> result = new PageData<Log4NetModel>();
            result.Rows = new List<Log4NetModel>();
            try
            {
                int total = 0;
                int limit = 0;
                var filePaths = LogUtil.GetLogFileListByDateRange(model.BeginDate, model.EndDate, Log4NetHelper.Instance.GetLogDir);

                foreach (string path in filePaths)
                {
                    string fileName = Path.GetFileName(path);
                    if (fileName.EndsWith("log"))
                    {
                        total++;
                        if (total > model.Offset)
                        {
                            if (model.Limit > limit)
                            {
                                Log4NetModel item = new Log4NetModel();
                                item.FileSize = LogUtil.CountSize(LogUtil.GetFileSize(path));
                                item.FileName = fileName;

                                limit++;
                                result.Rows.Add(item);
                            }
                        }
                    }
                }
                result.Total = total;
            }
            catch (Exception e)
            {
                log.Error(e);
            }

            return result;
        }
    }
}
