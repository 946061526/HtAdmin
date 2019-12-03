using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HTAdmin.Models
{
    public class SystemUserSearchAdditionModel
    {
        public string RoleId { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public string Mobile { get; set; }
        public bool? IsEnable { get; set; }
        public string RealName { get; set; }
        public DateTime LoginTime { get; set; }
        public string LoginIp { get; set; }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}