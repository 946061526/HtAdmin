using GameBiz.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HTAdmin.Models.Finance
{
    /// <summary>
    /// 充值明细查询条件
    /// </summary>
    public class RechargeListQueryModel
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public string UserID { get; set; }

        public string OrderId { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 来源
        /// </summary>
        public string SchemeSource { get; set; }
        /// <summary>
        /// 代理商(渠道)
        /// </summary>
        public string AgentType { get; set; }

        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 推广商编号
        /// </summary>
        public string AgentIds { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string UserNickName { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal Money { get; set; }
    }
}