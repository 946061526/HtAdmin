using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MatchBiz.Core;
using Common.JSON;
using System.Configuration;
using System.IO;
using Common.Lottery.OpenDataGetters;
using System.Text;
using Common.Log;
using Common.Business;

namespace HTAdmin.jsonManager
{
    /// <summary>
    /// 数字彩 - json文件读取管理
    /// </summary>
    public static class Json_SZC
    {
        #region 属性
        /// <summary>
        /// 数字彩数据存放物理路径根目录
        /// </summary>
        public static string MatchRoot
        {
            get
            {
                return ConfigurationManager.AppSettings["MatchRoot"] ?? "~/MatchData/";
            }
        }

        /// <summary>
        /// Service请求接口
        /// </summary>
        public static HttpServerUtilityBase Service
        {
            get;
            set;
        }
        #endregion

        #region 功能函数
        /// <summary>
        /// 读取物理文件路径
        /// </summary>
        /// <param name="fileName">文件物理地址</param>
        /// <returns>文件内容</returns>
        public static string ReadFileString(string fileName)
        {
            using (var sr = new StreamReader(fileName))
            {
                return sr.ReadToEnd();
            }
        }
        #endregion

        #region 文件路径

        /// <summary>
        /// 数字彩奖期信息文件地址
        /// </summary>
        /// <param name="gameCode">彩种</param>
        /// <param name="issuseNumber">期号</param>
        /// <returns>数字彩奖期信息文件地址</returns>
        private static string IssueFile(string gameCode, string issuseNumber)
        {

            return Service.MapPath(string.Format("{0}/{1}/{1}_{2}.json", MatchRoot, gameCode, issuseNumber));
        }

        #endregion

        #region 数字彩数据读取
        /// <summary>
        /// 数字彩 - 奖池信息
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="gameCode">彩种</param>
        /// <param name="issuseNumber">期号</param>
        /// <returns>奖池信息</returns>
        public static OpenDataInfo GetOpenData(HttpServerUtilityBase service, string gameCode, string issuseNumber)
        {
            try
            {
                Service = service;
                string json_match = ReadFileString(IssueFile(gameCode, issuseNumber));
                var res = JsonSerializer.Deserialize<OpenDataInfo>(json_match);

                return res;
            }
            catch (Exception)
            {
                return new OpenDataInfo();
            }
        }

        #endregion
    }
}