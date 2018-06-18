using System;
using System.Collections.Generic;
using System.Text;

namespace Meloht.Quartz.JobCore.Model
{
    public class PageData<T>
    {
        public int Total { get; set; }

        public List<T> Rows { get; set; }
    }
}
