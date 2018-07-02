using log4net;
using Meloht.Quartz.JobCore.Log;
using Meloht.Quartz.JobCore.Model;
using Newtonsoft.Json;
using Quartz;
using Quartz.Xml.JobSchedulingData20;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Meloht.Quartz.JobCore
{
    public class JobUtils
    {
        private static ILog log = LogManager.GetLogger(Log4NetHelper.Instance.RepositoryName, typeof(JobUtils));
        public static readonly Task CompletedTask = Task.FromResult(true);

        public static string GetDictToString(Dictionary<string, string> dict)
        {
            if (dict == null || dict.Count == 0)
                return String.Empty;

            string jsonStr = JsonConvert.SerializeObject(dict);
            return jsonStr;
        }

        public static Dictionary<string, string> GetDictFromString(string jsonStr)
        {
            if (String.IsNullOrEmpty(jsonStr))
                return new Dictionary<string, string>();
            try
            {
                Dictionary<string, string> jsonDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonStr);
                return jsonDict;
            }
            catch (Exception e)
            {
                log.Error(e);
            }

            return new Dictionary<string, string>(); ;
        }

        public static string GetHttpPost(string url, Dictionary<string, string> paras)
        {
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.None };
            using (HttpClient httpclient = new HttpClient(handler))
            {
                httpclient.BaseAddress = new Uri(url);

                var content = new FormUrlEncodedContent(paras);

                var response = httpclient.PostAsync(url, content);

                var responseString = response.GetAwaiter().GetResult().Content.ReadAsStringAsync().GetAwaiter().GetResult();


                return responseString;
            }
        }

        public static string GetHttpGet(string url)
        {
            if (url.StartsWith("https"))
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = httpClient.GetAsync(url).Result;
                var statusCode = response.StatusCode;
                if (response.IsSuccessStatusCode)
                {
                    string result = response.Content.ReadAsStringAsync().Result;
                    return result;
                }
            }
            return null;
        }


        public static string GetJobDataString(JobDataMap jobData)
        {
            if (jobData != null)
            {
                List<string> ls = new List<string>();
                foreach (var item in jobData)
                {
                    ls.Add($"{item.Key}:{item.Value}");
                }
                return String.Join(",", ls);
            }
            return String.Empty;
        }

        public static string GetJobDataString(jobdatamapType jobData)
        {
            if (jobData != null && jobData.entry != null)
            {
                List<string> ls = new List<string>();
                foreach (var item in jobData.entry)
                {
                    if (!string.IsNullOrEmpty(item.value))
                    {
                        ls.Add($"{item.key}:{item.value}");
                    }
                }
                return String.Join(",", ls);
            }
            return String.Empty;
        }



        public static void GetErrMessage(Exception ex, StringBuilder sb)
        {
            if (ex != null)
            {
                sb.Append(ex.Message);
                GetErrMessage(ex.InnerException, sb);
            }
        }

        public static T ToObject<T>(string json)
        {
            try
            {
                JsonSerializerSettings setting = new JsonSerializerSettings();
                setting.NullValueHandling = NullValueHandling.Ignore;
                setting.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                return JsonConvert.DeserializeObject<T>(json, setting);
            }
            catch (Exception e)
            {
                log.Error(e);
            }

            return default(T);
        }

        public static string ToJson(object obj)
        {
            try
            {
                JsonSerializerSettings setting = new JsonSerializerSettings();
                setting.NullValueHandling = NullValueHandling.Ignore;
                setting.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                return JsonConvert.SerializeObject(obj, setting);
            }
            catch (Exception e)
            {
                log.Error(e);
            }

            return string.Empty;
        }

        public static JobType GetJobType(string groupName)
        {
            if (groupName == JobConfig.JobGroupNameHttp)
            {
                return JobType.Http;
            }
            else if (groupName == JobConfig.JobGroupNameAssembly)
            {
                return JobType.Assembly;
            }

            return JobType.Http;

        }

    }
}
