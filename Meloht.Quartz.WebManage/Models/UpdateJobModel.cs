using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Meloht.Quartz.WebManage.Models
{
    public class UpdateJobModel
    {
        [Display(Name = "任务名")]
        [Required(ErrorMessage = "任务名不能为空")]
        public string JobName { get; set; }

        public string JobGroupValue { get; set; }

        [Display(Name = "执行时间表达式")]
        [Required(ErrorMessage = "执行时间表达式不能为空")]
        public string CronExpression { get; set; }

        [Display(Name = "任务Url")]
        [Required(ErrorMessage = "任务Url不能为空")]
        public string CallbackUrl { get; set; }

        [Display(Name = "任务参数")]
        public string CallbackParams { get; set; }
    }
}
