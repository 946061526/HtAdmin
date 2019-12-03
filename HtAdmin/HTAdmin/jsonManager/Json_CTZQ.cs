using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MatchBiz.Core;
using Common.JSON;
using System.IO;
using System.Configuration;
using Common.Net;
using System.Text;

namespace HTAdmin.jsonManager
{
    /// <summary>
    /// 传统足球 - json文件读取管理
    /// </summary>
    public static class Json_CTZQ
    {
        #region 属性
        /// <summary>
        /// 竞彩数据存放物理路径根目录
        /// </summary>
        public static string MatchRoot
        {
            get
            {
                //return ConfigurationManager.AppSettings["MatchRoot"] ?? "~/MatchData/";
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
        public static string ReadFileString(string fullUrl)
        {
            //using (var sr = new StreamReader(fileName))
            //{
            //    return sr.ReadToEnd();
            //}
            try
            {
                string strResult = PostManager.Get(fullUrl, Encoding.UTF8);
                if (strResult == "404") return string.Empty;

                if (!string.IsNullOrEmpty(strResult))
                {
                    if (strResult.ToLower().StartsWith("var"))
                    {
                        string[] strArray = strResult.Split('=');
                        if (strArray != null && strArray.Length == 2)
                        {
                            if (strArray[1].ToString().Trim().EndsWith(";"))
                            {
                                return strArray[1].ToString().Trim().TrimEnd(';');
                            }
                            return strArray[1].ToString().Trim();
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return string.Empty;
        }
        #endregion

        #region 文件路径
        /// <summary>
        /// 传统足球 - 奖期数据文件
        /// </summary>
        private static string IssuseFile(string type)
        {
            //return Service.MapPath(MatchRoot + "ctzq/Match_" + type + "_Issuse_List.json");
            return MatchRoot + "/ctzq/Match_" + type + "_Issuse_List.json";
        }

        /// <summary>
        /// 传统足球 - 根据奖期获取队伍信息文件地址
        /// </summary>
        /// <returns>队伍信息文件地址</returns>
        private static string MatchFile(string type, string issuse)
        {
            //return Service.MapPath(MatchRoot + "ctzq" + "/" + issuse + "/Match_" + type + "_List.json");
            return MatchRoot + "/ctzq" + "/" + issuse + "/Match_" + type + "_List.json";
        }

        /// <summary>
        /// 传统足球 - 根据奖期获取队伍平均赔率数据
        /// </summary>
        /// <returns>队伍信息文件地址</returns>
        private static string OddFiles(string type, string issuse)
        {
            //return Service.MapPath(MatchRoot + "ctzq" + "/" + issuse + "/Match_" + type + "_Odds_List.json");
            return MatchRoot + "/ctzq" + "/" + issuse + "/Match_" + type + "_Odds_List.json";
        }

        /// <summary>
        /// 传统足球 - 开奖结果文件
        /// </summary>
        /// <returns>开奖结果文件地址</returns>
        private static string BonusFile(string type, string issuse)
        {
            //return Service.MapPath(MatchRoot + "ctzq" + "/" + issuse + "/CTZQ_" + type + "_BonusLevel.json");
            return MatchRoot + "/ctzq" + "/" + issuse + "/CTZQ_" + type + "_BonusLevel.json";
        }
        #endregion

        #region 传统足球数据读取
        /// <summary>
        /// 传统足球 - 奖期数据列表
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <returns>奖期数据列表</returns>
        public static List<CTZQ_IssuseInfo> IssuseList(HttpServerUtilityBase service, string type)
        {
            try
            {
                Service = service;
                string json_issuse = ReadFileString(IssuseFile(type));
                return JsonSerializer.Deserialize<List<CTZQ_IssuseInfo>>(json_issuse);
            }
            catch (Exception ex)
            {
                return new List<CTZQ_IssuseInfo>();
            }
        }

        /// <summary>
        /// 传统足球 - 获取队伍信息列表
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="issuse">期号</param>
        /// <param name="type">玩法</param>
        /// <returns>队伍信息列表</returns>
        public static List<CTZQ_MatchInfo> MatchList(HttpServerUtilityBase service, string issuse, string type)
        {
            try
            {
                Service = service;
                string json_match = ReadFileString(MatchFile(type, issuse));
                return JsonSerializer.Deserialize<List<CTZQ_MatchInfo>>(json_match);
            }
            catch (Exception)
            {
                return new List<CTZQ_MatchInfo>();
            }
        }

        /// <summary>
        /// 传统足球 - 获取队伍平均赔率数据
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="issuse">期号</param>
        /// <param name="type">玩法</param>
        /// <returns>队伍平均赔率数据</returns>
        public static List<CTZQ_OddInfo> OddsList(HttpServerUtilityBase service, string issuse, string type)
        {
            try
            {
                Service = service;
                string json_match = ReadFileString(OddFiles(type, issuse));
                return JsonSerializer.Deserialize<List<CTZQ_OddInfo>>(json_match);
            }
            catch (Exception)
            {
                return new List<CTZQ_OddInfo>();
            }
        }

        /// <summary>
        /// 查询队伍信息与平均赔率数据 - WEB页面使用
        /// - 合并队伍基础信息与平均赔率数据
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="issuse">期数</param>
        /// <param name="type">彩种玩法类型</param>
        /// <returns>队伍信息及平均赔率数据</returns>
        public static List<CTZQ_MatchInfo_WEB> MatchList_WEB(HttpServerUtilityBase service, string issuse, string type)
        {
            var match = MatchList(service, issuse, type);
            var odds = OddsList(service, issuse, type);

            var list = new List<CTZQ_MatchInfo_WEB>();
            foreach (var item in match)
            {
                var res = odds.FirstOrDefault(p => p.Id == item.Id);

                #region 队伍基础信息
                var info = new CTZQ_MatchInfo_WEB()
                {
                    GameCode = item.GameCode,
                    Color = item.Color,
                    GameType = item.GameType,
                    OrderNumber = item.OrderNumber,
                    Id = item.Id,
                    //UpdateTime = item.UpdateTime,
                    UpdateTime = DateTime.Parse(item.UpdateTime),
                    MatchName = item.MatchName,
                    MatchResult = item.MatchResult,
                    //MatchStartTime = item.MatchStartTime,
                    MatchStartTime = DateTime.Parse(item.MatchStartTime),
                    MatchState = item.MatchState,
                    HomeTeamHalfScore = item.HomeTeamHalfScore,
                    HomeTeamId = item.HomeTeamId,
                    HomeTeamName = item.HomeTeamName,
                    HomeTeamScore = item.HomeTeamScore,
                    HomeTeamStanding = item.HomeTeamStanding,
                    GuestTeamHalfScore = item.GuestTeamHalfScore,
                    GuestTeamId = item.GuestTeamId,
                    GuestTeamName = item.GuestTeamName,
                    GuestTeamScore = item.GuestTeamScore,
                    GuestTeamStanding = item.GuestTeamStanding,
                    IssuseNumber = item.IssuseNumber
                };
                #endregion

                #region 附加平均赔率数据
                if (res != null)
                {
                    info.AverageOdds = res.AverageOdds;
                    info.FullAverageOdds = res.FullAverageOdds;
                    info.HalfAverageOdds = res.HalfAverageOdds;
                    info.KLFlat = res.KLFlat;
                    info.KLLose = res.KLLose;
                    info.KLWin = res.KLWin;
                    info.LSFlat = res.LSFlat;
                    info.LSLose = res.LSLose;
                    info.LSWin = res.LSWin;
                    info.YPSW = res.YPSW;
                }
                #endregion

                list.Add(info);
            }
            return list;
        }

        /// <summary>
        /// 传统足球 - 获取开奖结果信息
        /// </summary>
        /// <param name="service"></param>
        /// <param name="issuse"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<CTZQ_BonusLevelInfo> BonusLevelList(HttpServerUtilityBase service, string issuse, string type)
        {
            try
            {
                Service = service;
                string json_match = ReadFileString(BonusFile(type, issuse));
                return JsonSerializer.Deserialize<List<CTZQ_BonusLevelInfo>>(json_match);
            }
            catch
            {
                return new List<CTZQ_BonusLevelInfo>();
            }
        }

        #endregion
    }
}