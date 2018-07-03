using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Meloht.Quartz.JobCore;
using Meloht.Quartz.JobCore.Model;
using Meloht.Quartz.JobCore.Xml;
using Meloht.Quartz.WebManage.Common;
using Meloht.Quartz.WebManage.Models;
using Microsoft.AspNetCore.Mvc;
using Quartz;

namespace Meloht.Quartz.WebManage.Controllers
{
    public class JobController : Controller
    {
        private JobManger _job = new JobManger();
        public IActionResult JobListIndex()
        {
            return View();
        }

        public IActionResult GetCreateJobView()
        {
            return PartialView("JobCreateView");
        }

        public IActionResult GetUpdateJobView(string jobName, string group)
        {
            UpdateJobModel model = new UpdateJobModel();
            model.JobName = jobName;
            model.JobGroupValue = group;

            var data = XmlJobManage.GetJobDataByKey(new JobKey(jobName, group));
            if (data != null)
            {
                model.CronExpression = data.CronExpression;
                model.CallbackUrl = data.CallbackUrl;

            }

            return PartialView("UpdateJobView", model);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



        [HttpPost]
        public JsonResult GetJobList(JobQueryModel model)
        {
            if (string.IsNullOrEmpty(model.JobName))
            {
                model.JobName = string.Empty;
            }

            var data = _job.GetJobList();
            var tdata = data.Where(p => p.JobName.Contains(model.JobName)).OrderBy(p => p.JobName).ToList();
            int total = tdata.Count;
            var list = tdata.Skip(model.Offset).Take(model.Limit).ToList(); ;
            return Json(new { total = total, rows = list });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult RunAtOnce(string jobName, string groupName)
        {
            bool bl = _job.RunAtOnce(jobName, groupName);
            if (bl)
            {
                return BaseJson(MsgCode.Success, "运行成功");
            }
            else
            {
                return BaseJson(MsgCode.Failed, "运行失败");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CreateJob(JobHttpModel model)
        {
            if (!ModelState.IsValid)
            {
                return BaseJson(MsgCode.Failed, "数据验证失败");
            }

            JobParamHttpModel jobParam = new JobParamHttpModel();
            jobParam.JobName = model.JobName;
            jobParam.CronExpression = model.CronExpression;
            jobParam.CallbackUrl = model.CallbackUrl;
            jobParam.CallbackParams = GetParaDict(model.CallbackParams);

            bool bl = _job.AddHttpJob(jobParam);
            if (bl)
            {
                return BaseJson(MsgCode.Success, "创建成功");
            }
            else
            {
                return BaseJson(MsgCode.Failed, "创建失败");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult PauseJob(string jobName,string groupName)
        {

            bool bl = _job.PauseJob(jobName, groupName);
            if (bl)
            {
                return BaseJson(MsgCode.Success, "暂停成功");
            }
            else
            {
                return BaseJson(MsgCode.Failed, "暂停失败");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ResumeJob(string jobName, string groupName)
        {
            bool bl = _job.ResumeJob(jobName, groupName);
            if (bl)
            {
                return BaseJson(MsgCode.Success, "恢复成功");
            }
            else
            {
                return BaseJson(MsgCode.Failed, "恢复失败");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteJob(string jobName,string groupName)
        {
            var model = JobUtils.GetJobParamBase(jobName, groupName);

            bool bl = _job.DeleteJob(model);
            if (bl)
            {
                return BaseJson(MsgCode.Success, "删除成功");
            }
            else
            {
                return BaseJson(MsgCode.Failed, "删除失败");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ModifyJob(UpdateJobModel model)
        {
            JobParamHttpModel jobParam = new JobParamHttpModel();
            jobParam.JobName = model.JobName;
            jobParam.CronExpression = model.CronExpression;
            jobParam.CallbackUrl = model.CallbackUrl;
            jobParam.CallbackParams = GetParaDict(model.CallbackParams);

            bool bl = _job.UpdateHttpJob(jobParam);
            if (bl)
            {
                return BaseJson(MsgCode.Success, "修改成功");
            }
            else
            {
                return BaseJson(MsgCode.Failed, "修改失败");
            }
        }

        [HttpGet]
        public JsonResult GetJobParaList(string jobName, string groupName)
        {
            var data = _job.GetJobParaList(jobName, groupName);
            return Json(data);

        }

        #region private

        private Dictionary<string, string> GetParaDict(string json)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(json))
            {
                return dict;
            }

            var para = JobUtils.ToObject<List<KeyValueModel>>(json);
            if (para != null)
            {
                foreach (var item in para)
                {
                    if (!dict.ContainsKey(item.key))
                    {
                        dict.Add(item.key, item.value);
                    }
                }
            }
            return dict;
        }


        private JsonResult BaseJson(MsgCode code, string msg, object data = null)
        {
            object model = null;

            if (code == MsgCode.None)
            {
                model = data;
            }
            else
            {
                if (data != null)
                {
                    model = new { status = (int)code, msg = msg, data = data };
                }
                else
                {
                    model = new { status = (int)code, msg = msg };
                }
            }
            JsonResult json = new JsonResult(model);

            return json;
        }
        #endregion
    }
}