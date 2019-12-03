using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HTAdmin.Models
{
    public class NoticeConditionModel
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string Title { get; set; }
        public DateTime? StartCreationDate { get; set; }
        public DateTime? EndCreationDate { get; set; }
        public string CreatorUserId { get; set; }
        public EnableStatus Status { get; set; } = EnableStatus.Unknown;
    }
}