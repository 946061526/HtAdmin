using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HTAdmin.Models
{
    public class ArticleModel
    {
        public string Id { get; set; }
        /// <summary>
        /// 对应彩种
        /// </summary>
        public string GameCode { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 关键字
        /// </summary>
        public string KeyWords { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string DescContent { get; set; }
        /// <summary>
        /// 是否标红
        /// </summary>
        public bool IsRedTitle { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        public string Category { get; set; }
        public string OriginalCategory { get; set; }
        /// <summary>
        /// 创建者编号
        /// </summary>
        public string CreateUserKey { get; set; }
        /// <summary>
        /// 创建者显示名称
        /// </summary>
        public string CreateUserDisplayName { get; set; }
        /// <summary>
        /// 优先级别（数字越小，表示级别越高）（1高 2中 3低）
        /// </summary>
        public virtual int Priority { get; set; }
        /// <summary>
        /// 是否置顶  1置顶  0不置顶
        /// </summary>
        public bool IsPutTop { get; set; }
        /// <summary>
        /// 咨询图片Url
        /// </summary>
        public virtual string Url { get; set; }
        public virtual string Url1 { get; set; }
        public virtual string Url2 { get; set; }
        public virtual string ImgType { get; set; }
        /// <summary>
        /// 0表示禁用 1 表示启用
        /// </summary>
        public int Status { get; set; }
        public DateTime PublishTime { get;  set; }
    }
}