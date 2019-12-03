using Activity.Client;
using Common.Cryptography;
using Common.Net;
using External.Client;
using External.Core.Login;
using GameBiz.Client;
using HTAdmin.Filters;
using HTAdmin.IdentityManager;
using HTAdmin.Models;
using LotteryData.Client;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace HTAdmin.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {

        #region 各WCF属性

        private static GameBizWcfClient_Core gameClient = new GameBizWcfClient_Core();
        public GameBizWcfClient_Core GameClient
        {
            get { return gameClient; }
        }
        private static GameBizWcfClient_Fund gameFundClient = new GameBizWcfClient_Fund();
        public GameBizWcfClient_Fund FundClient
        {
            get { return gameFundClient; }
        }
        private static GameBizWcfClient_Query gameQueryClient = new GameBizWcfClient_Query();
        public GameBizWcfClient_Query QueryClient
        {
            get { return gameQueryClient; }
        }
        private static GameBizWcfClient_Issuse gameIssuseClient = new GameBizWcfClient_Issuse();
        public GameBizWcfClient_Issuse GameIssuseClient
        {
            get { return gameIssuseClient; }
        }
        private static GameBizWcfClient_Experter gameBizExperterClient = new GameBizWcfClient_Experter();
        public GameBizWcfClient_Experter GameBizExperterClient
        {
            get { return gameBizExperterClient; }
        }

        private static ExternalWcfClient _ExternalClient = new ExternalWcfClient();
        /// <summary>
        /// 插件
        /// </summary>
        public ExternalWcfClient ExternalClient
        {
            get { return _ExternalClient; }
        }
        private static ActivityWcfClient _ActivityClient = new ActivityWcfClient();
        /// <summary>
        /// 活动
        /// </summary>
        public ActivityWcfClient ActivityClient
        {
            get { return _ActivityClient; }
        }
        private static ChartWcfClient _chartClient = new ChartWcfClient();
        /// <summary>
        /// 走势图
        /// </summary>
        public ChartWcfClient ChartWcfClient
        {
            get { return _chartClient; }
        }

        #endregion

        #region 各属性

        private AdminOperLogManager _operLogManager;
        public AdminOperLogManager OperLogManager
        {
            get
            {
                return _operLogManager ?? HttpContext.GetOwinContext().Get<AdminOperLogManager>();
            }
            protected set
            {
                _operLogManager = value;
            }
        }


        /// <summary>
        /// 当前用户
        /// </summary>
        public LoginInfo CurrentUser1
        {
            get
            {
                var userInfo = Session["CurrentUser"] as LoginInfo;

                if (userInfo == null)
                {
                    //测试登录
                    userInfo = ExternalClient.LoginAdmin("admin", "722w", IpManager.IPAddress);
                }
                return userInfo;
            }
            set
            {
                Session["CurrentUser"] = value;
                Session.Timeout = 120;
                if (value == null)
                {
                    Session.Clear();
                }
            }
        }

        public LoginInfoModel GetLoginInfo()
        {
            LoginInfoModel m = new LoginInfoModel();
            var manager = HttpContext.GetOwinContext().GetUserManager<AdminUserManager>();
            var user = manager.FindByName(User.Identity.Name);
            m.UserId = user.Id;
            m.UserName = user.UserName;
            m.Permissions = manager.GetPermissionsByUserId(user.Id);
            return m;
        }
        public LoginInfoModel CurUser
        {
            get
            {
                var userInfo = Session["CurUser"] as LoginInfoModel;
                if (userInfo == null && User!=null &&User.Identity!=null &&User.Identity.IsAuthenticated)
                {
                    userInfo = GetLoginInfo();
                    Session["CurUser"] = userInfo;
                }
                return userInfo;
            }
            set
            {
                Session["CurUser"] = value;
            }
        }
        /// <summary>
        /// 默认页面索引
        /// </summary>
        public int PageIndex
        {
            get
            {
                return 1;
            }
        }
        /// <summary>
        /// 默认页面条数
        /// </summary>
        public int PageSize
        {
            get
            {
                return 20;
            }
        }
        #endregion

        #region 系统配置信息

        //public string AdminAgentToken
        //{
        //    get { return ConfigurationManager.AppSettings["AdminAgentToken"]; }
        //}
        //public string GatewayAdminToken
        //{
        //    get { return ConfigurationManager.AppSettings["GatewayAdminToken"]; }
        //}


        /// <summary>
        /// 当前站点路径
        /// </summary>
        public string SiteRoot
        {
            get
            {
                string UrlAuthority = this.Request.Url.GetLeftPart(UriPartial.Authority);
                if (this.Request.ApplicationPath == null || this.Request.ApplicationPath == "/")
                {
                    //直接安装在Web站点
                    return UrlAuthority;
                }
                else
                {
                    //安装在虚拟子目录下
                    return UrlAuthority + this.Request.ApplicationPath;
                }
            }
        }

        #endregion

        #region 获取接口配置信息
        /// <summary>
        /// 解析接口配置文件，并返回配置信息
        /// </summary>
        /// <param name="gatewayType">接口类型，必须与Config.xml里的子项名称一致</param>
        /// <returns>配置信息</returns>
        protected Dictionary<string, string> GetGatewayConfig(string gatewayType)
        {
            var ret = new Dictionary<string, string>();
            XmlDocument doc = new XmlDocument();
            doc.Load(Server.MapPath("~/Mappings/Account.Config.xml"));
            var mappings = doc.GetElementsByTagName(gatewayType);
            foreach (XmlElement e in mappings)
            {
                ret.Add(e.Attributes["key"].Value, e.Attributes["value"].Value);
            }
            return ret;
        }
        #endregion

        #region 导出excel
        public void ExportExcelFromDataSet(DataSet ds, string fileName, string typeid = "1")
        {
            var resp = Response;
            resp.ContentEncoding = System.Text.Encoding.GetEncoding("GB2312");
            resp.ContentType = "application/ms-excel";

            resp.AddHeader("Content-Disposition", "attachment; filename=" + System.Web.HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8) + ".xls");

            string colHeaders = "", Is_item = "";
            int i = 0;

            //定义表对象与行对象，同时使用DataSet对其值进行初始化
            DataTable dt = ds.Tables[0];
            DataRow[] myRow = dt.Select("");
            //typeid=="1"时导出为Excel格式文件;typeid=="2"时导出为XML文件
            if (typeid == "1")
            {
                //取得数据表各列标题，标题之间以\t分割，最后一个列标题后加回车符
                for (i = 0; i < dt.Columns.Count; i++)
                {
                    colHeaders += dt.Columns[i].Caption.ToString() + "\t";
                }
                colHeaders += "\n";

                resp.Write(colHeaders);
                //逐行处理数据
                foreach (DataRow row in myRow)
                {
                    //在当前行中，逐列取得数据，数据之间以\t分割，结束时加回车符\n
                    for (i = 0; i < dt.Columns.Count; i++)
                    {
                        Is_item += row[i].ToString() + "\t";
                    }
                    Is_item += "\n";
                    resp.Write(Is_item);
                    Is_item = "";
                }
            }
            else
            {
                if (typeid == "2")
                {
                    //从DataSet中直接导出XML数据并且写到HTTP输出流中
                    resp.Write(ds.GetXml());
                }
            }
            //写缓冲区中的数据到HTTP头文件中
            resp.End();
        }
        #endregion

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="mobile"></param>
        /// <param name="msgContent"></param>
        /// <returns></returns>
        protected string SendMessage(string mobile, string msgContent)
        {
            //var agentName = this.GameClient.QueryCoreConfigByKey("SMSAgent.Name").ConfigValue;
            //var attach = this.GameClient.QueryCoreConfigByKey("SMSAgent.Attach").ConfigValue;
            //var password = this.GameClient.QueryCoreConfigByKey("SMSAgent.Password").ConfigValue;
            //var userName = this.GameClient.QueryCoreConfigByKey("SMSAgent.UserId").ConfigValue;
            //var returnted = SMSSenderFactory.GetSMSSenderInstance(new SMSConfigInfo
            //{
            //    AgentName = agentName,
            //    Attach = attach,
            //    Password = password,
            //    UserName = userName
            //}).SendSMS(mobile, msgContent, attach);
            //return returnted;
            return string.Empty;
        }

        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase file, string uploadfile, string imgName = "")
        {
            try
            {
                if (file == null)
                {
                    file = Request.Files["images"];
                }
                if (string.IsNullOrEmpty(uploadfile))
                {
                    uploadfile = "/images/add/admin/";
                }
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

                return Json(new { IsSuccess = true, Data = url });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }

        public string LoadImageFile(HttpPostedFileBase file, string uploadfile, string imgName = "")
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
                return url;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 检查是否有权限
        /// </summary>
        /// <param name="needFunction"></param>
        /// <returns></returns>
        public bool CheckRights(string needFunction)
        {
            try
            {
                if (CurUser != null)
                {
                    if (CurUser.Permissions.Contains(needFunction))
                        return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 发送生成静态数据通知
        /// </summary>
        public void SendBuildStaticDataNotice(string pageType, string key)
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


        #region 配置信息
        /// <summary>
        /// 资源站点URl
        /// </summary>
        public readonly string resourceSiteUrl = ConfigurationManager.AppSettings["ResourceSiteUrl"].ToString();
        /// <summary>
        /// 资源站点图片上传地址
        /// </summary>
        public readonly string resourceSitePostUrl = ConfigurationManager.AppSettings["ResourceSitePostUrl"].ToString();

        /// <summary>
        /// 上传非图片文件请求url,比如http://img.cai.com
        /// </summary>
        public readonly string UploadFileRequestUrl = ConfigurationManager.AppSettings["UploadFileRequestUrl"].ToString();
        /// <summary>
        /// 文件上传请求action，比如/UpLoad/UploadFile
        /// </summary>
        public readonly string UploadFileAction = ConfigurationManager.AppSettings["UploadFileAction"].ToString();
        /// <summary>
        /// 非图片文件保存目录，比如/Upload/files/
        /// </summary>
        public readonly string FileSavePath = ConfigurationManager.AppSettings["FileSavePath"].ToString();
        /// <summary>
        /// 判断是那个项目文件目录
        /// </summary>
        public readonly string WebSiteEName = ConfigurationManager.AppSettings["WebSiteEName"].ToString();
        #endregion
    }
}