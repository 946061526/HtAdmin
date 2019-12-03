using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HTAdmin.Models
{
    public class UserQueryConditionModel
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string UserId { get; set; }
        public string DisplayName { get; set; }
        public string Mobile { get; set; }
        public bool? IsEnable { get; set; }
        public string OrderBy { get; set; }
        public bool AscendingOrder { get; set; }
        public string RegistrationTime { get; set; }
        public string PurchaseCount { get; set; }
        public string TotalLotteryMoney { get; set; }
    }
}