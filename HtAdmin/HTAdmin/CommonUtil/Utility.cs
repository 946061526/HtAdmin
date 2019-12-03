using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace HTAdmin
{
    /// <summary>
    /// 通用处理类
    /// </summary>
    public class Utility
    {
        /// <summary>
        /// 根据日期取到日期的23点59分59.999秒
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetDateTime24(DateTime date)
        {
            if (date != null)
                return Convert.ToDateTime(date).Date.AddDays(1).AddMilliseconds(-1);
            return DateTime.Today.AddDays(1).AddMilliseconds(-1);
        }

        ///// <summary>
        ///// 顺利付
        ///// </summary>
        ///// <param name="channelID"></param>
        ///// <returns></returns>
        //public static string GetPayTypeSlf(string channelID)
        //{
        //    var pay = "";
        //    var channel = ConfigurationManager.AppSettings["slf_appid"].ToString();
        //    if (!string.IsNullOrEmpty(channel))
        //        channel = "11006";
        //    if (channelID == channel)
        //        pay = "顺利付代付";

        //    return pay;
        //}
        ///// <summary>
        ///// 艾付
        ///// </summary>
        ///// <param name="channelID"></param>
        ///// <returns></returns>
        //public static string GetPayTypeAf(string channelID)
        //{
        //    var pay = "";
        //    var channel = ConfigurationManager.AppSettings["af_merchant_no"].ToString();
        //    if (!string.IsNullOrEmpty(channel))
        //        channel = "144801008923";
        //    if (channelID == channel)
        //        pay = "艾付代付";

        //    return pay;
        //}
        ///// <summary>
        ///// 新付
        ///// </summary>
        ///// <param name="channelID"></param>
        ///// <returns></returns>
        //public static string GetPayTypeXf(string channelID)
        //{
        //    var pay = "";
        //    var channel = ConfigurationManager.AppSettings["xinfu_merchNo"].ToString();
        //    if (!string.IsNullOrEmpty(channel))
        //        channel = "20180126001676";
        //    if (channelID == channel)
        //        pay = "新付代付";

        //    return pay;
        //}

        /// <summary>
        /// 获取代付通道名
        /// </summary>
        /// <param name="channnelID">代付通道号</param>
        /// <returns></returns>
        public static string GetPayType(string channnelID)
        {
            if (string.IsNullOrEmpty(channnelID))
                return string.Empty;

            Hashtable ht = new Hashtable();
            ht["11006"] = "顺利付代付";
            var key = ConfigurationManager.AppSettings["slf_appid"].ToString();
            if (!string.IsNullOrEmpty(key))
                ht[key] = "顺利付代付";

            ht["144801008923"] = "艾付代付";
            key = ConfigurationManager.AppSettings["af_merchant_no"].ToString();
            if (!string.IsNullOrEmpty(key))
                ht[key] = "艾付代付";

            ht["20180126001676"] = "艾付代付";
            key = ConfigurationManager.AppSettings["xinfu_merchNo"].ToString();
            if (!string.IsNullOrEmpty(key))
                ht[key] = "新付代付";

            return ht.Contains(channnelID) ? ht[channnelID].ToString() : string.Empty;
        }
    }
}