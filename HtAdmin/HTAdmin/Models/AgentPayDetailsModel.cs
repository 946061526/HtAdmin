using External.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HTAdmin.Models
{
    public class AgentPayDetailsModel
    {
        public virtual DateTime? fromDate { get; set; }
        public virtual DateTime? toDate { get; set; }
        public virtual string userId { get; set; }
        public virtual string displayName { get; set; }
        public virtual string gameCode { get; set; }
        public virtual decimal startBuyMoney { get; set; }
        public virtual decimal endBuyMoney { get; set; }
        public virtual decimal startRabateMoney { get; set; }
        public virtual decimal endRabateMoney { get; set; }
        public virtual int pageIndex { get; set; }
        public virtual int pageSize { get; set; }
    }
}