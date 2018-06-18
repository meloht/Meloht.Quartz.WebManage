using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Meloht.Quartz.WebManage.Common
{
    public enum MsgCode
    {
        /// <summary>
        /// 无返回消息
        /// </summary>
        None = -2,
        /// <summary>
        /// 失败
        /// </summary>
        Failed = 0,
        /// <summary>
        /// 成功
        /// </summary>
        Success = 1

    }
}
