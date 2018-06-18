using Meloht.Quartz.JobCore.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Meloht.Quartz.WebManage.Models
{
    public class JobQueryModel : PageBaseParams
    {
        public string JobName { get; set; }
    }
}
