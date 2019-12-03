using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Configuration;
using Common.Lottery;
using GameBiz.Core;
using External.Core.SiteMessage;

/// <summary>
/// wcf数据获取类
/// </summary>
public static class WCFService
{
    private static string _guestToken = "";
    //用户口令
    public static string UserToken
    {
        get
        {
            try
            {
                _guestToken = string.IsNullOrEmpty(_guestToken) ? WCFClients.ExternalClient.GetGuestToken().ReturnValue : _guestToken;
            }
            catch
            {
                _guestToken = "";
            }
            return _guestToken;
        }
    }

    private static Dictionary<string, Dictionary<string, string>> _gameTypeList = new Dictionary<string, Dictionary<string, string>>();
    /// <summary>
    /// 彩种玩法信息
    /// </summary>
    public static string GameTypeList(string gameCode, string gameType)
    {
        if (!_gameTypeList.ContainsKey(gameCode))
        {
            Dictionary<string, string> tmpDic = new Dictionary<string, string>();
            var gameTypeCollection = WCFClients.GameIssuseClient.QueryGameTypeList(gameCode, UserToken);
            foreach (var item in gameTypeCollection)
            {
                tmpDic.Add(item.GameType, item.DisplayName);
            }
            _gameTypeList.Add(gameCode, tmpDic);
        }
        if (!string.IsNullOrEmpty(gameType) && _gameTypeList[gameCode].ContainsKey(gameType))
        {
            return _gameTypeList[gameCode][gameType];
        }
        else
        {
            return gameType;
        }
    }

    /// <summary>
    /// 已派奖的奖期信息
    /// </summary>
    /// <param name="gameCode"></param>
    /// <param name="gameType"></param>
    /// <returns></returns>
    public static string[] PrizedIssuseList(string gameCode, string gameType, int length = 5)
    {
        try
        {
            var issuse = WCFClients.GameClient.QueryPrizedIssuseList(gameCode, gameType, length, UserToken);
            return issuse.Split(',');
        }
        catch (Exception ex)
        {
            return new string[] { };
        }
    }

    /// <summary>
    /// 已停止的奖期信息
    /// </summary>
    /// <param name="gameCode"></param>
    /// <param name="gameType"></param>
    /// <returns></returns>
    public static string[] StoppedIssuseList(string gameCode, string gameType, int length = 5)
    {
        try
        {
            var issuse = WCFClients.GameClient.QueryStopIssuseList(gameCode, gameType, length, UserToken);
            return issuse.Split(',');
        }
        catch (Exception ex)
        {
            return new string[] { };
        }
    }

    /// <summary>
    /// 查询最新开奖号码
    /// </summary>
    /// <param name="gameCode"></param>
    /// <param name="issuseNumber"></param>
    /// <param name="gameType"></param>
    /// <returns></returns>
    public static WinNumber_QueryInfo QueryWinNumber(string gameCode, string issuseNumber, string gameType = "")
    {
        try
        {
            var key = gameCode.ToLower() == "ctzq" ? "ctzq_" + gameType : gameCode;
            var award = WCFClients.GameIssuseClient.QueryWinNumber(key, issuseNumber);
            if (award == null)
            {
                return new WinNumber_QueryInfo()
                {
                    GameCode = gameCode,
                    GameType = gameType,
                    IssuseNumber = issuseNumber,
                    WinNumber = "",
                    DisplayName = "",
                    AwardTime = DateTime.MinValue
                };
            }
            else
            {
                return award;
            }
        }
        catch
        {
            return new WinNumber_QueryInfo()
            {
                GameCode = gameCode,
                GameType = gameType,
                IssuseNumber = issuseNumber,
                WinNumber = "",
                DisplayName = "",
                AwardTime = DateTime.MinValue
            };
        }
    }

    #region 网站自带文章系统
    public static Dictionary<string, ArticleInfo_QueryCollection> _articleCol = new Dictionary<string, ArticleInfo_QueryCollection>();
    /// <summary>
    /// 获取文章列表
    /// </summary>
    /// <param name="category">文章类型</param>
    /// <param name="game">彩种</param>
    /// <returns>文章集合</returns>
    public static List<ArticleInfo_Query> GetArticleCollection(string category, string game, int count)
    {
        string key = game + "|" + category;
        if (!_articleCol.ContainsKey(key))
        {
            _articleCol.Add(key, WCFClients.ExternalClient.QueryArticleList("", game, category, 0, 50, UserToken));
        }
        else
        {
            _articleCol[key] = WCFClients.ExternalClient.QueryArticleList("", game, category, 0, 50, UserToken);
        }
        return _articleCol[key].ArticleList.Take(count).ToList();
    }

    public static BulletinInfo_Collection _noticeList = null;
    /// <summary>
    /// 获取公告列表
    /// </summary>
    /// <param name="count">获取公告的条数</param>
    /// <returns>公告列表</returns>
    public static List<BulletinInfo_Query> GetNoticeList(int count)
    {
        _noticeList = WCFClients.ExternalClient.QueryDisplayBulletinCollection(BulletinAgent.Local, 0, 15, UserToken);
        if (_noticeList == null)
        {
            _noticeList = WCFClients.ExternalClient.QueryDisplayBulletinCollection(BulletinAgent.Local, 0, 15, UserToken);
        }
        return _noticeList.BulletinList.OrderByDescending(a => a.IsPutTop).OrderByDescending(a => a.CreateTime).Take(count).ToList();
    }
    #endregion
}