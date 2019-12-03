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
    /// 竞彩足球 - json文件读取管理
    /// </summary>
    public static class Json_JCZQ
    {
        #region 属性
        /// <summary>
        /// 竞彩数据存放物理路径根目录
        /// </summary>
        public static string MatchRoot
        {
            get
            {
                return ConfigurationManager.AppSettings["MatchRoot"];
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
        /// 竞彩足球队伍信息文件地址
        /// </summary>
        /// <param name="matchdate">奖期，如果为空则取根目录比赛结果</param>
        /// <returns>队伍信息文件地址</returns>
        private static string MatchFile(string matchDate = null)
        {
            if (string.IsNullOrEmpty(matchDate))
            {
                return Service.MapPath(MatchRoot + "jczq/Match_List.json");
            }
            else
            {
                return Service.MapPath(MatchRoot + "/jczq/" + matchDate + "/Match_List.json");
            }
        }

        /// <summary>
        /// 竞彩足球 - 根据奖期获取队伍结果信息文件地址
        /// </summary>
        /// <param name="matchdate">奖期，如果为空则取根目录比赛结果</param>
        /// <returns>队伍结果信息文件地址</returns>
        private static string MatchResultFile(string matchDate = null)
        {
            if (string.IsNullOrEmpty(matchDate))
            {
                return Service.MapPath(MatchRoot + "jczq/Match_Result_List.json");
            }
            else
            {
                return Service.MapPath(MatchRoot + "/jczq/" + matchDate + "/Match_Result_List.json");
            }
        }

        /// <summary>
        /// 竞彩足球 - SP文件地址
        /// </summary>
        /// <param name="type">玩法类型</param>
        /// <param name="matchdate">奖期，如果为空则取根目录SP</param>
        /// <returns>SP文件地址</returns>
        private static string SPFile(string type, string matchdate = null)
        {
            if (string.IsNullOrEmpty(matchdate))
            {
                return Service.MapPath(MatchRoot + "jczq/" + type + "_SP.json");
            }
            else
            {
                return Service.MapPath(MatchRoot + "/jczq/" + matchdate + "/" + type + "_SP.json");
            }
        }

        #endregion

        #region 竞彩足球数据读取
        /// <summary>
        /// 竞彩足球 - 获取队伍信息列表
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="matchDate">查询日期</param>
        /// <returns>队伍信息列表</returns>
        public static List<JCZQ_MatchInfo> MatchList(HttpServerUtilityBase service, string matchDate = null)
        {
            try
            {
                Service = service;
                string json_match = ReadFileString(MatchFile(matchDate));
                var res = JsonSerializer.Deserialize<List<JCZQ_MatchInfo>>(json_match);

                return res;
            }
            catch (Exception)
            {
                return new List<JCZQ_MatchInfo>();
            }
        }

        /// <summary>
        /// 竞彩足球 - 获取队伍比赛结果信息
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="matchDate">查询日期</param>
        /// <returns>队伍信息列表</returns>
        public static List<JCZQ_MatchResultInfo> MatchResultList(HttpServerUtilityBase service, string matchDate = null)
        {
            try
            {
                Service = service;
                string json_match = ReadFileString(MatchResultFile(matchDate));
                return JsonSerializer.Deserialize<List<JCZQ_MatchResultInfo>>(json_match);
            }
            catch (Exception)
            {
                return new List<JCZQ_MatchResultInfo>();
            }
        }

        /// <summary>
        /// 查询队伍信息与队伍比赛结果信息 - WEB页面使用
        /// - 合并队伍基础信息与队伍结果信息
        /// - 合并各玩法SP数据
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="matchDate">查询日期</param>
        /// <param name="isLeftJoin">是否查询没有结果的队伍比赛信息</param>
        /// <returns>队伍信息及比赛结果信息</returns>
        public static List<JCZQ_MatchInfo_WEB> MatchList_WEB(HttpServerUtilityBase service, string matchDate = null, bool isLeftJoin = true)
        {
            var match = MatchList(service, matchDate);
            var matchresult = MatchResultList(service, matchDate);
            var sp_spf = SPList_SPF(service, matchDate); //胜平负sp数据
            var sp_zjq = SPList_ZJQ(service, matchDate); //总进球sp数据
            var sp_bf = SPList_BF(service, matchDate); //比分sp数据
            var sp_bqc = SPList_BQC(service, matchDate); //半全场sp数据

            var list = new List<JCZQ_MatchInfo_WEB>();
            foreach (var item in match)
            {
                #region 队伍基础信息
                var info = new JCZQ_MatchInfo_WEB()
                {
                    //CreateTime = item.CreateTime,
                    //DSStopBettingTime = item.DSStopBettingTime,
                    //FSStopBettingTime = item.FSStopBettingTime,
                    CreateTime = DateTime.Parse(item.CreateTime),
                    DSStopBettingTime = DateTime.Parse(item.DSStopBettingTime),
                    FSStopBettingTime = DateTime.Parse(item.FSStopBettingTime),
                    GuestTeamId = item.GuestTeamId,
                    GuestTeamName = item.GuestTeamName,
                    HomeTeamId = item.HomeTeamId,
                    HomeTeamName = item.HomeTeamName,
                    LeagueColor = item.LeagueColor,
                    LeagueId = item.LeagueId,
                    LeagueName = item.LeagueName,
                    LetBall = item.LetBall,
                    LoseOdds = item.LoseOdds,
                    MatchIdName = item.MatchIdName,
                    StartDateTime = DateTime.Parse(item.StartDateTime),
                    //StartDateTime = item.StartDateTime,
                    WinOdds = item.WinOdds,
                    FlatOdds = item.FlatOdds,
                    MatchData = item.MatchData,
                    MatchId = item.MatchId,
                    MatchNumber = item.MatchNumber
                };
                #endregion

                #region 附加队伍结果信息
                var res = matchresult.FirstOrDefault(p => p.MatchId == item.MatchId);
                if (res != null)
                {
                    info.ZJQ_Result = res.ZJQ_Result;
                    info.ZJQ_SP = res.ZJQ_SP;
                    info.SPF_SP = res.SPF_SP;
                    info.SPF_Result = res.SPF_Result;
                    info.BQC_SP = res.BQC_SP;
                    info.BQC_Result = res.BQC_Result;
                    info.BF_SP = res.BF_SP;
                    info.BF_Result = res.BF_Result;
                    info.FullGuestTeamScore = res.FullGuestTeamScore;
                    info.FullHomeTeamScore = res.FullHomeTeamScore;
                    info.HalfGuestTeamScore = res.HalfGuestTeamScore;
                    info.HalfHomeTeamScore = res.HalfHomeTeamScore;
                    info.MatchState = res.MatchState;
                }
                else if (!isLeftJoin)
                {
                    continue;
                }
                #endregion

                #region 附加胜平负sp数据
                var sp_spf_item = sp_spf.FirstOrDefault(p => p.MatchId == item.MatchId);
                if (sp_spf_item != null)
                {
                    info.SP_Win_Odds = sp_spf_item.WinOdds;
                    info.SP_Lose_Odds = sp_spf_item.LoseOdds;
                    info.SP_Flat_Odds = sp_spf_item.FlatOdds;
                }
                #endregion

                #region 附加总进球sp数据
                var sp_zjq_item = sp_zjq.FirstOrDefault(p => p.MatchId == item.MatchId);
                if (sp_zjq_item != null)
                {
                    info.JinQiu_0_Odds = sp_zjq_item.JinQiu_0_Odds;
                    info.JinQiu_1_Odds = sp_zjq_item.JinQiu_1_Odds;
                    info.JinQiu_2_Odds = sp_zjq_item.JinQiu_2_Odds;
                    info.JinQiu_3_Odds = sp_zjq_item.JinQiu_3_Odds;
                    info.JinQiu_4_Odds = sp_zjq_item.JinQiu_4_Odds;
                    info.JinQiu_5_Odds = sp_zjq_item.JinQiu_5_Odds;
                    info.JinQiu_6_Odds = sp_zjq_item.JinQiu_6_Odds;
                    info.JinQiu_7_Odds = sp_zjq_item.JinQiu_7_Odds;
                }
                #endregion

                #region 附加比分sp数据
                var sp_bf_item = sp_bf.FirstOrDefault(p => p.MatchId == item.MatchId);
                if (sp_bf_item != null)
                {
                    info.F_01 = sp_bf_item.F_01;
                    info.F_02 = sp_bf_item.F_02;
                    info.F_03 = sp_bf_item.F_03;
                    info.F_04 = sp_bf_item.F_04;
                    info.F_05 = sp_bf_item.F_05;
                    info.F_12 = sp_bf_item.F_12;
                    info.F_13 = sp_bf_item.F_13;
                    info.F_14 = sp_bf_item.F_14;
                    info.F_15 = sp_bf_item.F_15;
                    info.F_23 = sp_bf_item.F_23;
                    info.F_24 = sp_bf_item.F_24;
                    info.F_25 = sp_bf_item.F_25;
                    info.F_QT = sp_bf_item.F_QT;
                    info.P_00 = sp_bf_item.P_00;
                    info.P_11 = sp_bf_item.P_11;
                    info.P_22 = sp_bf_item.P_22;
                    info.P_33 = sp_bf_item.P_33;
                    info.P_QT = sp_bf_item.P_QT;
                    info.S_10 = sp_bf_item.S_10;
                    info.S_20 = sp_bf_item.S_20;
                    info.S_21 = sp_bf_item.S_21;
                    info.S_30 = sp_bf_item.S_30;
                    info.S_31 = sp_bf_item.S_31;
                    info.S_32 = sp_bf_item.S_32;
                    info.S_40 = sp_bf_item.S_40;
                    info.S_41 = sp_bf_item.S_41;
                    info.S_42 = sp_bf_item.S_42;
                    info.S_50 = sp_bf_item.S_50;
                    info.S_51 = sp_bf_item.S_51;
                    info.S_52 = sp_bf_item.S_52;
                    info.S_QT = sp_bf_item.S_QT;
                }
                #endregion

                #region 附加半全场sp数据
                var sp_bqc_item = sp_bqc.FirstOrDefault(p => p.MatchId == item.MatchId);
                if (sp_bqc_item != null)
                {
                    info.F_F_Odds = sp_bqc_item.F_F_Odds;
                    info.F_P_Odds = sp_bqc_item.F_P_Odds;
                    info.F_SH_Odds = sp_bqc_item.F_SH_Odds;
                    info.P_F_Odds = sp_bqc_item.P_F_Odds;
                    info.P_P_Odds = sp_bqc_item.P_P_Odds;
                    info.P_SH_Odds = sp_bqc_item.P_SH_Odds;
                    info.SH_F_Odds = sp_bqc_item.SH_F_Odds;
                    info.SH_P_Odds = sp_bqc_item.SH_P_Odds;
                    info.SH_SH_Odds = sp_bqc_item.SH_SH_Odds;
                }
                #endregion

                list.Add(info);
            }
            return list;
        }

        #region 竞彩足球-SP数据
        /// <summary>
        /// 竞彩足球 - 胜平负SP数据
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="matchDate">查询日期</param>
        /// <returns>竞彩足球-胜平负SP数据</returns>
        public static List<JCZQ_SPF_SPInfo> SPList_SPF(HttpServerUtilityBase service, string matchDate = null)
        {
            try
            {
                Service = service;
                string json_match = ReadFileString(SPFile("spf", matchDate));
                return JsonSerializer.Deserialize<List<JCZQ_SPF_SPInfo>>(json_match);
            }
            catch (Exception)
            {
                return new List<JCZQ_SPF_SPInfo>();
            }
        }

        /// <summary>
        /// 竞彩足球 - 总进球SP数据
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="matchDate">查询日期</param>
        /// <returns>竞彩足球-总进球SP数据</returns>
        public static List<JCZQ_ZJQ_SPInfo> SPList_ZJQ(HttpServerUtilityBase service, string matchDate = null)
        {
            try
            {
                Service = service;
                string json_match = ReadFileString(SPFile("zjq", matchDate));
                return JsonSerializer.Deserialize<List<JCZQ_ZJQ_SPInfo>>(json_match);
            }
            catch (Exception)
            {
                return new List<JCZQ_ZJQ_SPInfo>();
            }
        }

        /// <summary>
        /// 竞彩足球 - 比分SP数据
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="matchDate">查询日期</param>
        /// <returns>竞彩足球-比分SP数据</returns>
        public static List<JCZQ_BF_SPInfo> SPList_BF(HttpServerUtilityBase service, string matchDate = null)
        {
            try
            {
                Service = service;
                string json_match = ReadFileString(SPFile("bf", matchDate));
                return JsonSerializer.Deserialize<List<JCZQ_BF_SPInfo>>(json_match);
            }
            catch (Exception)
            {
                return new List<JCZQ_BF_SPInfo>();
            }
        }

        /// <summary>
        /// 竞彩足球 - 半全场SP数据
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="matchDate">查询日期</param>
        /// <returns>竞彩足球-半全场SP数据</returns>
        public static List<JCZQ_BQC_SPInfo> SPList_BQC(HttpServerUtilityBase service, string matchDate = null)
        {
            try
            {
                Service = service;
                string json_match = ReadFileString(SPFile("bqc", matchDate));
                return JsonSerializer.Deserialize<List<JCZQ_BQC_SPInfo>>(json_match);
            }
            catch (Exception)
            {
                return new List<JCZQ_BQC_SPInfo>();
            }
        }
        #endregion

        #endregion
    }
}