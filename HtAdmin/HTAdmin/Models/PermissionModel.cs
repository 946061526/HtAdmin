using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HTAdmin.Models
{
    public class PermissionModel
    {
        /// <summary>
        /// 权限标题
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// input显示的值
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }
        /// <summary>
        /// 表示是否选中
        /// </summary>
        [JsonProperty("checked")]
        public bool Checked { get; set; }

        /// <summary>
        /// 表示是否禁用
        /// </summary>
        [JsonProperty("disabled")]
        public bool Disabled { get; set; }

        [JsonProperty("visible")]
        public bool Visible { get; set; } = true;

        /// <summary>
        /// 内部其他的权限
        /// </summary>
        [JsonProperty("data")]
        public List<PermissionModel> Data { get; set; }
    }
}