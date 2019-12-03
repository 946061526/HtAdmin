using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HTAdmin.Models
{
    public class AddAwardModel
    {
        public int Id { get; set; }
        public int OrderIndex { get; set; }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public string PlayType { get; set; }
        public decimal AddBonusMoneyPercent { get; set; }
        public decimal MaxAddBonusMoney { get; set; }
        public string AddMoneyWay { get; set; }
        public DateTime CreateTime { get; set; }
    }
}