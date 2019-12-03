using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GameBiz.Client;
using Activity.Client;
using External.Client;

/// <summary>
/// WCF客户端类
/// </summary>
public class WCFClients
{
    private static ExternalWcfClient _ExternalClient = null;
    /// <summary>
    /// 插件WCF客户端
    /// </summary>
    public static ExternalWcfClient ExternalClient
    {
        get
        {
            if (_ExternalClient == null)
            {
                _ExternalClient = new ExternalWcfClient();
            }
            return _ExternalClient;
        }
    }
    private static ActivityWcfClient _ActivityClient = null;
    /// <summary>
    /// 活动WCF客户端
    /// </summary>
    public static ActivityWcfClient ActivityClient
    {
        get
        {
            if (_ActivityClient == null)
            {
                _ActivityClient = new ActivityWcfClient();
            }
            return _ActivityClient;
        }
    }

    private static GameBizWcfClient_Core _gameBiz = null;
    /// <summary>
    /// 彩种WCF客户端
    /// </summary>
    public static GameBizWcfClient_Core GameClient
    {
        get
        {
            if (_gameBiz == null)
            {
                _gameBiz = new GameBizWcfClient_Core();
            }
            return _gameBiz;
        }
    }

    private static GameBizWcfClient_Fund _gameFund = null;
    /// <summary>
    /// 彩种资金WCF客户端
    /// </summary>
    public static GameBizWcfClient_Fund GameFundBusiness
    {
        get
        {
            if (_gameFund == null)
            {
                _gameFund = new GameBizWcfClient_Fund();
            }
            return _gameFund;
        }
    }
    private static GameBizWcfClient_Issuse _gameIssuse = null;
    /// <summary>
    /// 彩种奖期WCF客户端
    /// </summary>
    public static GameBizWcfClient_Issuse GameIssuseClient
    {
        get
        {
            if (_gameIssuse == null)
            {
                _gameIssuse = new GameBizWcfClient_Issuse();
            }
            return _gameIssuse;
        }
    }
}

