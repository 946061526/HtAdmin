using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HTAdmin.Models
{
    public class StaticPageInfo
    {
        public virtual string Type { get; set; }
        public virtual int Index { get; set; }
        public virtual string PageName { get; set;}
        public virtual string Value { get; set; }
        public virtual string CreateTime { get; set; }
    }
}