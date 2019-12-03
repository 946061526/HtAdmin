using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using External.Core;

namespace HTAdmin.Models
{
    public class AgentDetailModel
    {
        public virtual string UserId { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual string Mobile { get; set; }
        public virtual string ParentAgentId { get; set; }
        public virtual string IsAgent { get; set; }
        public virtual AgentGrade AgentGrade { get; set; }
        public virtual int PageIndex { get; set; }
        public virtual int PageSize { get; set; }
    }
}