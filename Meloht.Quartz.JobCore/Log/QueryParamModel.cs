using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.Quartz.JobCore.Log
{
    public class QueryParamModel
    {
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }

        public string KeyWord { get; set; }

        public int Limit { get; set; }

        public int Offset { get; set; }
    }
}
