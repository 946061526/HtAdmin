using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HTAdmin.Models
{
    /// <summary>
    /// 返回数据grid模型
    /// </summary>
    public class LayuiGridResult
    {
        /// <summary>
        /// 状态
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 描述信息
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 总行数
        /// </summary>
        public int count { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public object data { get; set; }
    }
}