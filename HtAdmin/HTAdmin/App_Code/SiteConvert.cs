using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using GameBiz.Core;
using External.Core;
using Common.Lottery;
using Common.Algorithms;

/// <summary>
/// 网站编码转换类
/// </summary>
public static class SiteConvert
{
    /// <summary>
    /// 把英文星期格式转换为中文格式
    /// </summary>
    /// <param name="week">Monday</param>
    /// <returns>星期一</returns>
    public static string Week_CN(DayOfWeek week)
    {
        var tmp = "";
        switch (week)
        {
            case DayOfWeek.Monday: tmp = "一";
                break;
            case DayOfWeek.Tuesday: tmp = "二";
                break;
            case DayOfWeek.Wednesday: tmp = "三";
                break;
            case DayOfWeek.Thursday: tmp = "四";
                break;
            case DayOfWeek.Friday: tmp = "五";
                break;
            case DayOfWeek.Saturday: tmp = "六";
                break;
            case DayOfWeek.Sunday: tmp = "日";
                break;
            default:
                tmp = week.ToString();
                break;
        }
        return "星期" + tmp;
    }

    /// <summary>
    /// 资讯类型编码转换为中文名称
    /// </summary>
    /// <param name="category">资讯:INFO  技巧:TECH  预测:Expert</param>
    /// <returns>中文编码</returns>
    public static string ZX_Category(string category)
    {
        string status = string.Empty;
        switch (category)
        {
            case "INFO":
                status = "赛事资讯";
                break;
            case "ZDZT":
                status = "置顶主题";
                break;
            case "CPZJ":
                status = "彩票中奖";
                break;
            case "HOT":
                status = "热点彩讯";
                break;
            case "ZQDP":
                status = "足球点评";
                break;
            case "LQDP":
                status = "篮球点评";
                break;
            case "SZZX":
                status = "数字资讯";
                break;
            default: status = category;
                break;
        }
        return status;
    }

    /// <summary>
    /// 根据彩种编码（玩法类型）获取彩种（玩法）名称
    /// </summary>
    /// <param name="gamecode">彩种编码</param>
    /// <param name="type">玩法编码，可为空</param>
    /// <returns>彩种（玩法）名称</returns>
    public static string GameName(string gamecode, string type = "")
    {
        if (string.IsNullOrEmpty(gamecode))
        {
            return "";
        }
        type = string.IsNullOrEmpty(type) ? gamecode : type;
        //根据彩种编号获取彩种名称
        switch (gamecode.ToLower())
        {
            case "cqssc": return "时时彩";
            case "jxssc": return "新时时彩";
            case "sd11x5": return "老11选5";
            case "gd11x5": return "新11选5";
            case "jx11x5": return "11选5";
            case "pl3": return "排列三";
            case "fc3d": return "福彩3D";
            case "ssq": return "双色球";
            case "qxc": return "七星彩";
            case "qlc": return "七乐彩";
            case "dlt": return "大乐透";
            case "sdqyh": return "群英会";
            case "gdklsf": return "快乐十分";
            case "gxklsf": return "广西快乐十分";
            case "jsks": return "江苏快3";
            case "sdklpk3": return "山东快乐扑克3";
            case "ozb": return "欧洲杯";
            case "sjb": return "世界杯";
            case "jczq":
                switch (type.ToLower())
                {
                    case "spf": return "竞彩让球胜平负";
                    case "brqspf": return "竞彩胜平负";
                    case "bf": return "竞彩比分";
                    case "zjq": return "竞彩总进球数";
                    case "bqc": return "竞彩半全场";
                    case "hh": return "竞彩混合过关";
                    default: return "竞彩足球";
                }
            case "jclq":
                switch (type.ToLower())
                {
                    case "sf": return "篮球胜负";
                    case "rfsf": return "篮球让分胜负";
                    case "sfc": return "篮球胜分差";
                    case "dxf": return "篮球大小分";

                    case "hh": return "篮球混合过关";                   
                    default: return "竞彩篮球";
                }
            case "ctzq":
                switch (type.ToLower())
                {
                    case "t14c": return "14场胜负";
                    case "tr9": return "任9场";
                    case "t6bqc": return "6场半全";
                    case "t4cjq": return "4场进球";
                    default: return "传统足球";
                }
            case "bjdc":
                switch (type.ToLower())
                {
                    case "sxds": return "单场上下单双";
                    case "spf": return "单场胜平负";
                    case "zjq": return "单场总进球";
                    case "bf": return "单场比分";
                    case "bqc": return "单场半全场";
                    default: return "北京单场";
                }
            default: return gamecode;
        }
    }

    /// <summary>
    /// 分析投注号码，返回注数
    /// </summary>
    /// <param name="gamecode"></param>
    /// <param name="type"></param>
    /// <param name="anteCode"></param>
    /// <returns></returns>
    public static int AnalyzeAnteCodeBetCount(string gamecode, string type, string anteCode)
    {
        var analyzer = AnalyzerFactory.GetAntecodeAnalyzer(gamecode, type);
        return analyzer.AnalyzeAnteCode(anteCode);
    }

    #region 资金状态
    /// <summary>
    /// 转换账户类型名称
    /// </summary>
    /// <param name="type">账户类型</param>
    /// <returns>账户类型名称</returns>
    public static string AccountTypeName(AccountType type)
    {
        switch (type)
        {
            //case AccountType.Activity:
            //    return "活动账户";
            case AccountType.Bonus:
                return "奖金账户";
            case AccountType.Commission:
                return "返点账户";
            //case AccountType.Common:
            //    return "通用账户";
            case AccountType.Freeze:
                return "冻结账户";
            default:
                return type.ToString();
        }
    }

    /// <summary>
    /// 转换充值订单状态
    /// </summary>
    /// <param name="fillMoneyStatus">充值订单状态</param>
    /// <returns>充值订单状态显示名称</returns>
    public static string FillMoneyStatusName(FillMoneyStatus fillMoneyStatus)
    {
        switch (fillMoneyStatus)
        {
            case FillMoneyStatus.Success:
                return "已成功";
            case FillMoneyStatus.Failed:
                return "已失败";
            case FillMoneyStatus.Requesting:
                return "待付款";
            default:
                return fillMoneyStatus.ToString();
        }
    }

    /// <summary>
    /// 转换充值接口类型名称
    /// </summary>
    /// <param name="fillAgent">充值接口类型</param>
    /// <returns>充值接口类型名称</returns>
    public static string FillAgentName(FillMoneyAgentType fillAgent)
    {
        //充值订单接口
        string agent = fillAgent.ToString();
        switch (fillAgent)
        {
            case FillMoneyAgentType.Alipay:
                agent = "支付宝";
                break;
            case FillMoneyAgentType.AlipayWAP:
                agent = "支付宝-WAP";
                break;
            case FillMoneyAgentType.CallsPay:
                agent = "手机充值卡";
                break;
            case FillMoneyAgentType.ChinaPay:
                agent = "网银在线";
                break;
            case FillMoneyAgentType.KuanQian:
                agent = "快钱支付";
                break;
            case FillMoneyAgentType.ManualDeduct:
                agent = "系统扣款";
                break;
            case FillMoneyAgentType.ManualFill:
                agent = "手工充值";
                break;
            case FillMoneyAgentType.Tenpay:
                agent = "财付通";
                break;
            case FillMoneyAgentType.Yeepay:
                agent = "易宝支付";
                break;
            case FillMoneyAgentType.IPS:
                agent = "环迅充值";
                break;
            case FillMoneyAgentType.IPS_Bank:
                agent = "环迅网银";
                break;
            default:
                agent = fillAgent.ToString();
                break;
        }
        return agent;
    }

    /// <summary>
    /// 转换提现状态名称
    /// </summary>
    /// <param name="withdrawStatus">提现状态</param>
    /// <returns>提现状态名称</returns>
    public static string WithdrawStatusName(WithdrawStatus withdrawStatus)
    {
        //转换提现状态
        string status = string.Empty;
        switch (withdrawStatus)
        {
            case WithdrawStatus.Requesting:
                status = "请求中";
                break;
            case WithdrawStatus.Success:
                status = "已成功";
                break;
            case WithdrawStatus.Refused:
                status = "已拒绝";
                break;
            default:
                status = withdrawStatus.ToString();
                break;
        }
        return status;
    }
    #endregion

    #region 竞彩状态
    /// <summary>
    /// 转换北单截止状态显示
    /// </summary>
    /// <param name="salestate">北单比赛状态</param>
    /// <param name="stoptime">比赛截止时间</param>
    /// <param name="matchstatename">状态编码</param>
    /// <returns>北单截止时间或截止状态</returns>
    //public static string bjdc_stoptime(BJDCMatchState salestate, DateTime stoptime, string matchstatename)
    //{
    //    switch (salestate)
    //    {
    //        case BJDCMatchState.Stop:
    //            {
    //                switch (matchstatename)
    //                {
    //                    case "Finish":
    //                        return "完场";
    //                    case "Late":
    //                        return "延迟";
    //                    case "Cancel":
    //                        return "取消";
    //                    default:
    //                        return DateTime.Now >= stoptime ? "截止" : stoptime.ToString("HH:mm");
    //                }
    //            }
    //        default:
    //            return DateTime.Now >= stoptime ? "截止" : stoptime.ToString("HH:mm");
    //    }
    //}

    /// <summary>
    /// 转换竞彩截止状态显示
    /// </summary>
    /// <param name="stoptime">截止时间</param>
    /// <param name="matchstate">状态编码</param>
    /// <returns>竞彩截止时间或截止状态</returns>
    public static string jc_stoptime(DateTime stoptime, string matchstate)
    {
        if (string.IsNullOrEmpty(matchstate))
        {
            return DateTime.Now >= stoptime ? "截止" : stoptime.ToString("HH:mm");
        }
        switch (matchstate.ToLower())
        {
            case "2":
                return "完场";
            case "3":
                return "取消";
            case "4":
                return "推迟";
            default:
                return DateTime.Now >= stoptime ? "截止" : stoptime.ToString("HH:mm");
        }
    }

    /// <summary>
    /// 转换竞彩截止状态显示
    /// </summary>
    /// <param name="stoptime">截止时间</param>
    /// <param name="matchstate">状态编码</param>
    /// <returns>竞彩截止时间或截止状态</returns>
    public static string jc_matchstate(DateTime stoptime, string matchstate)
    {
        if (string.IsNullOrEmpty(matchstate))
        {
            return string.Empty;
        }
        switch (matchstate.ToLower())
        {
            case "3":
                return "取消";
            case "4":
                return "推迟";
            default:
                return string.Empty;
        }
    }

    /// <summary>
    /// 北单sp结果值显示
    /// </summary>
    /// <param name="tag">目标值</param>
    /// <param name="result">结果值</param>
    /// <param name="sp">sp值</param>
    /// <returns>北单sp结果</returns>
    public static string bjdc_sp_result(string tag, string result, decimal sp)
    {
        return tag == result ? sp.ToString("f4") : string.Empty;
    }

    /// <summary>
    /// 转换过关方式名称
    /// </summary>
    /// <param name="playType">过关方式</param>
    /// <returns>过关方式名称</returns>
    public static string PlayTypeName(string playType)
    {
        var plays = playType.Replace("P", "").Replace("|", ",").Split(',');
        var tmp = new List<string>();
        foreach (var item in plays)
        {
            if (item == "1_1")
            {
                tmp.Add("单关");
            }
            else if (item == "0_1")
            {
                tmp.Add("混合过关");
            }
            else
            {
                tmp.Add(item.Replace("_", "串"));
            }
        }
        return string.Join(",", tmp);
    }

    /// <summary>
    /// 根据原过关方式，获取拆分串关列表，每个元素为串关数，表示N串1
    /// </summary>
    /// <param name="playType">原过关方式</param>
    /// <returns></returns>
    public static int[] GetPlayTypeList(string playType)
    {
        var comb = new Combination();
        try
        {
            List<int> tmp = new List<int>();
            if (playType.Split(',').Length == 1 && !playType.EndsWith("_1"))
            {
                var nps = playType.Replace("P", "").Split('_');
                var gs = int.Parse(nps[0]);
                var zs = int.Parse(nps[1]);
                tmp = SportAnalyzer.AnalyzeChuan(gs, zs).ToList();

                //var allZS = 0;
                //for (int i = gs; i >= 1; i--)
                //{
                //    tmp.Add(i.ToString());
                //    allZS += comb.Calculate(gs, i);
                //    if (allZS == zs)
                //    { break; }
                //}
            }
            else
            {
                tmp = playType.Replace("P", "").Replace("_1", "").Split(',').Select(a => int.Parse(a)).ToList();
            }

            return tmp.OrderByDescending(a => a).ToArray(); ;
        }
        catch
        {
            return new int[] { };
        }
    }
    #endregion

    #region 系统设置 wb 2018.06.29

    /// <summary>
    /// 把第三方账号类别转换为中文格式
    /// </summary>
    public static string GetThirdPartyAccountCategory(ThirdPartyAccountCategory category)
    {
        var result = "";
        switch (category)
        {
            case ThirdPartyAccountCategory.SMSInterface: result = "短信接口";
                break;
            case ThirdPartyAccountCategory.SpeechVerificationCode: result = "语音验证码";
                break;
            case ThirdPartyAccountCategory.PictureVerificationCode: result = "图片验证码";
                break;
            case ThirdPartyAccountCategory.DigitalLotteryAward: result = "数字彩开奖";
                break;
            case ThirdPartyAccountCategory.CompetitiveLottery: result = "竞技彩开奖";
                break;
            case ThirdPartyAccountCategory.PaymentInterface: result = "支付接口";
                break;
            case ThirdPartyAccountCategory.Push: result = "推送接口";
                break;
            case ThirdPartyAccountCategory.OnlineService: result = "在线客服";
                break;
            case ThirdPartyAccountCategory.BaiduTongJi: result = "百度统计";
                break;
            default:
                result = "其他账号";
                break;
        }
        return result;
    }

    /// <summary>
    /// 把场景转换为中文格式
    /// </summary>
    /// <returns></returns>
    public static string GetPaymentInterfaceClient(ClientType type)
    {
        var result = "";
        switch (type)
        {
            case ClientType.PC: result = "PC端";
                break;
            case ClientType.M: result = "M端";
                break;
            case ClientType.APP: result = "APP端";
                break;
            default :
                break;
        }
        return result;
    }

    #endregion

    #region 推送 wb 2018.07.06

    public static string GetPushType(PushType pushType)
    {
        var result = "";
        switch (pushType)
        {
            case PushType.Unicast: result = "单播";
                break;
            case PushType.Broadcast: result = "广播";
                break;
            default:
                break;
        }
        return result;
    }

    public static string GetAfterOpenType(AfterOpenType afterOpenType)
    {
        var result = "";
        switch (afterOpenType)
        {
            case AfterOpenType.GoApp: result = "唤醒APP";
                break;
            case AfterOpenType.GoUrl: result = "跳转到URL";
                break;
            case AfterOpenType.GoActivity: result = "跳转到页面";
                break;
            case AfterOpenType.GoCustom: result = "自定义内容";
                break;
            default:
                break;
        }
        return result;
    }

    #endregion

    /// <summary>
    /// 合买形式
    /// </summary>
    /// <param name="jointype"></param>
    /// <returns></returns>
    public static string JoinTypeName(TogetherJoinType jointype)
    {
        switch (jointype)
        {
            case TogetherJoinType.Subscription:
                return "方案预投";
            case TogetherJoinType.FollowerJoin:
                return "自动跟单";
            case TogetherJoinType.Join:
                return "参与合买";
            case TogetherJoinType.SystemGuarantees:
                return "系统保底";
            case TogetherJoinType.Guarantees:
                return "方案保底";
            default:
                return jointype.ToString();
        }
    }
}