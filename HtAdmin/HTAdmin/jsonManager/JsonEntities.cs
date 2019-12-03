using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameBiz.Core;
using System.Collections;
using MatchBiz.Core;

namespace HTAdmin.jsonManager
{
    /// <summary>
    /// 北单队伍信息
    /// </summary>
    public class BJDC_MatchInfo_WEB
    {
        //队伍基础信息
        public DateTime CreateTime { get; set; }
        public decimal FlatOdds { get; set; }
        public string GuestTeamName { get; set; }
        public string GuestTeamSort { get; set; }
        public string HomeTeamName { get; set; }
        public string HomeTeamSort { get; set; }
        public string Id { get; set; }
        public string IssuseNumber { get; set; }
        public int LetBall { get; set; }
        public DateTime LocalStopTime { get; set; }
        public decimal LoseOdds { get; set; }
        public string MatchColor { get; set; }
        public string MatchName { get; set; }
        public int MatchOrderId { get; set; }
        public DateTime MatchStartTime { get; set; }
        public BJDCMatchState MatchState { get; set; }
        public decimal WinOdds { get; set; }

        //队伍结果信息
        public string BF_Result { get; set; }
        public decimal BF_SP { get; set; }
        public string BQC_Result { get; set; }
        public decimal BQC_SP { get; set; }
        public string GuestFull_Result { get; set; }
        public string GuestHalf_Result { get; set; }
        public string HomeFull_Result { get; set; }
        public string HomeHalf_Result { get; set; }
        public string MatchStateName { get; set; }
        public string SPF_Result { get; set; }
        public decimal SPF_SP { get; set; }
        public string SXDS_Result { get; set; }
        public decimal SXDS_SP { get; set; }
        public string ZJQ_Result { get; set; }
        public decimal ZJQ_SP { get; set; }
        public DateTime LotteryTime { get; set; }

        //胜平负-SP数据
        public decimal SP_Flat_Odds { get; set; }
        public decimal SP_Lose_Odds { get; set; }
        public decimal SP_Win_Odds { get; set; }

        //总进球-SP数据
        public decimal JinQiu_0_Odds { get; set; }
        public decimal JinQiu_1_Odds { get; set; }
        public decimal JinQiu_2_Odds { get; set; }
        public decimal JinQiu_3_Odds { get; set; }
        public decimal JinQiu_4_Odds { get; set; }
        public decimal JinQiu_5_Odds { get; set; }
        public decimal JinQiu_6_Odds { get; set; }
        public decimal JinQiu_7_Odds { get; set; }

        //上下单双-SP数据
        public decimal SH_D_Odds { get; set; }
        public decimal SH_S_Odds { get; set; }
        public decimal X_D_Odds { get; set; }
        public decimal X_S_Odds { get; set; }

        //比分-SP数据
        public decimal F_01 { get; set; }
        public decimal F_02 { get; set; }
        public decimal F_03 { get; set; }
        public decimal F_04 { get; set; }
        public decimal F_12 { get; set; }
        public decimal F_13 { get; set; }
        public decimal F_14 { get; set; }
        public decimal F_23 { get; set; }
        public decimal F_24 { get; set; }
        public decimal F_QT { get; set; }
        public decimal P_00 { get; set; }
        public decimal P_11 { get; set; }
        public decimal P_22 { get; set; }
        public decimal P_33 { get; set; }
        public decimal P_QT { get; set; }
        public decimal S_10 { get; set; }
        public decimal S_20 { get; set; }
        public decimal S_21 { get; set; }
        public decimal S_30 { get; set; }
        public decimal S_31 { get; set; }
        public decimal S_32 { get; set; }
        public decimal S_40 { get; set; }
        public decimal S_41 { get; set; }
        public decimal S_42 { get; set; }
        public decimal S_QT { get; set; }

        //半全场-SP数据
        public decimal F_F_Odds { get; set; }
        public decimal F_P_Odds { get; set; }
        public decimal F_SH_Odds { get; set; }
        public decimal P_F_Odds { get; set; }
        public decimal P_P_Odds { get; set; }
        public decimal P_SH_Odds { get; set; }
        public decimal SH_F_Odds { get; set; }
        public decimal SH_P_Odds { get; set; }
        public decimal SH_SH_Odds { get; set; }
    }

    /// <summary>
    /// 传统足球队伍信息
    /// </summary>
    public class CTZQ_MatchInfo_WEB
    {
        //队伍基础信息
        public string Color { get; set; }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public int GuestTeamHalfScore { get; set; }
        public string GuestTeamId { get; set; }
        public string GuestTeamName { get; set; }
        public int GuestTeamScore { get; set; }
        public string GuestTeamStanding { get; set; }
        public int HomeTeamHalfScore { get; set; }
        public string HomeTeamId { get; set; }
        public string HomeTeamName { get; set; }
        public int HomeTeamScore { get; set; }
        public string HomeTeamStanding { get; set; }
        public string Id { get; set; }
        public string IssuseNumber { get; set; }
        public string MatchName { get; set; }
        public string MatchResult { get; set; }
        public DateTime MatchStartTime { get; set; }
        public CTZQMatchState MatchState { get; set; }
        public int OrderNumber { get; set; }
        public DateTime UpdateTime { get; set; }

        //队伍平均赔率数据
        public string AverageOdds { get; set; }
        public string FullAverageOdds { get; set; }
        public string HalfAverageOdds { get; set; }
        public decimal KLFlat { get; set; }
        public decimal KLLose { get; set; }
        public decimal KLWin { get; set; }
        public decimal LSFlat { get; set; }
        public decimal LSLose { get; set; }
        public decimal LSWin { get; set; }
        public string YPSW { get; set; }
    }

    /// <summary>
    /// 竞彩足球队伍信息
    /// </summary>
    public class JCZQ_MatchInfo_WEB
    {
        //队伍基础信息
        public DateTime CreateTime { get; set; }
        public DateTime DSStopBettingTime { get; set; }
        public string MatchData { get; set; }
        public string MatchId { get; set; }
        public string MatchNumber { get; set; }
        public decimal FlatOdds { get; set; }
        public DateTime FSStopBettingTime { get; set; }
        public int GuestTeamId { get; set; }
        public string GuestTeamName { get; set; }
        public int HomeTeamId { get; set; }
        public string HomeTeamName { get; set; }
        public string LeagueColor { get; set; }
        public int LeagueId { get; set; }
        public string LeagueName { get; set; }
        public int LetBall { get; set; }
        public decimal LoseOdds { get; set; }
        public string MatchIdName { get; set; }
        public DateTime StartDateTime { get; set; }
        public decimal WinOdds { get; set; }

        //队伍结果信息
        public string BF_Result { get; set; }
        public decimal BF_SP { get; set; }
        public string BQC_Result { get; set; }
        public decimal BQC_SP { get; set; }
        public int FullGuestTeamScore { get; set; }
        public int FullHomeTeamScore { get; set; }
        public int HalfGuestTeamScore { get; set; }
        public int HalfHomeTeamScore { get; set; }
        public string MatchState { get; set; }
        public string SPF_Result { get; set; }
        public decimal SPF_SP { get; set; }
        public string ZJQ_Result { get; set; }
        public decimal ZJQ_SP { get; set; }

        //胜平负-SP数据
        public decimal SP_Flat_Odds { get; set; }
        public decimal SP_Lose_Odds { get; set; }
        public decimal SP_Win_Odds { get; set; }

        //总进球-SP数据
        public decimal JinQiu_0_Odds { get; set; }
        public decimal JinQiu_1_Odds { get; set; }
        public decimal JinQiu_2_Odds { get; set; }
        public decimal JinQiu_3_Odds { get; set; }
        public decimal JinQiu_4_Odds { get; set; }
        public decimal JinQiu_5_Odds { get; set; }
        public decimal JinQiu_6_Odds { get; set; }
        public decimal JinQiu_7_Odds { get; set; }

        //比分-SP数据
        public decimal F_01 { get; set; }
        public decimal F_02 { get; set; }
        public decimal F_03 { get; set; }
        public decimal F_04 { get; set; }
        public decimal F_05 { get; set; }
        public decimal F_12 { get; set; }
        public decimal F_13 { get; set; }
        public decimal F_14 { get; set; }
        public decimal F_15 { get; set; }
        public decimal F_23 { get; set; }
        public decimal F_24 { get; set; }
        public decimal F_25 { get; set; }
        public decimal F_QT { get; set; }
        public decimal P_00 { get; set; }
        public decimal P_11 { get; set; }
        public decimal P_22 { get; set; }
        public decimal P_33 { get; set; }
        public decimal P_QT { get; set; }
        public decimal S_10 { get; set; }
        public decimal S_20 { get; set; }
        public decimal S_21 { get; set; }
        public decimal S_30 { get; set; }
        public decimal S_31 { get; set; }
        public decimal S_32 { get; set; }
        public decimal S_40 { get; set; }
        public decimal S_41 { get; set; }
        public decimal S_42 { get; set; }
        public decimal S_50 { get; set; }
        public decimal S_51 { get; set; }
        public decimal S_52 { get; set; }
        public decimal S_QT { get; set; }

        //半全场-SP数据
        public decimal F_F_Odds { get; set; }
        public decimal F_P_Odds { get; set; }
        public decimal F_SH_Odds { get; set; }
        public decimal P_F_Odds { get; set; }
        public decimal P_P_Odds { get; set; }
        public decimal P_SH_Odds { get; set; }
        public decimal SH_F_Odds { get; set; }
        public decimal SH_P_Odds { get; set; }
        public decimal SH_SH_Odds { get; set; }
    }

    /// <summary>
    /// 竞彩篮球队伍信息
    /// </summary>
    public class JCLQ_MatchInfo_WEB
    {
        //队伍基础信息
        public decimal AverageLose { get; set; }
        public decimal AverageWin { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime DSStopBettingTime { get; set; }
        public DateTime FSStopBettingTime { get; set; }
        public string GuestTeamName { get; set; }
        public string HomeTeamName { get; set; }
        public string LeagueColor { get; set; }
        public int LeagueId { get; set; }
        public string LeagueName { get; set; }
        public string MatchIdName { get; set; }
        public string MatchData { get; set; }
        public string MatchId { get; set; }
        public string MatchNumber { get; set; }
        public DateTime StartDateTime { get; set; }

        //队伍结果信息
        public string DXF_Result { get; set; }
        public decimal DXF_SP { get; set; }
        public string DXF_Trend { get; set; }
        public int GuestScore { get; set; }
        public int HomeScore { get; set; }
        public string MatchState { get; set; }
        public string RFSF_Result { get; set; }
        public decimal RFSF_SP { get; set; }
        public string RFSF_Trend { get; set; }
        public string SF_Result { get; set; }
        public decimal SF_SP { get; set; }
        public string SFC_Result { get; set; }
        public decimal SFC_SP { get; set; }

        //胜负-SP数据
        public decimal SF_LoseSP { get; set; }
        public decimal SF_WinSP { get; set; }

        //让分胜负-SP数据
        public decimal RF_LoseSP { get; set; }
        public decimal RF { get; set; }
        public decimal RF_WinSP { get; set; }

        //胜分差-SP数据
        public decimal GuestWin1_5 { get; set; }
        public decimal GuestWin11_15 { get; set; }
        public decimal GuestWin16_20 { get; set; }
        public decimal GuestWin21_25 { get; set; }
        public decimal GuestWin26 { get; set; }
        public decimal GuestWin6_10 { get; set; }
        public decimal HomeWin1_5 { get; set; }
        public decimal HomeWin11_15 { get; set; }
        public decimal HomeWin16_20 { get; set; }
        public decimal HomeWin21_25 { get; set; }
        public decimal HomeWin26 { get; set; }
        public decimal HomeWin6_10 { get; set; }

        //大小分-SP数据
        public decimal DF { get; set; }
        public decimal XF { get; set; }
        public decimal YSZF { get; set; }
    }
}