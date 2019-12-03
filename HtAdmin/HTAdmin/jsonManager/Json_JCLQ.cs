using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MatchBiz.Core;
using Common.JSON;
using System.Configuration;
using System.IO;

namespace HTAdmin.jsonManager
{
    /// <summary>
    /// 竞彩篮球 - json文件读取管理
    /// </summary>
    public static class Json_JCLQ
    {
        #region 属性
        /// <summary>
        /// 竞彩数据存放物理路径根目录
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
        /// 竞彩篮球队伍信息文件地址
        /// </summary>
        /// <param name="type">玩法类型</param>
        /// <param name="matchdate">奖期，如果为空则取根目录比赛结果</param>
        /// <returns>队伍信息文件地址</returns>
        private static string MatchFile(string type, string matchDate = null)
        {
            if (string.IsNullOrEmpty(matchDate))
            {
                return Service.MapPath(MatchRoot + "jclq/Match_" + (type.ToLower() == "sf" ? "sf" : "rfsf") + "_List.json");
            }
            else
            {
                return Service.MapPath(MatchRoot + "/jclq/" + matchDate + "/Match_List.json");
            }
        }

        /// <summary>
        /// 竞彩篮球 - 根据奖期获取队伍结果信息文件地址
        /// </summary>
        /// <param name="matchdate">奖期，如果为空则取根目录比赛结果</param>
        /// <returns>队伍结果信息文件地址</returns>
        private static string MatchResultFile(string matchDate = null)
        {
            if (string.IsNullOrEmpty(matchDate))
            {
                return Service.MapPath(MatchRoot + "jclq/Match_Result_List.json");
            }
            else
            {
                return Service.MapPath(MatchRoot + "/jclq/" + matchDate + "/Match_Result_List.json");
            }
        }

        /// <summary>
        /// 竞彩篮球 - SP文件地址
        /// </summary>
        /// <param name="type">玩法类型</param>
        /// <param name="matchdate">奖期，如果为空则取根目录SP</param>
        /// <returns>SP文件地址</returns>
        private static string SPFile(string type, string matchdate = null)
        {
            if (string.IsNullOrEmpty(matchdate))
            {
                return Service.MapPath(MatchRoot + "jclq/" + type + "_SP.json");
            }
            else
            {
                return Service.MapPath(MatchRoot + "/jclq/" + matchdate + "/" + type + "_SP.json");
            }
        }

        #endregion

        #region 竞彩篮球数据读取
        /// <summary>
        /// 竞彩篮球 - 获取队伍信息列表
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="type">玩法类型</param>
        /// <param name="matchDate">查询日期</param>
        /// <returns>队伍信息列表</returns>
        public static List<JCLQ_MatchInfo> MatchList(HttpServerUtilityBase service, string type, string matchDate = null)
        {
            try
            {
                Service = service;
                string json_match = ReadFileString(MatchFile(type, matchDate));
                var res = JsonSerializer.Deserialize<List<JCLQ_MatchInfo>>(json_match);

                return res;
            }
            catch (Exception)
            {
                return new List<JCLQ_MatchInfo>();
            }
        }

        /// <summary>
        /// 竞彩篮球 - 获取队伍比赛结果信息
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="matchDate">查询日期</param>
        /// <returns>队伍信息列表</returns>
        public static List<JCLQ_MatchResultInfo> MatchResultList(HttpServerUtilityBase service, string matchDate = null)
        {
            try
            {
                Service = service;
                string json_match = ReadFileString(MatchResultFile(matchDate));
                return JsonSerializer.Deserialize<List<JCLQ_MatchResultInfo>>(json_match);
            }
            catch (Exception)
            {
                return new List<JCLQ_MatchResultInfo>();
            }
        }

        /// <summary>
        /// 查询队伍信息与队伍比赛结果信息 - WEB页面使用
        /// - 合并队伍基础信息与队伍结果信息
        /// - 合并各玩法SP数据
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="type">玩法类型</param>
        /// <param name="matchDate">查询日期</param>
        /// <param name="isLeftJoin">是否查询没有结果的队伍比赛信息</param>
        /// <returns>队伍信息及比赛结果信息</returns>
        public static List<JCLQ_MatchInfo_WEB> MatchList_WEB(HttpServerUtilityBase service, string type, string matchDate = null, bool isLeftJoin = true)
        {
            var match = MatchList(service, type, matchDate);
            var matchresult = MatchResultList(service, matchDate);
            var sp_sf = SPList_SF(service, matchDate); //胜负sp数据
            var sp_rfsf = SPList_RFSF(service, matchDate); //让分胜负sp数据
            var sp_sfc = SPList_SFC(service, matchDate); //胜分差sp数据
            var sp_dxf = SPList_DXF(service, matchDate); //大小分sp数据

            var list = new List<JCLQ_MatchInfo_WEB>();
            foreach (var item in match)
            {
                #region 队伍基础信息
                var info = new JCLQ_MatchInfo_WEB()
                {
                    //CreateTime = item.CreateTime,
                    //DSStopBettingTime = item.DSStopBettingTime,
                    //FSStopBettingTime = item.FSStopBettingTime,
                    CreateTime = DateTime.Parse(item.CreateTime),
                    DSStopBettingTime = DateTime.Parse(item.DSStopBettingTime),
                    FSStopBettingTime = DateTime.Parse(item.FSStopBettingTime),
                    GuestTeamName = item.GuestTeamName,
                    HomeTeamName = item.HomeTeamName,
                    LeagueColor = item.LeagueColor.Contains("#") ? item.LeagueColor : "#" + item.LeagueColor,
                    LeagueId = item.LeagueId,
                    LeagueName = item.LeagueName,
                    MatchIdName = item.MatchIdName,
                    //StartDateTime = item.StartDateTime,
                    StartDateTime = DateTime.Parse(item.StartDateTime),
                    MatchData = item.MatchData,
                    MatchId = item.MatchId,
                    MatchNumber = item.MatchNumber,
                    AverageLose = item.AverageLose,
                    AverageWin = item.AverageWin
                };
                #endregion

                #region 附加队伍结果信息
                var res = matchresult.FirstOrDefault(p => p.MatchId == item.MatchId);
                if (res != null)
                {
                    info.DXF_Result = res.DXF_Result;
                    info.DXF_SP = res.DXF_SP;
                    info.DXF_Trend = res.DXF_Trend;
                    info.GuestScore = res.GuestScore;
                    info.HomeScore = res.HomeScore;
                    info.RFSF_Result = res.RFSF_Result;
                    info.RFSF_SP = res.RFSF_SP;
                    info.RFSF_Trend = res.RFSF_Trend;
                    info.SF_Result = res.SF_Result;
                    info.SF_SP = res.SF_SP;
                    info.SFC_Result = res.SFC_Result;
                    info.SFC_SP = res.SFC_SP;
                    info.MatchState = res.MatchState;
                }
                else if (!isLeftJoin)
                {
                    continue;
                }
                #endregion

                #region 附加胜负sp数据
                var sp_sf_item = sp_sf.FirstOrDefault(p => p.MatchId == item.MatchId);
                if (sp_sf_item != null)
                {
                    info.SF_WinSP = sp_sf_item.WinSP;
                    info.SF_LoseSP = sp_sf_item.LoseSP;
                }
                #endregion

                #region 附加让分胜负sp数据
                var sp_rfsf_item = sp_rfsf.FirstOrDefault(p => p.MatchId == item.MatchId);
                if (sp_rfsf_item != null)
                {
                    info.RF = sp_rfsf_item.RF;
                    info.RF_LoseSP = sp_rfsf_item.LoseSP;
                    info.RF_WinSP = sp_rfsf_item.WinSP;
                }
                #endregion

                #region 附加胜分差sp数据
                var sp_sfc_item = sp_sfc.FirstOrDefault(p => p.MatchId == item.MatchId);
                if (sp_sfc_item != null)
                {
                    info.GuestWin1_5 = sp_sfc_item.GuestWin1_5;
                    info.GuestWin11_15 = sp_sfc_item.GuestWin11_15;
                    info.GuestWin16_20 = sp_sfc_item.GuestWin16_20;
                    info.GuestWin21_25 = sp_sfc_item.GuestWin21_25;
                    info.GuestWin26 = sp_sfc_item.GuestWin26;
                    info.GuestWin6_10 = sp_sfc_item.GuestWin6_10;

                    info.HomeWin1_5 = sp_sfc_item.HomeWin1_5;
                    info.HomeWin11_15 = sp_sfc_item.HomeWin11_15;
                    info.HomeWin16_20 = sp_sfc_item.HomeWin16_20;
                    info.HomeWin21_25 = sp_sfc_item.HomeWin21_25;
                    info.HomeWin26 = sp_sfc_item.HomeWin26;
                    info.HomeWin6_10 = sp_sfc_item.HomeWin6_10;
                }
                #endregion

                #region 附加大小分sp数据
                var sp_dxf_item = sp_dxf.FirstOrDefault(p => p.MatchId == item.MatchId);
                if (sp_dxf_item != null)
                {
                    info.DF = sp_dxf_item.DF;
                    info.XF = sp_dxf_item.XF;
                    info.YSZF = sp_dxf_item.YSZF;
                }
                #endregion

                list.Add(info);
            }
            return list;
        }

        #region 竞彩篮球-SP数据
        /// <summary>
        /// 竞彩篮球 - 胜负SP数据
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="matchDate">查询日期</param>
        /// <returns>竞彩篮球-胜负SP数据</returns>
        public static List<JCLQ_SF_SPInfo> SPList_SF(HttpServerUtilityBase service, string matchDate = null)
        {
            try
            {
                Service = service;
                string json_match = ReadFileString(SPFile("sf", matchDate));
                return JsonSerializer.Deserialize<List<JCLQ_SF_SPInfo>>(json_match);
            }
            catch (Exception)
            {
                return new List<JCLQ_SF_SPInfo>();
            }
        }

        /// <summary>
        /// 竞彩篮球 - 让分胜负SP数据
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="matchDate">查询日期</param>
        /// <returns>竞彩篮球-让分胜负SP数据</returns>
        public static List<JCLQ_RFSF_SPInfo> SPList_RFSF(HttpServerUtilityBase service, string matchDate = null)
        {
            try
            {
                Service = service;
                string json_match = ReadFileString(SPFile("rfsf", matchDate));
                return JsonSerializer.Deserialize<List<JCLQ_RFSF_SPInfo>>(json_match);
            }
            catch (Exception)
            {
                return new List<JCLQ_RFSF_SPInfo>();
            }
        }

        /// <summary>
        /// 竞彩篮球 -胜分差SP数据
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="matchDate">查询日期</param>
        /// <returns>竞彩篮球-胜分差SP数据</returns>
        public static List<JCLQ_SFC_SPInfo> SPList_SFC(HttpServerUtilityBase service, string matchDate = null)
        {
            try
            {
                Service = service;
                string json_match = ReadFileString(SPFile("sfc", matchDate));
                return JsonSerializer.Deserialize<List<JCLQ_SFC_SPInfo>>(json_match);
            }
            catch (Exception)
            {
                return new List<JCLQ_SFC_SPInfo>();
            }
        }

        /// <summary>
        /// 竞彩篮球 - 大小分SP数据
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="matchDate">查询日期</param>
        /// <returns>竞彩篮球-大小分SP数据</returns>
        public static List<JCLQ_DXF_SPInfo> SPList_DXF(HttpServerUtilityBase service, string matchDate = null)
        {
            try
            {
                Service = service;
                string json_match = ReadFileString(SPFile("dxf", matchDate));
                return JsonSerializer.Deserialize<List<JCLQ_DXF_SPInfo>>(json_match);
            }
            catch (Exception)
            {
                return new List<JCLQ_DXF_SPInfo>();
            }
        }
        #endregion

        #endregion
    }
}