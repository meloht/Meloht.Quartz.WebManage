using Meloht.Quartz.JobCore.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Meloht.Quartz.WebManage.Models
{
    public class LogQueryModel : PageBaseParams
    {
        public string BeginDate { get; set; }
        public string EndDate { get; set; }

        public string KeyWord { get; set; }
    }
}
