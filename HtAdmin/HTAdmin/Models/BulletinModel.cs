using Common;
using GameBiz.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HTAdmin.Models
{
    public class BulletinModel
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public EnableStatus Status { get; set; }
        public BulletinAgent BulletinAgent { get; set; }
        public int Priority { get; set; }
        public int IsPutTop { get; set; }
        public string Keyword { get; set; }
        public string Summary { get; set; }
        public string CoverImage { get; set; }
    }
}