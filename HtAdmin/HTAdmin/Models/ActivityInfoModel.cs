using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HTAdmin.Models
{
    public class ActivityInfoModel
    {
        public int Id { get; set; }
        [Required]
        public string ActivityName { get; set; }
        public int ActivityCategory { get; set; }
        public int ActivityType { get; set; }
        public int Status { get; set; }
        [Required]
        public DateTime BeginTime { get; set; }
        public DateTime EndTime { get; set; }
        public string CreatorId { get; set; }
        public DateTime CreationDate { get; set; }
        public long TotalCount { get; set; }
        public int TodayCount { get; set; }
        /// <summary>
        /// 具体彩种加奖配置
        /// </summary>
        public List<AddAwardModel> BonusList { get; set; }
    }
}