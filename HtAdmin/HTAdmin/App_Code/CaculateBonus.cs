using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameBiz.Core;
using Common.Lottery;


public static class CaculateBonus
{
    public static decimal SportsScheme(Sports_SchemeQueryInfo sports_Scheme, Sports_AnteCodeQueryInfoCollection sports_AnteCodeQuery)
    {
        //var ggfsArr = sports_Scheme.PlayType.Split('|');
        //foreach (var ggfs in ggfsArr)
        //{
        //    int baseCount = int.Parse(ggfs.Split('_')[0].ToString());
        //    var analyzer = AnalyzerFactory.GetSportAnalyzer(sports_Scheme.GameCode, sports_Scheme.GameType, baseCount);

        //    IList<SportResultWeb> sportResult = new List<SportResultWeb>();
        //    IList<SportAnteCodeWeb> sportAnteCode = new List<SportAnteCodeWeb>();

        //    foreach (var item in sports_AnteCodeQuery)
        //    {
        //        sportAnteCode.Add(new SportAnteCodeWeb() { AnteCode = item.AnteCode, GameType = item.GameType, IsDan = item.IsDan, MatchId = item.MatchId, Odds = item.CurrentSp });
        //        var sportResultInfo = new SportResultWeb();
        //        sportResultInfo.BF_Result = "";
        //        sportResultInfo.BQC_Result = "";
        //        sportResultInfo.DXF_Result = "";
        //        sportResultInfo.GuestTeamScore = "";
        //        sportResultInfo.HomeTeamScore = "";
        //        sportResultInfo.MatchIdentity = "";
        //        sportResultInfo.MatchIndex = 0;
        //        sportResultInfo.BF_Result = "";


        //    }


        //    //var bonusResult = analyzer.CaculateBonus(sports_AnteCodeQuery.ToArray(), matchList.ToArray());

        //}


        return 0;
    }
}

public class SportResultWeb : ISportResult
{
    public int MatchIndex { get; set; }
    // 比赛唯一编号
    public string MatchIdentity { get; set; }

    #region 比赛
    // 主队得分
    public string HomeTeamScore { get; set; }
    // 客队得分
    public string GuestTeamScore { get; set; }
    #endregion

    #region 结果
    // 胜平负结果:3,1,0
    public string SPF_Result { get; set; }
    // 进球数结果
    public string ZJQ_Result { get; set; }
    // 单双比分结果
    public string SXDS_Result { get; set; }
    // 比分结果
    public string BF_Result { get; set; }
    // 半全场结果
    public string BQC_Result { get; set; }

    // 胜负结果:3,1,0
    public string SF_Result { get; set; }
    // 让分胜负结果
    public string RFSF_Result { get; set; }
    // 比分结果
    public string SFC_Result { get; set; }
    // 半全场结果
    public string DXF_Result { get; set; }
    #endregion

    public string GetFullMatchScore(string gameCode)
    {
        switch (gameCode)
        {
            case "BJDC":
                return MatchIndex.ToString();
            case "JCZQ":
            case "JCLQ":
                return MatchIdentity;
            default:
                throw new ArgumentException("获取比赛编号，不支持的彩种 - " + gameCode);
        }
    }

    public string GetMatchId(string gameCode)
    {
        return HomeTeamScore + ":" + GuestTeamScore;
    }

    public string GetMatchResult(string gameCode, string gameType, decimal offset = -1m)
    {
        gameCode = gameCode.ToUpper();
        gameType = gameType.ToUpper();
        switch (gameCode)
        {
            case "BJDC":
                switch (gameType)
                {
                    case "SPF":
                        return SPF_Result;
                    case "ZJQ":
                        return ZJQ_Result;
                    case "SXDS":
                        return SXDS_Result;
                    case "BF":
                        return BF_Result;
                    case "BQC":
                        return BQC_Result;
                    default:
                        throw new ArgumentException("获取北单比赛结果，不支持的玩法 - " + gameCode + "-" + gameType);
                }
            case "JCZQ":
                switch (gameType)
                {
                    case "SPF":
                        return SPF_Result;
                    case "ZJQ":
                        return ZJQ_Result;
                    case "BF":
                        return BF_Result;
                    case "BQC":
                        return BQC_Result;
                    default:
                        throw new ArgumentException("获取竞彩足球比赛结果，不支持的玩法 - " + gameCode + "-" + gameType);
                }
            case "JCLQ":
                switch (gameType)
                {
                    case "SF":
                        return SF_Result;
                    case "RFSF":
                        if (offset != -1)
                        {
                            var host1 = decimal.Parse(HomeTeamScore);
                            var guest1 = decimal.Parse(GuestTeamScore);
                            if (host1 + offset > guest1)
                            {
                                return "3";
                            }
                            else if (host1 + offset < guest1)
                            {
                                return "0";
                            }
                        }
                        return RFSF_Result;
                    case "SFC":
                        return SFC_Result;
                    case "DXF":
                        if (offset != -1)
                        {
                            var host2 = decimal.Parse(HomeTeamScore);
                            var guest2 = decimal.Parse(GuestTeamScore);
                            if (host2 + guest2 > offset)
                            {
                                return "3";
                            }
                            else if (host2 + guest2 < offset)
                            {
                                return "0";
                            }
                        }
                        return DXF_Result;
                    default:
                        throw new ArgumentException("获取竞彩篮球比赛结果，不支持的玩法 - " + gameCode + "-" + gameType);
                }
            default:
                throw new ArgumentException("获取比赛结果，不支持的彩种 - " + gameCode);
        }
    }
}

public class SportAnteCodeWeb : ISportAnteCode
{
    public string AnteCode { get; set; }
    public string GameType { get; set; }
    public bool IsDan { get; set; }
    public int Length
    {
        get { return AnteCode.Split(',').Length; }
    }
    public string MatchId { get; set; }
    public string Odds { get; set; }

    public decimal GetResultOdds(string matchResult)
    {
        var tmp = Odds.Split(',');
        foreach (var item in tmp)
        {
            var p = item.Split('|');
            if (p[0].Equals(matchResult, StringComparison.OrdinalIgnoreCase))
            {
                return decimal.Parse(p[1]);
            }
        }
        throw new Exception(string.Format("没找到比赛{0}结果对应的赔率 - {1}", MatchId, matchResult));
    }
    public string GetMatchResult(string gameCode, string gameType, string score)
    {
        if (gameCode.ToLower() != "jclq")
        {
            return "";
        }
        if (score == "-" || score == "*")
        {
            return "*";
        }
        var tmp = score.Split(':');
        if (tmp.Length != 2)
        {
            throw new ArgumentException("比分格式错误");
        }
        if (tmp[0] == "-" || tmp[1] == "-" || tmp[0] == "-1" || tmp[1] == "-1")
        {
            return "*";
        }
        var score1 = decimal.Parse(tmp[0]);
        var score2 = decimal.Parse(tmp[1]);
        var total = score1 + score2;
        if (gameType.ToLower() == "rfsf")
        {
            var rf = GetResultOdds("rf");
            if (score1 + rf > score2)
            {
                return "3";
            }
            else if (score1 + rf < score2)
            {
                return "0";
            }
            else
            {
                return "1";
            }
        }
        else if (gameType.ToLower() == "dxf")
        {
            var yszf = GetResultOdds("yszf");
            if (total > yszf)
            {
                return "3";
            }
            else if (total < yszf)
            {
                return "0";
            }
            else
            {
                return "1";
            }
        }
        else
        {
            return "";
        }
    }
}


