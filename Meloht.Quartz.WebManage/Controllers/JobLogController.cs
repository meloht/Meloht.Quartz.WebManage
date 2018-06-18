using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using log4net;
using Meloht.Quartz.JobCore.Log;
using Meloht.Quartz.WebManage.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Meloht.Quartz.WebManage.Controllers
{
    public class JobLogController : Controller
    {
        private ILog _logger = LogManager.GetLogger(Log4NetHelper.Instance.RepositoryName, typeof(JobLogController));
        /// <summary>
        /// 运行日志
        /// </summary>
        /// <returns></returns>
        public IActionResult JobWorkLogList()
        {
            return View();
        }

        public IActionResult JobErrorLogList()
        {
            return View();
        }

        public IActionResult JobAppLogList()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetRunLogList(LogQueryModel model)
        {
            QueryParamModel query = GetQueryModel(model);

            var list = LogUtil.GetRunLogListPage(query);

            return Json(new { total = list.Total, rows = list.Rows });
        }

        [HttpPost]
        public JsonResult GetErrLogList(LogQueryModel model)
        {
            QueryParamModel query = GetQueryModel(model);

            var list = LogUtil.GetErrLogListPage(query);
            return Json(new { total = list.Total, rows = list.Rows });
        }

        [HttpPost]
        public JsonResult GetAppLogList(LogQueryModel model)
        {
            QueryParamModel query = GetQueryModel(model);
            var list = LogUtil.GetListPage(query);
            return Json(new { total = list.Total, rows = list.Rows });
        }
        [HttpGet]
        public void DownloadLog(string file)
        {
            try
            {
                string path = Log4NetHelper.Instance.GetFilePath(file);
                if (!System.IO.File.Exists(path))
                {
                    return;
                }

                Response.Clear();
                FileInfo fileInfo = new FileInfo(path);

                string ua = Request.Headers["UserAgent"];
                if (ua != null && ua.ToLower().IndexOf("firefox") > -1)
                {
                    Response.Headers.Add("Content-Disposition", "attachment;filename*=utf-8'zh_cn'" + HttpUtility.UrlEncode(file, System.Text.Encoding.UTF8));

                }
                else
                {
                    Response.Headers.Add("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(file, System.Text.Encoding.UTF8));
                }
                HttpContext.Response.Headers.Add("Content-Length", fileInfo.Length.ToString());
                HttpContext.Response.Headers.Add("Content-Transfer-Encoding", "binary");

                Response.ContentType = "application/octet-stream;charset=UTF-8";

                HttpContext.Response.SendFileAsync(path).GetAwaiter().GetResult();

            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
        }

        private QueryParamModel GetQueryModel(LogQueryModel model)
        {
            QueryParamModel data = new QueryParamModel();

            data.KeyWord = model.KeyWord ?? "";
            data.BeginDate = LogUtil.GetDateFormName(model.BeginDate, DateTime.MinValue);
            data.EndDate = LogUtil.GetDateFormName(model.EndDate, DateTime.MaxValue);

            data.Limit = model.Limit;
            data.Offset = model.Offset;

            return data;
        }

    }
}