using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HTAdmin.Models
{
    public class ArticleConditionModel
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string Title { get; set; }
        /// <summary>
        /// 文章类别
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// 文章对应的彩种
        /// </summary>
        public string GameCode { get; set; }

        public DateTime? StartCreationDate { get; set; }
        public DateTime? EndCreationDate { get; set; }
        public string CreatorUserId { get; set; }
        /// <summary>
        /// 状态 -1 表示获取所有的状态的文章  0表示 禁用  1表示启用
        /// </summary>
        public int Status { get; set; } = -1;
    }
}