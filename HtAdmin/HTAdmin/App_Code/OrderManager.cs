using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using GameBiz.Core;



/// <summary>
/// 订单管理类
/// </summary>
public static class OrderManager
{
    #region 订单属性获取转换
    /// <summary>
    /// 转换方案类型名称
    /// </summary>
    /// <param name="schemeType">方案类型</param>
    /// <returns>方案类型名称</returns>
    public static string SchemeTypeName(SchemeType schemeType, bool isSimple = false)
    {
        switch (schemeType)
        {
            case SchemeType.ChaseBetting:
                return isSimple ? "追" : "追号";
            case SchemeType.GeneralBetting:
                return isSimple ? "" : "代购";
            case SchemeType.TogetherBetting:
                return isSimple ? "合" : "合买";
case SchemeType.SingleCopy:
                return isSimple ? "" : "抄单";            default:
                return schemeType.ToString();
        }
    }

    /// <summary>
    /// 转换方案来源名称
    /// </summary>
    /// <param name="schemeCategory">方案来源</param>
    /// <returns>方案来源名称</returns>
    public static string SchemeCategoryName(SchemeBettingCategory schemeCategory, bool isSimple = false)
    {
        switch (schemeCategory)
        {
            case SchemeBettingCategory.GeneralBetting:
                return isSimple ? "" : "普通投注";
            case SchemeBettingCategory.FilterBetting:
                return isSimple ? "滤" : "过滤投注";
            case SchemeBettingCategory.SingleBetting:
                return isSimple ? "单" : "单式投注";
            default:
                return schemeCategory.ToString();
        }
    }


    /// <summary>
    /// 彩种类型
    /// </summary>
    /// <param name="gameCode">彩种编码</param>
    /// <returns>彩种类型</returns>
    public static GameCategory GameCategory(string gameCode)
    {
        switch (gameCode.ToLower())
        {
            case "jczq":
            case "jclq":
            case "bjdc":
                return global::GameCategory.北单竞彩;
            case "ctzq":
                return global::GameCategory.传统足球;
            default:
                return global::GameCategory.数字彩;
        }
    }

    /// <summary>
    /// 获取传统足球订单详情投注号码CSS样式名称
    /// </summary>
    /// <param name="matchResult">开奖结果</param>
    /// <param name="code">本场投注号码</param>
    /// <param name="tag">目标开奖号码，胜3 平1 负0，以此类推</param>
    /// <returns>样式名称</returns>
    public static string CTZQ_GetAnteCodeCSS(string matchResult, string code, string tag)
    {
        return code.Contains(tag) ? (string.IsNullOrEmpty(matchResult) || matchResult == tag || matchResult == "*") ? "pass_on" : "pass_off" : string.Empty;
    }

    /// <summary>
    /// 传统足球四场进球-彩果转换显示
    /// </summary>
    /// <param name="score"></param>
    /// <returns></returns>
    public static string CTZQ_CG_T4CJQ(int score)
    {
        return score < 0 ? string.Empty : score > 3 ? "3" : score.ToString();
    }

    /// <summary>
    /// 传统足球六场半全场-彩果转换显示
    /// </summary>
    /// <param name="score"></param>
    /// <returns></returns>
    public static string CTZQ_CG_T6BQC(int homeScore, int guestScore)
    {
        if (homeScore < 0 || guestScore < 0)
        {
            return string.Empty;
        }
        return homeScore > guestScore ? "3" : homeScore == guestScore ? "1" : "0";
    }

    /// <summary>
    /// 转换订单显示状态
    /// </summary>
    /// <param name="progressStatus">方案状态</param>
    /// <param name="ticketStatus">出票状态</param>
    /// <returns>显示状态</returns>
    public static string OrderStatusName(SchemeType schemeType, ProgressStatus progressStatus, TicketStatus ticketStatus = TicketStatus.Ticketed, BonusStatus bonusStatus = BonusStatus.Waitting, bool isPrizeMoney = false, bool isMine = true, bool isViturl = false)
    {
        if (isViturl) return "已撤单";  //如果是虚拟订单，则该订单为已撤单

        switch (progressStatus)
        {
            case ProgressStatus.Waitting: return "待开始";
            case ProgressStatus.AutoStop: return "自动停止";
            case ProgressStatus.Aborted: return "撤单";
            case ProgressStatus.Running:
            case ProgressStatus.Complate:
                switch (ticketStatus)
                {
                    case TicketStatus.Waitting: return "待投注";
                    case TicketStatus.Ticketing: return "投注中";
                    case TicketStatus.Ticketed:
                        if (progressStatus == ProgressStatus.Complate)
                        {
                            switch (bonusStatus)
                            {
                                case BonusStatus.Waitting: return "未开奖";
                                case BonusStatus.Awarding: return "开奖中";
                                case BonusStatus.Error: return "开奖错误";
                                case BonusStatus.Lose: return "未中奖";
                                case BonusStatus.Win: return isPrizeMoney ? "已派奖" : "已中奖";
                                default: return "已完成";
                            }
                        }
                        else
                        {
                            return (schemeType == SchemeType.TogetherBetting && !isMine ? "已跟单" : "已出票");
                        }
case TicketStatus.PrintTicket: return "已打票";                    case TicketStatus.Skipped: return "被跳过";
                    case TicketStatus.Error: return "出票失败";
                    case TicketStatus.Abort: return "撤单";
                    default: return "待投注";
                }
            default: return "待开始";
        }
    }

    /// <summary>
    /// 转换中奖显示名称
    /// </summary>
    /// <param name="bonusStatus">中奖状态</param>
    /// <returns>中奖显示名称</returns>
    public static string BonusStatusName(BonusStatus bonusStatus)
    {
        switch (bonusStatus)
        {
            case BonusStatus.Awarding: return "开奖中";
            case BonusStatus.Error: return "错误";
            case BonusStatus.Lose: return "未中奖";
            case BonusStatus.Waitting: return "未开奖";
            case BonusStatus.Win: return "已中奖";
            default: return string.Empty;
        }
    }

    /// <summary>
    /// 是否已开奖
    /// </summary>
    /// <param name="bonusStatus">开奖状态</param>
    /// <returns>是否已开奖</returns>
    public static bool IsAward(BonusStatus bonusStatus)
    {
        return bonusStatus == BonusStatus.Win || bonusStatus == BonusStatus.Lose || bonusStatus == BonusStatus.Error;
    }

    /// <summary>
    /// 转换方案保密性名称
    /// </summary>
    /// <param name="securityStatus">方案保密类型</param>
    /// <returns>方案保密性名称</returns>
    public static string SecurityName(TogetherSchemeSecurity securityStatus)
    {
        string status = string.Empty;
        switch (securityStatus)
        {
            case TogetherSchemeSecurity.Public:
                status = "公开";
                break;
            case TogetherSchemeSecurity.JoinPublic:
                status = "参与公开";
                break;
            case TogetherSchemeSecurity.CompletePublic:
                status = "截止公开";
                break;
            case TogetherSchemeSecurity.KeepSecrecy:
                status = "永久保密";
                break;
            default: status = "公开"; break;
        }
        return status;
    }

    /// <summary>
    /// 转换合买方案状态名称
    /// </summary>
    /// <param name="togetherStatus">合买方案状态</param>
    /// <param name="togetherStatus">合买方案出票状态</param>
    /// <returns>合买方案状态名称</returns>
    public static string TogetherProgressName(TogetherSchemeProgress togetherStatus, TicketStatus ticketStatus = TicketStatus.Ticketed, BonusStatus bonusStatus = BonusStatus.Waitting, bool isPrizeMoney = false)
    {
        //合买方案进度
        string status = string.Empty;
        switch (togetherStatus)
        {
            case TogetherSchemeProgress.SalesIn:
                status = "销售中";
                break;
            case TogetherSchemeProgress.Standard:
                status = "达标";
                switch (ticketStatus)
                {
                    case TicketStatus.Waitting:
                        status = "待出票";
                        break;
                    case TicketStatus.Ticketing:
                        status = "出票中";
                        break;
case TicketStatus.PrintTicket:
                        status = "已打票";
                        break;                    case TicketStatus.Abort:
                        status = "被终止";
                        break;
                    case TicketStatus.Skipped:
                        status = "被跳过";
                        break;
                    case TicketStatus.Ticketed:
                        status = "已出票";
                        break;
                    case TicketStatus.Error:
                        status = "出票失败";
                        break;
                }
                break;
            case TogetherSchemeProgress.Finish:
                status = "满员";
                switch (ticketStatus)
                {
                    case TicketStatus.Waitting:
                        status = "待出票";
                        break;
                    case TicketStatus.Ticketing:
                        status = "出票中";
                        break;
case TicketStatus.PrintTicket:
                        status = "已打票";
                        break;                    case TicketStatus.Abort:
                        status = "被终止";
                        break;
                    case TicketStatus.Skipped:
                        status = "被跳过";
                        break;
                    case TicketStatus.Ticketed:
                        status = "已出票";
                        break;
                    case TicketStatus.Error:
                        status = "出票失败";
                        break;
                }
                break;
            case TogetherSchemeProgress.Completed:
                status = "已完成";
                switch (ticketStatus)
                {
                    case TicketStatus.Waitting:
                        status = "待出票";
                        break;
                    case TicketStatus.Ticketing:
                        status = "出票中";
                        break;
 case TicketStatus.PrintTicket:
                        status = "已打票";
                        break;                    case TicketStatus.Abort:
                        status = "被终止";
                        break;
                    case TicketStatus.Skipped:
                        status = "被跳过";
                        break;
                    case TicketStatus.Ticketed:
                        switch (bonusStatus)
                        {
                            case BonusStatus.Waitting: status = "未开奖"; break;
                            case BonusStatus.Awarding: status = "开奖中"; break;
                            case BonusStatus.Error: status = "开奖错误"; break;
                            case BonusStatus.Lose: status = "未中奖"; break;
                            case BonusStatus.Win: status = isPrizeMoney ? "已派奖" : "已中奖"; ; break;
                            default: status = "已完成"; break;
                        }
                        break;
                    case TicketStatus.Error:
                        status = "出票失败";
                        break;
                }
                break;
            case TogetherSchemeProgress.AutoStop:
                status = "撤单";
                break;
            case TogetherSchemeProgress.Cancel:
                status = "撤单";
                break;
            default: status = togetherStatus.ToString(); break;
        }
        return status;
    }

    /// <summary>
    /// 是否还可以参与合买
    /// </summary>
    /// <param name="togInfo">合买订单信息</param>
    /// <returns>是否可以参与</returns>
    public static bool IsCanBuy(Sports_TogetherSchemeQueryInfo togInfo)
    {
        return (togInfo.ProgressStatus == TogetherSchemeProgress.SalesIn || togInfo.ProgressStatus == TogetherSchemeProgress.Standard)
            && (togInfo.TotalMoney > togInfo.SoldCount)
            && togInfo.StopTime > DateTime.Now;
    }

    /// <summary>
    /// 转换出票状态名称
    /// </summary>
    /// <param name="togetherTicketStatus">出票状态</param>
    /// <returns>出票状态名称</returns>
    public static string TicketStatusName(TicketStatus ticketStatus)
    {
        //合买出票状态
        string status = string.Empty;
        switch (ticketStatus)
        {
            case TicketStatus.Waitting:
                status = "待出票";
                break;
            case TicketStatus.Ticketing:
                status = "出票中";
                break;
  case TicketStatus.PrintTicket:
                status = "已打票";
                break;            case TicketStatus.Abort:
                status = "被终止";
                break;
            case TicketStatus.Skipped:
                status = "被跳过";
                break;
            case TicketStatus.Ticketed:
                status = "已出票";
                break;
            case TicketStatus.Error:
                status = "出票失败";
                break;
            default: status = ticketStatus.ToString(); break;
        }
        return status;
    }

    /// <summary>
    /// 转换订单来源名称
    /// </summary>
    /// <param name="source">订单来源</param>
    /// <returns>订单来源名称</returns>
    public static string SchemeSourceName(SchemeSource source)
    {
        switch (source)
        {
            case SchemeSource.Iphone: return "iPhone";
            case SchemeSource.Android: return "Android";
            case SchemeSource.Web: return "Site";
            case SchemeSource.Wap: return "Wap";
            default: return "Site";
        }
    }

    /// <summary>
    /// 方案拆分票时，解析出票SP值
    /// </summary>
    /// <param name="currentSP">出票sp值</param>
    /// <returns>sp值列表</returns>
    public static Dictionary<string, string> SpliteSPList(string currentSP)
    {
        try
        {
            var _tmp = new Dictionary<string, string>();
            var sps = currentSP.Split(',');
            foreach (var item in sps)
            {
                var _ret = item.Split('|');
                _tmp.Add(_ret[0], _ret[1]);
            }
            return _tmp;
        }
        catch
        {
            var tmp = new Dictionary<string, string>();
            tmp.Add("", "");

            return tmp;
        }
    }

    /// <summary>
    /// 获取中奖状态的样式名称
    /// </summary>
    /// <param name="progressStatus"></param>
    /// <param name="bonusStatus"></param>
    /// <returns></returns>
    public static string GetBonusCSS(ProgressStatus progressStatus, BonusStatus bonusStatus)
    {
        try
        {
            var css = "";
            switch (progressStatus)
            {
                case ProgressStatus.Aborted:
                    css = "nobonus";
                    break;
                case ProgressStatus.Running:
                case ProgressStatus.Waitting:
                    css = "pro";
                    break;
                case ProgressStatus.Complate:
                    switch (bonusStatus)
                    {
                        case BonusStatus.Awarding:
                        case BonusStatus.Error:
                        case BonusStatus.Waitting:
                            css = "pro";
                            break;
                        case BonusStatus.Lose:
                            css = "nobonus";
                            break;
                    }
                    break;
            }
            return css;
        }
        catch
        {
            return "";
        }
    }
    #endregion

    #region 进度条计算
    /// <summary>
    /// 计算普通订单当前进度所在， 0:刚发起  1:投注中  2:已出票  3:已开奖  4:已派奖
    /// </summary>
    /// <param name="status">方案状态</param>
    /// <param name="ticketstatus">出票状态</param>
    /// <param name="bonusstatus">中奖状态</param>
    /// <returns>所在位置</returns>
    public static int Progress_StatusIndex(ProgressStatus status, TicketStatus ticketstatus, BonusStatus bonusstatus, bool isPrizeMoney = false)
    {
        var at = 0;

        try
        {
            switch (status)
            {
                case ProgressStatus.Aborted: at = 0; break;
                case ProgressStatus.AutoStop: at = 0; break;
                case ProgressStatus.Waitting: at = 1; break;
                case ProgressStatus.Complate:
                case ProgressStatus.Running:
                    switch (ticketstatus)
                    {
                        case TicketStatus.Skipped:
                        case TicketStatus.Error:
                        case TicketStatus.Abort: at = 0; break;
                        case TicketStatus.Ticketing:
                        case TicketStatus.Waitting: at = 1; break;
                        case TicketStatus.Ticketed:
                            if (status == ProgressStatus.Complate && (bonusstatus == BonusStatus.Win || bonusstatus == BonusStatus.Lose))
                            {
                                at = 3;
                                if (bonusstatus == BonusStatus.Win && isPrizeMoney)
                                { at = 4; }
                            }
                            else { at = 2; }
                            break;
                    }
                    break;
            }
        }
        catch
        {

        }

        return at;
    }

    /// <summary>
    /// 计算合买订单当前进度所在， 0:刚发起  1:投注中  2:已出票  3:已开奖  4:已派奖
    /// </summary>
    /// <param name="togetherStatus">合买方案状态</param>
    /// <param name="ticketstatus">出票状态</param>
    /// <param name="BonusStatus">是否开奖</param>
    /// <returns>所在位置</returns>
    public static int Progress_StatusIndex(TogetherSchemeProgress togetherStatus, TicketStatus ticketStatus, BonusStatus bonusStatus, bool isPrizeMoney = false)
    {
        var at = 0;

        try
        {
            switch (togetherStatus)
            {
                case TogetherSchemeProgress.Finish:
                    at = 2;
                    break;
                case TogetherSchemeProgress.Cancel:
                case TogetherSchemeProgress.AutoStop:
                    at = 0;
                    break;
                case TogetherSchemeProgress.Standard:
                    at = 2;
                    break;
                case TogetherSchemeProgress.SalesIn:
                    at = 1;
                    break;
                case TogetherSchemeProgress.Completed:
                    switch (ticketStatus)
                    {
                        case TicketStatus.Skipped:
                        case TicketStatus.Error:
                        case TicketStatus.Abort:
                            at = 0;
                            break;
                        case TicketStatus.Ticketing:
                        case TicketStatus.Waitting:
                            at = 1;
                            break;
                        case TicketStatus.Ticketed:
                            if ((bonusStatus == BonusStatus.Win || bonusStatus == BonusStatus.Lose))
                            {
                                at = 3;
                                if (bonusStatus == BonusStatus.Win && isPrizeMoney)
                                { at = 4; }
                            }
                            else { at = 2; }
                            break;
                    }
                    break;
            }
        }
        catch
        {

        }

        return at;
    }
    #endregion

    #region 命中场次计算
    /// <summary>
    /// 计算投注号码命中场次
    /// </summary>
    /// <param name="anteList">投注号码列表</param>
    /// <returns>命中场次</returns>
    public static int CalHitCount(Sports_AnteCodeQueryInfoCollection anteList)
    {
        try
        {
            return anteList.Where(a => a.BonusStatus == BonusStatus.Win).Count();
        }
        catch
        {
            return 0;
        }
    }
    #endregion

    #region 今日开奖彩种
    /// <summary>
    /// 获取今日开奖彩种编码
    /// </summary>
    /// <param name="week"></param>
    /// <returns></returns>
    public static string GetTodayGame(DayOfWeek week)
    {
        switch (week)
        {
            case DayOfWeek.Tuesday:
            case DayOfWeek.Thursday:
            case DayOfWeek.Sunday:
                return "ssq";
            case DayOfWeek.Monday:
            case DayOfWeek.Wednesday:
            case DayOfWeek.Saturday:
                return "dlt";
            default:
                return "fc3d";
        }
    }

    /// <summary>
    /// 彩种是否今日开奖
    /// </summary>
    /// <param name="gameCode"></param>
    /// <returns></returns>
    public static bool isTodayGame(string gameCode)
    {
        switch (gameCode.ToLower())
        {
            case "ssq":
                return DateTime.Today.DayOfWeek == DayOfWeek.Tuesday || DateTime.Today.DayOfWeek == DayOfWeek.Thursday || DateTime.Today.DayOfWeek == DayOfWeek.Sunday;
            case "dlt":
                return DateTime.Today.DayOfWeek == DayOfWeek.Monday || DateTime.Today.DayOfWeek == DayOfWeek.Wednesday || DateTime.Today.DayOfWeek == DayOfWeek.Saturday;
            case "fc3d":
            case "pl3":
            case "pl5":
                return true;
            default: return false;
        }
    }
    #endregion

    #region 合买信息
    /// <summary>
    /// 返回合买方案截止时间描述
    /// </summary>
    /// <param name="game"></param>
    /// <param name="fsStopTime"></param>
    /// <param name="dsStopTime"></param>
    /// <returns></returns>
    public static string GetGameStopTimeDesc(string game, DateTime fsStopTime = new DateTime(), DateTime dsStopTime = new DateTime())
    {
        switch (game.ToLower())
        {
            case "jczq":
            case "jclq":
                return "周一至周五当天赛事停售时间：<b class='red'>22:40</b> 周六至周日当天赛事停售时间：次日<b class='red'>0:40</b>";
            case "bjdc":
                return "复式合买截止时间：赛前15分钟，单式合买截止日期：赛前20分钟";
            default:
                return "复式合买截止时间：<b class='red'>" + fsStopTime.ToString("MM-dd HH:mm") + "</b>，单式合买截止日期：<b class='red'>" + dsStopTime.ToString("MM-dd HH:mm") + "</b>";
        }
    }
    #endregion


}
