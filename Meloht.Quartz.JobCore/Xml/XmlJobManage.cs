using log4net;
using Meloht.Quartz.JobCore.Log;
using Meloht.Quartz.JobCore.Model;
using Quartz;
using Quartz.Util;
using Quartz.Xml.JobSchedulingData20;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meloht.Quartz.JobCore.Xml
{
    public class XmlJobManage
    {
        private static ILog log = LogManager.GetLogger(Log4NetHelper.Instance.RepositoryName, typeof(XmlJobManage));

        private static string GetXmlFilePath()
        {
            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, JobConfig.QuartzXmlFileName);
            return path;
        }




        private static jobdetailType GetJobdetailType(IJobDetail model)
        {
            jobdetailType job = new jobdetailType();
            job.durable = model.Durable;
            job.description = model.Description;
            job.group = model.Key.Group;
            job.jobtype = model.JobType.AssemblyQualifiedNameWithoutVersion();
            job.name = model.Key.Name;
            job.recover = model.RequestsRecovery;

            if (model.JobDataMap != null && model.JobDataMap.Count > 0)
            {
                job.jobdatamap = new jobdatamapType();
                job.jobdatamap.entry = GetEntryTypes(model.JobDataMap);
            }
            return job;
        }

        private static entryType[] GetEntryTypes(JobDataMap jobDataMap)
        {
            List<entryType> entries = new List<entryType>();
            foreach (var item in jobDataMap)
            {
                entryType entry = new entryType();
                entry.key = item.Key;
                entry.value = item.Value.ToString();

                entries.Add(entry);
            }
            return entries.ToArray();
        }

        private static triggerType GetTriggerType(ITrigger trigger, JobKey jobKey)
        {
            triggerType triggerType = new triggerType();
            cronTriggerType cronTrigger = new cronTriggerType();

            cronTrigger.priority = trigger.Priority.ToString();
            cronTrigger.name = trigger.Key.Name;
            cronTrigger.calendarname = trigger.CalendarName.TrimEmptyToNull();
            cronTrigger.description = trigger.Description;
            cronTrigger.misfireinstruction = GetCronMisfireInstruction(trigger.MisfireInstruction);

            if (trigger.EndTimeUtc != null)
            {
                cronTrigger.endtime = trigger.EndTimeUtc.Value.DateTime;
                cronTrigger.endtimeSpecified = true;
            }
            else
            {
                cronTrigger.endtimeSpecified = false;
            }
            cronTrigger.group = trigger.Key.Group;

            if (trigger.JobKey != null)
            {
                cronTrigger.jobname = trigger.JobKey.Name;
                cronTrigger.jobgroup = trigger.JobKey.Group;
            }
            else
            {
                cronTrigger.jobname = jobKey.Name;
                cronTrigger.jobgroup = jobKey.Group;
            }

            if (trigger is ICronTrigger cTrigger)
            {
                cronTrigger.cronexpression = cTrigger.CronExpressionString.TrimEmptyToNull();
                cronTrigger.timezone = cTrigger.TimeZone.Id;

            }

            if (trigger.JobDataMap != null && trigger.JobDataMap.Count > 0)
            {
                cronTrigger.jobdatamap = new jobdatamapType();
                cronTrigger.jobdatamap.entry = GetEntryTypes(trigger.JobDataMap);
            }

            triggerType.Item = cronTrigger;

            return triggerType;
        }

        private static string GetCronMisfireInstruction(int misfireInstruction)
        {
            switch (misfireInstruction)
            {
                case 0:
                    return "SmartPolicy";
                case -1:
                    return "IgnoreMisfirePolicy";
                case 2:
                    return "DoNothing";
                case 1:
                    return "FireOnceNow";
                default:
                    break;
            }

            return null;
        }

        public static bool AddJobToXml(IJobDetail jobDetail, ITrigger trigger)
        {
            try
            {
                string path = GetXmlFilePath();
                var data = XmlUtil.DeserializeXml<QuartzXmlConfiguration20>(path);

                if (data.schedule == null || data.schedule.Length == 0)
                {
                    jobschedulingdataSchedule schedule = new jobschedulingdataSchedule();
                    data.schedule = new[] { schedule };
                }

                var job = GetJobdetailType(jobDetail);
                var trg = GetTriggerType(trigger, jobDetail.Key);

                var sch = data.schedule.First();
                if (sch.job != null)
                {
                    var ls = sch.job.ToList();
                    var dels = sch.job.Where(p => p.group == jobDetail.Key.Group && p.name == jobDetail.Key.Name).ToList();
                    foreach (var jobDel in dels)
                    {
                        ls.Remove(jobDel);
                    }
                    ls.Add(job);
                    sch.job = ls.ToArray();
                }
                else
                {
                    sch.job = new[] { job };
                }

                if (sch.trigger != null)
                {
                    var ls = sch.trigger.ToList();
                    var dels = ls.Where(p => p.Item.name == trigger.Key.Name && p.Item.group == trigger.Key.Group).ToList();
                    foreach (var del in dels)
                    {
                        ls.Remove(del);
                    }
                    ls.Add(trg);

                    sch.trigger = ls.ToArray();
                }
                else
                {
                    sch.trigger = new[] { trg };
                }

                return XmlUtil.SerializeXml(data, path);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return false;
        }

        public static bool DeleteJobToXml(JobKey jobKey, TriggerKey triggerKey)
        {
            try
            {
                string path = GetXmlFilePath();
                var data = XmlUtil.DeserializeXml<QuartzXmlConfiguration20>(path);
                if (data.schedule != null && data.schedule.Length > 0)
                {
                    var sch = data.schedule.First();
                    if (sch.job != null && sch.job.Length > 0)
                    {
                        var jobs = sch.job.Where(p => p.name == jobKey.Name && p.group == jobKey.Group).ToList();
                        if (jobs.Count > 0)
                        {
                            var ls = sch.job.ToList();
                            foreach (var item in jobs)
                            {
                                ls.Remove(item);
                            }
                            sch.job = ls.ToArray();
                        }

                    }
                    if (sch.trigger != null && sch.trigger.Length > 0)
                    {
                        var trigs = sch.trigger.Where(p => p.Item.name == triggerKey.Name && p.Item.group == triggerKey.Group).ToList();
                        if (trigs.Count > 0)
                        {
                            var ls = sch.trigger.ToList();
                            foreach (var item in trigs)
                            {
                                ls.Remove(item);
                            }

                            sch.trigger = ls.ToArray();
                        }
                    }
                }
                return XmlUtil.SerializeXml(data, path);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            return false;
        }

        public static bool ModifyJobCronToXml(TriggerKey triggerKey, JobKey jobKey, ITrigger trigger)
        {
            try
            {
                string path = GetXmlFilePath();
                var data = XmlUtil.DeserializeXml<QuartzXmlConfiguration20>(path);

                if (data.schedule != null && data.schedule.Length > 0)
                {
                    var trg = GetTriggerType(trigger, jobKey);
                    var sch = data.schedule.First();
                    if (sch.trigger != null && sch.trigger.Length > 0)
                    {
                        var trigs = sch.trigger.SingleOrDefault(p => p.Item.name == triggerKey.Name && p.Item.group == triggerKey.Group);
                        if (trigs != null)
                        {
                            var ls = sch.trigger.ToList();

                            ls.Remove(trigs);
                            ls.Add(trg);
                            sch.trigger = ls.ToArray();
                        }
                    }
                    else
                    {
                        sch.trigger = new[] { trg };
                    }
                }

                return XmlUtil.SerializeXml(data, path);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return false;
        }
        private static TriggerState GetTriggerState(string triggerName, string triggerGroupName)
        {

            TriggerKey triggerKey = new TriggerKey(triggerName, triggerGroupName);
            var triggerState = SchedulerManager.Instance.GetTriggerState(triggerKey).GetAwaiter().GetResult();

            return triggerState;
        }

        private static int GetTriggerStateValue(TriggerState triggerState)
        {
            switch (triggerState)
            {
                case TriggerState.None: return -1;
                case TriggerState.Normal: return 0;
                case TriggerState.Paused: return 1;
                case TriggerState.Complete: return 2;
                case TriggerState.Error: return 3;
                case TriggerState.Blocked: return 4;
                default: return -1;
            }

        }
        public static List<JobViewModel> GetJobList()
        {
            List<JobViewModel> list = new List<JobViewModel>();
            try
            {
                string path = GetXmlFilePath();
                var data = XmlUtil.DeserializeXml<QuartzXmlConfiguration20>(path);
                if (data == null)
                    return list;
                if (data.schedule == null || data.schedule.Length == 0)
                    return list;

                foreach (var sch in data.schedule)
                {
                    if (sch.job == null || sch.job.Length == 0)
                        continue;

                    foreach (var job in sch.job)
                    {
                        JobViewModel model = new JobViewModel();

                        model.JobName = job.name;
                        model.JobGroupName = job.group;
                        model.JobType = job.jobtype;
                        model.JobData = JobUtils.GetJobDataString(job.jobdatamap);

                        if (sch.trigger != null)
                        {
                            var tr = sch.trigger.SingleOrDefault(p => p.Item.jobname == job.name && p.Item.jobgroup == job.group);
                            if (tr != null)
                            {
                                var cd = tr.Item as cronTriggerType;
                                if (cd != null)
                                {
                                    model.CronExpression = cd.cronexpression;

                                }

                                var state = GetTriggerState(tr.Item.name, tr.Item.group);
                                model.JobState = GetTriggerStateValue(state);
                            }

                        }
                        list.Add(model);
                    }
                }

            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return list;
        }

        public static JobDataModel GetJobDataByKey(JobKey jobKey)
        {
            JobDataModel jobData = new JobDataModel();
            jobData.JobType = JobUtils.GetJobType(jobKey.Group);
            jobData.JobName = jobKey.Name;
            jobData.JobGroupName = jobKey.Group;

            try
            {
                string path = GetXmlFilePath();
                var data = XmlUtil.DeserializeXml<QuartzXmlConfiguration20>(path);
                if (data == null)
                    return null;
                if (data.schedule == null || data.schedule.Length == 0)
                    return null;

                foreach (var sch in data.schedule)
                {
                    if (sch.job == null || sch.job.Length == 0)
                        continue;

                    var item = sch.job.SingleOrDefault(p => p.name == jobKey.Name && p.group == jobKey.Group);
                    if (item != null)
                    {
                        if (jobData.JobType == JobType.Http)
                        {
                            jobData.CallbackUrl = GetDataMapValue(item.jobdatamap, JobConfig.CallbackUrl);
                            string paramString = GetDataMapValue(item.jobdatamap, JobConfig.CallbackParams);
                            jobData.CallbackParams = JobUtils.GetDictFromString(paramString);
                        }
                        else
                        {
                            if (item.jobtype != null)
                            {
                                var arr = item.jobtype.Split(",", StringSplitOptions.RemoveEmptyEntries);
                                if (arr.Length == 2)
                                {
                                    jobData.TypeFullName = arr[0];
                                    jobData.AssemblyDllName = arr[1];
                                }
                            }
                        }
                        if (sch.trigger != null)
                        {
                            var tr = sch.trigger.SingleOrDefault(p => p.Item.jobname == jobKey.Name && p.Item.jobgroup == jobKey.Group);
                            if (tr != null)
                            {
                                var cd = tr.Item as cronTriggerType;
                                if (cd != null)
                                {
                                    jobData.CronExpression = cd.cronexpression;
                                }
                            }
                        }

                        return jobData;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return null;

        }

        public static string GetDataMapValue(jobdatamapType jobdatamap, string key)
        {
            if (jobdatamap == null || jobdatamap.entry == null)
                return string.Empty;
            var item = jobdatamap.entry.SingleOrDefault(p => p.key == key);
            if (item != null)
                return item.value;
            return string.Empty;
        }


    }
}
