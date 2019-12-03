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
    /// 北单 - json文件读取管理
    /// </summary>
    public static class Json_BJDC
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
        /// 北单 - 奖期数据文件
        /// </summary>
        private static string IssuseFile
        {
            get
            {
                return Service.MapPath(MatchRoot + "bjdc/Match_IssuseNumber_List.json");
            }
        }

        /// <summary>
        /// 北单 - 根据奖期获取队伍信息文件地址
        /// </summary>
        /// <param name="issuse">期数</param>
        /// <returns>队伍信息文件地址</returns>
        private static string MatchFile(string issuse)
        {
            return Service.MapPath(MatchRoot + "bjdc" + "/" + issuse + "/Match_List.json");
        }

        /// <summary>
        /// 北单 - 根据奖期获取队伍结果信息文件地址
        /// </summary>
        /// <param name="issuse">期数</param>
        /// <returns>队伍结果信息文件地址</returns>
        private static string MatchResultFile(string issuse)
        {
            return Service.MapPath(MatchRoot + "bjdc" + "/" + issuse + "/MatchResult_List.json");
        }

        /// <summary>
        /// 北单 - SP文件地址
        /// </summary>
        /// <param name="type">玩法类型</param>
        /// <param name="issuse">期数</param>
        /// <returns>SP文件地址</returns>
        private static string SPFile(string type, string issuse)
        {
            return Service.MapPath(MatchRoot + "bjdc" + "/" + issuse + "/SP_" + type + ".json");
        }

        /// <summary>
        /// 北单 - SP走势文件地址
        /// </summary>
        /// <param name="type">玩法类型</param>
        /// <param name="issuse">期数</param>
        /// <param name="matchId">场次ID</param>
        /// <returns>SP文件地址</returns>
        private static string SPTrendFile(string type, string issuse, string matchId)
        {
            return Service.MapPath(MatchRoot + "bjdc" + "/" + issuse + "/" + type + "_SP_Trend_" + matchId + ".json");
        }

        #endregion

        #region 北单数据读取
        /// <summary>
        /// 北单 - 奖期数据列表
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <returns>奖期数据列表</returns>
        public static List<BJDC_IssuseInfo> IssuseList(HttpServerUtilityBase service)
        {
            try
            {
                Service = service;
                string json_issuse = ReadFileString(IssuseFile);
                return JsonSerializer.Deserialize<List<BJDC_IssuseInfo>>(json_issuse).OrderByDescending(a => a.IssuseNumber).ToList();
            }
            catch (Exception ex)
            {
                return new List<BJDC_IssuseInfo>();
            }
        }

        /// <summary>
        /// 北单 - 获取队伍信息列表
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="issuse">期号</param>
        /// <returns>队伍信息列表</returns>
        public static List<BJDC_MatchInfo> MatchList(HttpServerUtilityBase service, string issuse)
        {
            try
            {
                Service = service;
                string json_match = ReadFileString(MatchFile(issuse));
                return JsonSerializer.Deserialize<List<BJDC_MatchInfo>>(json_match);
            }
            catch (Exception)
            {
                return new List<BJDC_MatchInfo>();
            }
        }

        /// <summary>
        /// 北单 - 获取队伍比赛结果信息
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="issuse">期号</param>
        /// <returns>队伍信息列表</returns>
        public static List<BJDC_MatchResultInfo> MatchResultList(HttpServerUtilityBase service, string issuse)
        {
            try
            {
                Service = service;
                string json_match = ReadFileString(MatchResultFile(issuse));
                return JsonSerializer.Deserialize<List<BJDC_MatchResultInfo>>(json_match);
            }
            catch (Exception)
            {
                return new List<BJDC_MatchResultInfo>();
            }
        }

        /// <summary>
        /// 查询队伍信息与队伍比赛结果信息 - WEB页面使用
        /// - 合并队伍基础信息与队伍结果信息
        /// - 合并各玩法SP数据
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="issuse">期数</param>
        /// <param name="isLeftJoin">是否查询没有结果的队伍比赛信息</param>
        /// <returns>队伍信息及比赛结果信息</returns>
        public static List<BJDC_MatchInfo_WEB> MatchList_WEB(HttpServerUtilityBase service, string issuse, bool isLeftJoin = true)
        {
            var match = MatchList(service, issuse);
            var matchresult = MatchResultList(service, issuse);
            var sp_spf = SPList_SPF(service, issuse); //胜平负sp数据
            var sp_zjq = SPList_ZJQ(service, issuse); //总进球sp数据
            var sp_sxds = SPList_SXDS(service, issuse); //上下单双sp数据
            var sp_bf = SPList_BF(service, issuse); //比分sp数据
            var sp_bqc = SPList_BQC(service, issuse); //半全场sp数据

            var list = new List<BJDC_MatchInfo_WEB>();
            foreach (var item in match)
            {
                var res = matchresult.FirstOrDefault(p => p.Id == item.Id);

                #region 队伍基础信息
                var info = new BJDC_MatchInfo_WEB()
                {
                    //CreateTime = item.CreateTime,
                    CreateTime = DateTime.Parse(item.CreateTime),
                    FlatOdds = item.FlatOdds,
                    GuestTeamName = item.GuestTeamName,
                    GuestTeamSort = item.GuestTeamSort,
                    HomeTeamName = item.HomeTeamName,
                    HomeTeamSort = item.HomeTeamSort,
                    Id = item.Id,
                    IssuseNumber = item.IssuseNumber,
                    LetBall = item.LetBall,
                    MatchState = item.MatchState,
                    MatchColor = item.MatchColor,
                    MatchName = item.MatchName,
                    LocalStopTime = DateTime.Parse(item.LocalStopTime),
                    //LocalStopTime = item.LocalStopTime,
                    LoseOdds = item.LoseOdds,
                    MatchOrderId = item.MatchOrderId,
                    MatchStartTime = DateTime.Parse(item.MatchStartTime),
                    //MatchStartTime = item.MatchStartTime,
                    WinOdds = item.WinOdds
                };
                #endregion

                #region 附加队伍结果信息
                if (res != null)
                {
                    info.ZJQ_Result = res.ZJQ_Result;
                    info.ZJQ_SP = res.ZJQ_SP;
                    info.SXDS_SP = res.SXDS_SP;
                    info.SXDS_Result = res.SXDS_Result;
                    info.SPF_SP = res.SPF_SP;
                    info.SPF_Result = res.SPF_Result;
                    info.MatchStateName = res.MatchState;
                    info.GuestHalf_Result = res.GuestHalf_Result;
                    info.GuestFull_Result = res.GuestFull_Result;
                    info.BQC_SP = res.BQC_SP;
                    info.BQC_Result = res.BQC_Result;
                    info.BF_SP = res.BF_SP;
                    info.BF_Result = res.BF_Result;
                    info.HomeFull_Result = res.HomeFull_Result;
                    info.HomeHalf_Result = res.HomeHalf_Result;
                    //info.LotteryTime = res.CreateTime;
                    info.LotteryTime = DateTime.Parse(res.CreateTime);
                }
                else if (!isLeftJoin)
                {
                    continue;
                }
                #endregion

                #region 附加胜平负sp数据
                var sp_spf_item = sp_spf.FirstOrDefault(p => p.MatchOrderId == item.MatchOrderId);
                if (sp_spf_item != null)
                {
                    info.SP_Win_Odds = sp_spf_item.Win_Odds;
                    info.SP_Lose_Odds = sp_spf_item.Lose_Odds;
                    info.SP_Flat_Odds = sp_spf_item.Flat_Odds;
                }
                #endregion

                #region 附加总进球sp数据
                var sp_zjq_item = sp_zjq.FirstOrDefault(p => p.MatchOrderId == item.MatchOrderId);
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

                #region 附加上下单双sp数据
                var sp_sxds_item = sp_sxds.FirstOrDefault(p => p.MatchOrderId == item.MatchOrderId);
                if (sp_sxds_item != null)
                {
                    info.SH_D_Odds = sp_sxds_item.SH_D_Odds;
                    info.SH_S_Odds = sp_sxds_item.SH_S_Odds;
                    info.X_D_Odds = sp_sxds_item.X_D_Odds;
                    info.X_S_Odds = sp_sxds_item.X_S_Odds;
                }
                #endregion

                #region 附加比分sp数据
                var sp_bf_item = sp_bf.FirstOrDefault(p => p.MatchOrderId == item.MatchOrderId);
                if (sp_bf_item != null)
                {
                    info.F_01 = sp_bf_item.F_01;
                    info.F_02 = sp_bf_item.F_02;
                    info.F_03 = sp_bf_item.F_03;
                    info.F_04 = sp_bf_item.F_04;
                    info.F_12 = sp_bf_item.F_12;
                    info.F_13 = sp_bf_item.F_13;
                    info.F_14 = sp_bf_item.F_14;
                    info.F_23 = sp_bf_item.F_23;
                    info.F_24 = sp_bf_item.F_24;
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
                    info.S_QT = sp_bf_item.S_QT;
                }
                #endregion

                #region 附加半全场sp数据
                var sp_bqc_item = sp_bqc.FirstOrDefault(p => p.MatchOrderId == item.MatchOrderId);
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

        #region 北单SP数据
        /// <summary>
        /// 北单 - 胜平负SP数据
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="issuse">期号</param>
        /// <returns>北单胜平负SP数据</returns>
        public static List<BJDC_SPF_SpInfo> SPList_SPF(HttpServerUtilityBase service, string issuse)
        {
            try
            {
                Service = service;
                string json_match = ReadFileString(SPFile("spf", issuse));
                return JsonSerializer.Deserialize<List<BJDC_SPF_SpInfo>>(json_match);
            }
            catch (Exception)
            {
                return new List<BJDC_SPF_SpInfo>();
            }
        }

        /// <summary>
        /// 北单 - 总进球SP数据
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="issuse">期号</param>
        /// <returns>北单总进球SP数据</returns>
        public static List<BJDC_ZJQ_SpInfo> SPList_ZJQ(HttpServerUtilityBase service, string issuse)
        {
            try
            {
                Service = service;
                string json_match = ReadFileString(SPFile("zjq", issuse));
                return JsonSerializer.Deserialize<List<BJDC_ZJQ_SpInfo>>(json_match);
            }
            catch (Exception)
            {
                return new List<BJDC_ZJQ_SpInfo>();
            }
        }

        /// <summary>
        /// 北单 - 上下单双SP数据
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="issuse">期号</param>
        /// <returns>北单上下单双SP数据</returns>
        public static List<BJDC_SXDS_SpInfo> SPList_SXDS(HttpServerUtilityBase service, string issuse)
        {
            try
            {
                Service = service;
                string json_match = ReadFileString(SPFile("sxds", issuse));
                return JsonSerializer.Deserialize<List<BJDC_SXDS_SpInfo>>(json_match);
            }
            catch (Exception)
            {
                return new List<BJDC_SXDS_SpInfo>();
            }
        }

        /// <summary>
        /// 北单 - 比分SP数据
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="issuse">期号</param>
        /// <returns>北单比分SP数据</returns>
        public static List<BJDC_BF_SpInfo> SPList_BF(HttpServerUtilityBase service, string issuse)
        {
            try
            {
                Service = service;
                string json_match = ReadFileString(SPFile("bf", issuse));
                return JsonSerializer.Deserialize<List<BJDC_BF_SpInfo>>(json_match);
            }
            catch (Exception)
            {
                return new List<BJDC_BF_SpInfo>();
            }
        }

        /// <summary>
        /// 北单 - 半全场SP数据
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="issuse">期号</param>
        /// <returns>北单半全场SP数据</returns>
        public static List<BJDC_BQC_SpInfo> SPList_BQC(HttpServerUtilityBase service, string issuse)
        {
            try
            {
                Service = service;
                string json_match = ReadFileString(SPFile("bqc", issuse));
                return JsonSerializer.Deserialize<List<BJDC_BQC_SpInfo>>(json_match);
            }
            catch (Exception)
            {
                return new List<BJDC_BQC_SpInfo>();
            }
        }
        #endregion

        #region 北单SP走势数据
        /// <summary>
        /// 北单 - 胜平负SP走势数据
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="issuse">期号</param>
        /// <param name="matchId">场次ID</param>
        /// <returns>北单胜平负SP数据</returns>
        public static List<BJDC_SPF_SpInfo> Trend_SPList_SPF(HttpServerUtilityBase service, string issuse, string matchId)
        {
            try
            {
                Service = service;
                string json_match = ReadFileString(SPTrendFile("spf", issuse, matchId));
                return JsonSerializer.Deserialize<List<BJDC_SPF_SpInfo>>(json_match);
            }
            catch (Exception)
            {
                return new List<BJDC_SPF_SpInfo>();
            }
        }

        /// <summary>
        /// 北单 - 总进球SP走势数据
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="issuse">期号</param>
        /// <param name="matchId">场次ID</param>
        /// <returns>北单总进球SP数据</returns>
        public static List<BJDC_ZJQ_SpInfo> Trend_SPList_ZJQ(HttpServerUtilityBase service, string issuse, string matchId)
        {
            try
            {
                Service = service;
                string json_match = ReadFileString(SPTrendFile("zjq", issuse, matchId));
                return JsonSerializer.Deserialize<List<BJDC_ZJQ_SpInfo>>(json_match);
            }
            catch (Exception)
            {
                return new List<BJDC_ZJQ_SpInfo>();
            }
        }

        /// <summary>
        /// 北单 - 上下单双SP走势数据
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="issuse">期号</param>
        /// <param name="matchId">场次ID</param>
        /// <returns>北单上下单双SP数据</returns>
        public static List<BJDC_SXDS_SpInfo> Trend_SPList_SXDS(HttpServerUtilityBase service, string issuse, string matchId)
        {
            try
            {
                Service = service;
                string json_match = ReadFileString(SPTrendFile("sxds", issuse, matchId));
                return JsonSerializer.Deserialize<List<BJDC_SXDS_SpInfo>>(json_match);
            }
            catch (Exception)
            {
                return new List<BJDC_SXDS_SpInfo>();
            }
        }

        /// <summary>
        /// 北单 - 比分SP走势数据
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="issuse">期号</param>
        /// <param name="matchId">场次ID</param>
        /// <returns>北单比分SP数据</returns>
        public static List<BJDC_BF_SpInfo> Trend_SPList_BF(HttpServerUtilityBase service, string issuse, string matchId)
        {
            try
            {
                Service = service;
                string json_match = ReadFileString(SPTrendFile("bf", issuse, matchId));
                return JsonSerializer.Deserialize<List<BJDC_BF_SpInfo>>(json_match);
            }
            catch (Exception)
            {
                return new List<BJDC_BF_SpInfo>();
            }
        }

        /// <summary>
        /// 北单 - 半全场SP走势数据
        /// </summary>
        /// <param name="service">HttpServerUtilityBase对象</param>
        /// <param name="issuse">期号</param>
        /// <param name="matchId">场次ID</param>
        /// <returns>北单半全场SP数据</returns>
        public static List<BJDC_BQC_SpInfo> Trend_SPList_BQC(HttpServerUtilityBase service, string issuse, string matchId)
        {
            try
            {
                Service = service;
                string json_match = ReadFileString(SPTrendFile("bqc", issuse, matchId));
                return JsonSerializer.Deserialize<List<BJDC_BQC_SpInfo>>(json_match);
            }
            catch (Exception)
            {
                return new List<BJDC_BQC_SpInfo>();
            }
        }
        #endregion

        #endregion
    }
}