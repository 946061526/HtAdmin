using Activity.Client;
using Common.Cryptography;
using Common.Net;
using External.Client;
using GameBiz.Client;
using log4net;
using LotteryData.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace HTAdmin
{
    public static class GlobalCache
    {
        #region ===WCF服务单例实例化===
        public static string MD5Key = "Q56GtyNkop97H334TtyturfgErvvv98a";
        public static string ResourceSiteUrl = ConfigurationManager.AppSettings["ResourceSiteUrl"];
        public static string ShareResourceUrl = ConfigurationManager.AppSettings["ShareRes"].ToString();

        private static ExternalWcfClient _externalClient;
        /// <summary>
        /// 插件
        /// </summary>
        public static ExternalWcfClient ExternalClient
        {
            get
            {
                if (_externalClient == null)
                {
                    _externalClient = new ExternalWcfClient();
                }
                return _externalClient;
            }
        }
        private static GameBizWcfClient_Core _gameClient;
        public static GameBizWcfClient_Core GameClient
        {
            get
            {
                if (_gameClient == null)
                {
                    _gameClient = new GameBizWcfClient_Core();
                }
                return _gameClient;
            }
        }
        private static GameBizWcfClient_Fund _fundClient;
        public static GameBizWcfClient_Fund FundClient
        {
            get
            {
                if (_fundClient == null)
                {
                    _fundClient = new GameBizWcfClient_Fund();
                }
                return _fundClient;
            }
        }
        private static GameBizWcfClient_Query _queryClient;
        public static GameBizWcfClient_Query QueryClient
        {
            get
            {
                if (_queryClient == null)
                {
                    _queryClient = new GameBizWcfClient_Query();
                }
                return _queryClient;
            }
        }
        private static GameBizWcfClient_Issuse _gameIssuseClient;
        public static GameBizWcfClient_Issuse GameIssuseClient
        {
            get
            {
                if (_gameIssuseClient == null)
                {
                    _gameIssuseClient = new GameBizWcfClient_Issuse();
                }
                return _gameIssuseClient;
            }
        }

        private static GameBizWcfClient_Experter _gameBizExperterClient;
        public static GameBizWcfClient_Experter GameBizExperterClient
        {
            get
            {
                if (_gameBizExperterClient == null)
                {
                    _gameBizExperterClient = new GameBizWcfClient_Experter();
                }
                return _gameBizExperterClient;
            }
        }
        private static ActivityWcfClient _activityClient;
        /// <summary>
        /// 活动
        /// </summary>
        public static ActivityWcfClient ActivityClient
        {
            get
            {
                if (_activityClient == null)
                {
                    _activityClient = new ActivityWcfClient();
                }
                return _activityClient;
            }
        }

        public static ChartWcfClient _chartWcfClient;
        /// <summary>
        /// 走势图
        /// </summary>
        public static ChartWcfClient ChartWcfClient
        {
            get
            {
                if (_chartWcfClient == null)
                {
                    _chartWcfClient = new ChartWcfClient();
                }
                return _chartWcfClient;
            }
        }


        #endregion

        public static string SiteName
        {
            get { return ConfigurationManager.AppSettings["SiteName"]; }
        }
        public static string WebSiteName
        {
            get { return ConfigurationManager.AppSettings["WebSiteName"]; }
        }
        public static string WithdrawRemarkConfigFile
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "/Configurations/" + SiteName + "/xmls/WithdrawRemark.Config.xml"); } //Server.MapPath("~/Configurations/" + SiteName + "/xmls/WithdrawRemark.Config.xml"); }
        }
        public static bool IsTest
        {

            get
            {
                if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["IsTest"]))
                { return false; }

                return bool.Parse(ConfigurationManager.AppSettings["IsTest"]);
            }
        }

        private static List<GameBiz.Core.LotteryGameInfo> lotterys;
        public static string GetLotteryName(string strGameCode)
        {
            try
            {
                if (lotterys == null)
                {
                    lotterys = GameIssuseClient.QueryAllGameList();
                }
                var lot = lotterys.FirstOrDefault(p => p.GameCode == strGameCode);
                if (lot != null)
                {
                    return lot.DisplayName;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch
            {
                return string.Empty;
            }
        }
        public static string GetLotteryType(string strGameCode)
        {
            switch (strGameCode)
            {
                case "DLT":
                case "FC3D":
                case "SSQ":
                case "PL5":
                case "PL3": return "数字彩";

                case "CQSSC": //return "重庆时时彩";
                case "GD11X5": //return "广东11选5";
                case "GDKLSF": //return "广东快乐十分";
                case "GXKLSF": //return "广西快乐十分";
                case "JSKS": //return "江苏快3";
                case "JX11X5": //return "江西11选5";
                case "JXSSC": //return "江西时时彩";
                case "QLC": //return "七乐彩";
                case "QXC": //return "七星彩";
                case "SD11X5": //return "山东11选5";
                case "SDQYH": //return "山东群英会";
                case "CQ11X5": //return "重庆11选5";
                case "CQKLSF": //return "重庆快乐十分";
                case "DF6J1": //return "东方6+1";
                case "HBK3": //return "湖北快3";
                case "HC1": //return "好彩一";
                case "HD15X5":// return "华东15选5";
                case "HNKLSF":// return "湖南快乐十分";
                case "JLK3": //return "新快3";
                case "JSK3": //return "江苏快3";
                case "BJPK10":
                case "LN11X5": return "高频彩";//return "辽宁11选5";

                case "BJDC":
                case "JCZQ":
                case "JCLQ":
                case "CTZQ": return "竞技彩";
                default: return string.Empty;
            }
        }

        /// <summary>
        /// 获取购彩方式名称
        /// </summary>
        /// <param name="buyType"1:自购; 2:追号; 3:合买</param>
        /// <returns></returns>
        public static string GetBuyTypeName(int buyType)
        {
            switch (buyType)
            {
                case 1:
                    return "自购";
                case 2:
                    return "追号";
                case 3:
                default:
                    return "自购";
            }
        }
        public static string GetGameCodeName(string strGameCode)
        {
            switch (strGameCode)
            {
                case "CQSSC": return "重庆时时彩";
                case "DLT": return "大乐透";
                case "FC3D": return "福彩3D";
                case "GD11X5": return "广东11选5";
                case "GDKLSF": return "广东快乐十分";
                case "GXKLSF": return "广西快乐十分";
                case "JSKS": return "江苏快3";
                case "JX11X5": return "江西11选5";
                case "JXSSC": return "江西时时彩";
                case "PL3": return "排列三";
                case "QLC": return "七乐彩";
                case "QXC": return "七星彩";
                case "SD11X5": return "山东11选5";
                case "SDQYH": return "山东群英会";
                case "SSQ": return "双色球";
                case "CQ11X5": return "重庆11选5";
                case "CQKLSF": return "重庆快乐十分";
                case "DF6J1": return "东方6+1";
                case "HBK3": return "湖北快3";
                case "HC1": return "好彩一";
                case "HD15X5": return "华东15选5";
                case "HNKLSF": return "湖南快乐十分";
                case "JLK3": return "新快3";
                case "JSK3": return "江苏快3";
                case "LN11X5": return "辽宁11选5";
                case "PL5": return "排列五";
                case "BJDC": return "北京单场";
                case "JCZQ": return "竞彩足球";
                case "JCLQ": return "竞彩篮球";
                case "CTZQ": return "传统足球";
                default: return string.Empty;
            }
        }

        public static string GetArticleCategory(string categoryCode)
        {
            switch (categoryCode)
            {
                case "HOT":
                case "Lottery_Hot":
                    return "热点资讯";
                case "CPZJ":
                    return "彩票中奖";
                case "FocusCMS":
                    return " 焦点新闻";
                case "ZDZT":
                    return " 置顶主题";
                case "ZQDP":
                    return " 足球点评";
                case "LQDP":
                    return "篮球点评";
                case "INFO":
                    return "赛事资讯";
                case "SZZX":
                    return "数字资讯";
                case "Lottery_GameCode":
                    return "彩种资讯";
                default:
                    return "";
            }
        }
        public static string GetLotterySelectOption(string gameCode, string gameType)
        {
            StringBuilder allOption = new StringBuilder(200);
            allOption.Append("<option value='ALL'>全部</option>");
            List<string> gameTypeArray = new List<string>();
            if (gameCode == "CTZQ")
            {
                gameTypeArray = new List<string>() { "T14C_14场", "T4CJQ_4场进球", "T6BQC_6场半全", "TR9_任9" };
            }
            else if (gameCode == "BJDC")
            {
                gameTypeArray = new List<string>() { "BF_比分", "BQC_半全场", "SPF_胜平负", "SXDS_上下单双", "ZJQ_进球数" };
            }
            else if (gameCode == "JCZQ")
            {
                gameTypeArray = new List<string>() { "SPF_胜平负", "BRQSPF_不让球胜平负", "ZJQ_进球数", "BF_比分", "BQC_半全场", "HH_混合过关" };
            }
            else if (gameCode == "JCLQ")
            {
                gameTypeArray = new List<string>() { "SF_胜负", "RFSF_让分胜负", "DXF_大小分", "SFC_胜分差", "HH_混合过关" };
            }
            else if (gameCode == "CQSSC")
            {
                gameTypeArray = new List<string>() { "1XDX_一星单选", "2XDX_二星单选", "2XHZ_二星和值", "3XDX_三星单选", "5XDX_五星单选", "2XZXFS_二星组选复式", "2XBAODAN_二星组选包胆", "ZX3DS_组三单式", "ZX3FS_组三复式", "ZX6_组选六", "5XTX_五星通选", "DXDS_大小单双" };
            }
            else if (gameCode == "JX11X5")
            {
                gameTypeArray = new List<string>() { "RX1_前一直选", "RX2_任选二", "RX3_任选三", "RX4_任选四", "RX5_任选五", "RX6_任选六", "RX7_任选七", "Q2ZHIX_前二直选", "Q2ZUX_前二组选", "Q3ZHIX_前三直选", "Q3ZUX_前三组选" };
            }
            else if (gameCode == "SSQ")
            {
                gameTypeArray = new List<string>() { "DS_标准单式", "FS_标准复式", "DT_拖胆投注" };
            }
            else if (gameCode == "DLT")
            {
                gameTypeArray = new List<string>() { "DS_标准单式", "FS_标准复式", "DT_拖胆投注" };
            }
            else if (gameCode == "FC3D")
            {
                gameTypeArray = new List<string>() { "FS_直选", "HZ_直选和值", "ZX3DS_组三单式", "ZX3FS_组三复式", "ZX6_组选六" };
            }
            else if (gameCode == "PL3")
            {
                gameTypeArray = new List<string>() { "FS_直选", "HZ_直选和值", "ZX3DS_组三单式", "ZX3FS_组三复式", "ZX6_组选六" };
            }

            foreach (var item in gameTypeArray)
            {
                var array = item.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                string selected = string.Empty;
                if (array.Length == 2)
                {
                    if (array[0] == gameType)
                    {
                        selected = "selected='selected'";
                    }
                    allOption.AppendFormat("<option value='{0}' {2}>{1}</option>", array[0], array[1], selected);
                }
            }
            return allOption.ToString();
        }
        public static string LoadImageFile(HttpPostedFileBase file, string uploadfile, string imgName = "")
        {
            try
            {
                string nameImg = string.Empty;
                if (string.IsNullOrEmpty(imgName))
                    nameImg = DateTime.Now.ToString("yyyyMMddHHmmssff");
                else nameImg = imgName;

                string resourceSiteUrl = ConfigurationManager.AppSettings["ResourceSiteUrl"].ToString();
                string resourceSitePostUrl = ConfigurationManager.AppSettings["ResourceSitePostUrl"].ToString();

                string upLoadFile = uploadfile;  // "/images/add/";
                string upLoadPostPath = ConfigurationManager.AppSettings["UpLoadPostPath"].ToString();

                nameImg += file.FileName.Substring(file.FileName.LastIndexOf(".")).ToLower();
                string url = string.Format("{0}{1}{2}", resourceSiteUrl, upLoadFile, nameImg);

                upLoadFile = "/" + ConfigurationManager.AppSettings["WebSiteEName"].ToString() + upLoadFile;

                string postUrl = string.Format("{0}{1}?filename={2}&upLoadFile={3}", resourceSitePostUrl, upLoadPostPath, nameImg, upLoadFile);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(postUrl);
                request.Method = "POST";
                request.AllowAutoRedirect = false;
                request.ContentType = "multipart/form-data";
                byte[] bytes = new byte[file.InputStream.Length];
                file.InputStream.Read(bytes, 0, (int)file.InputStream.Length);
                request.ContentLength = bytes.Length;
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                }
                HttpWebResponse respon = (HttpWebResponse)request.GetResponse();
                //Hashtable hash = new Hashtable();
                //hash["error"] = 0;
                //hash["url"] = url;
                //return Content(System.Web.Helpers.Json.Encode(hash), "text/html; charset=UTF-8");
                return url;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }


        /// <summary>
        /// 发送生成静态数据通知
        /// </summary>
        public static void SendBuildStaticDataNotice(string pageType, string key)
        {
            var urlArray = ConfigurationManager.AppSettings["BuildStaticFileSendUrl"].Split('|');
            foreach (var url in urlArray)
            {
                if (string.IsNullOrEmpty(url))
                    continue;
                var code = Encipherment.MD5(string.Format("Home_BuildSpecificPage_{0}", pageType), Encoding.UTF8);
                var webSiteUrl = string.Format("{0}/{1}?pageType={2}&code={3}&key={4}", url, "StaticHtml/BuildSpecificPage", pageType, code, key);
                var result = PostManager.Get(webSiteUrl, Encoding.UTF8, timeoutSeconds: 60);
            }
        }

        /// <summary>
        /// 彩富上传图片
        /// </summary>
        /// <param name="file"></param>
        /// <param name="uploadfile"></param>
        /// <returns></returns>
        public static string LoadImageFile_Tgbank(HttpPostedFileBase file, string uploadfile)
        {
            try
            {
                string nameImg = DateTime.Now.ToString("yyyyMMddHHmmssff");

                string resourceSiteUrl = ConfigurationManager.AppSettings["ResourceSiteUrl_Tgbank"].ToString();
                string resourceSitePostUrl = ConfigurationManager.AppSettings["ResourceSitePostUrl_Tgbank"].ToString();

                string upLoadFile = uploadfile;  // "/images/add/";
                string upLoadPostPath = ConfigurationManager.AppSettings["UpLoadPostPath"].ToString();

                nameImg += file.FileName.Substring(file.FileName.LastIndexOf(".")).ToLower();
                string url = string.Format("{0}{1}{2}", resourceSiteUrl, upLoadFile, nameImg);

                upLoadFile = "/" + ConfigurationManager.AppSettings["WebSiteEName_Tgbank"].ToString() + upLoadFile;

                string postUrl = string.Format("{0}{1}?filename={2}&upLoadFile={3}", resourceSitePostUrl, upLoadPostPath, nameImg, upLoadFile);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(postUrl);
                request.Method = "POST";
                request.AllowAutoRedirect = false;
                request.ContentType = "multipart/form-data";
                byte[] bytes = new byte[file.InputStream.Length];
                file.InputStream.Read(bytes, 0, (int)file.InputStream.Length);
                request.ContentLength = bytes.Length;
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                }
                HttpWebResponse respon = (HttpWebResponse)request.GetResponse();
                //Hashtable hash = new Hashtable();
                //hash["error"] = 0;
                //hash["url"] = url;
                //return Content(System.Web.Helpers.Json.Encode(hash), "text/html; charset=UTF-8");
                return url;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
    }
}