using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using System.Net;
using System.Net.Http;
using Common.Utilities;
using Common;
using Common.Communication;
using External.Core;
using External.Core.SiteMessage;
using External.Core.SystemConfig;
using GameBiz.Core;
using Common.Net;
using System.Text;
using Common.Cryptography;
using System.Configuration;
using External.Core.Login;
using External.Core.Help;
using HTAdmin.Filters;
using HTAdmin.Models;
using log4net;
using HTAdmin.Models;
using HTAdmin.IdentityManager;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;

namespace HTAdmin.Controllers
{
    public class ContentManageController : BaseController
    {
        private ILog logger = LogManager.GetLogger(typeof(SiteSettingsController));
        private AdminUserManager _userManager;
        public AdminUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<AdminUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        #region ==公告管理==
        /// <summary>
        /// 公告管理
        /// </summary>
        [CheckPermission("N101")]
        public ActionResult NoticeManage(FormCollection fc)
        {
            try
            {
                #region 权限控制
                //新增
                ViewBag.AddNotice = CheckRights("FBGG100") ? true : false;
                //修改
                ViewBag.UpdateNotice = CheckRights("XGGG110") ? true : false;
                //启用
                ViewBag.Enabled = CheckRights("JYGG120") ? true : false;
                //删除
                ViewBag.Delete = CheckRights("SCGG130") ? true : false;
                ViewBag.Admins = GlobalCache.ExternalClient.GetSystemUsers(new SystemUserSearchAddition()
                {
                    PageIndex = 1,
                    PageSize = 100000
                });
                #endregion
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [CheckPermission("N101")]
        public JsonResult GetNoticeInfos(NoticeConditionModel model)
        {
            LayuiGridResult result = new LayuiGridResult() { code = 0 };
            try
            {
                if (model != null)
                {
                    // priority = -1 表示获取所有  IsPutTop =-1 也表示获取所有
                    int priority = -1, istop = -1;
                    BulletinCondition con = new BulletinCondition()
                    {
                        OperatorUserId = model.CreatorUserId,
                        StartCreationDate = model.StartCreationDate,
                        EndCreationDate = model.EndCreationDate,
                        Status = model.Status,
                        PageSize = model.PageSize,
                        PageIndex = model.PageIndex,
                        Title = model.Title
                    };

                    BulletinInfo_Collection data = GlobalCache.ExternalClient.GetBulletinInfos(con);
                    result.count = data.TotalCount;
                    if (data.TotalCount > 0)
                    {
                        result.data = data.BulletinList.Select(p =>
                        {
                            return new
                            {
                                Id = p.Id,
                                Title = p.Title,
                                CreateTime = p.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                Creator = p.UpdatorDisplayName,
                                IsTop = p.IsPutTop == 1,
                                Status = (int)p.Status
                            };
                        }).ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                result.code = 1;
                result.msg = ex.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// 禁用
        /// </summary>
        public JsonResult DisableBulletin(long id)
        {
            if (id == 0)
            {
                return Json(new { status = 0, message = "公告标志不可以为空" });
            }
            try
            {
                var result = GlobalCache.ExternalClient.DisnableBulleinAdmin(id, CurUser.UserId,CurUser.UserName);
                return Json(new { status = result.IsSuccess, message = "成功禁用" });
            }
            catch (Exception ex)
            {
                return Json(new { status = 0, message = ex.Message });
            }
        }
        /// <summary>
        /// 启用
        /// </summary>
        public JsonResult EnableBulletin(long id, int status)
        {
            if (id == 0)
            {
                return Json(new { status = 0, message = "公告标志不可以为空" });
            }
            try
            {
                if (status == (int)EnableStatus.Enable)
                {
                    var result = GlobalCache.ExternalClient.EnableBulleinAdmin(id, CurUser.UserId, CurUser.UserName);
                    return Json(new { status = result.IsSuccess, message = "成功启用" });
                }
                else
                {
                    var result = GlobalCache.ExternalClient.DisnableBulleinAdmin(id, CurUser.UserId, CurUser.UserName);
                    return Json(new { status = result.IsSuccess, message = "成功禁用" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { status = 0, message = ex.Message });
            }
        }
        /// <summary>
        /// 删除公告
        /// </summary>
        public JsonResult DeleteBulletin(long id)
        {
            if (id == 0)
            {
                return Json(new { status = 0, message = "公告标志不可以为空" });
            }
            try
            {
                var result = GlobalCache.ExternalClient.DeleteBulletin(id);
                return Json(new { status = result.IsSuccess, message = "成功删除" });
            }
            catch (Exception ex)
            {
                return Json(new { status = 0, message = ex.Message });
            }
        }

        /// <summary>
        /// 发布公告
        /// </summary>
        [CheckPermission("FBGG100")]
        public ActionResult BulletinInfo()
        {
            if (!string.IsNullOrWhiteSpace(Request["bulletinId"]))
            {
                long bulletinId = Convert.ToInt64(Request["bulletinId"]);
                ViewBag.Bulletin = GlobalCache.ExternalClient.QueryManagementBulletinDetailByIdAdmin(bulletinId);
            }
            return View();
        }
        /// <summary>
        /// 发布公告
        /// </summary>
        [ValidateInput(false)]
        public JsonResult PublishNotice(BulletinModel model)
        {
            var bulletin = new BulletinInfo_Publish();
            try
            {
                if (string.IsNullOrWhiteSpace(model.Title))
                {
                    return Json(new { status = 0, message = "公告标题不能为空" });
                }
                if (string.IsNullOrWhiteSpace(model.Content))
                {
                    return Json(new { status = 0, message = "公告内容不能为空" });
                }
                if (model.EffectiveFrom == null || model.EffectiveFrom.HasValue == false)
                {
                    return Json(new { status = 0, message = "公告发布日期不可以为空" });
                }
                bulletin.Title = model.Title;
                bulletin.Content = model.Content;
                bulletin.EffectiveFrom = model.EffectiveFrom;
                bulletin.EffectiveTo = model.EffectiveTo;
                bulletin.CoverImage = model.CoverImage;
                bulletin.Keyword = model.Keyword;
                bulletin.Summary = model.Summary?.Trim();

                bulletin.Priority = model.Priority;
                bulletin.IsPutTop = model.IsPutTop;
                bulletin.Status = model.Status;
                bulletin.BulletinAgent = model.BulletinAgent;

                try
                {
                    //同步生成静态页
                    if (Request["chkBuildStatic"] != null)
                    {
                        bool ischeck = false;
                        bool.TryParse(Request["chkBuildStatic"], out ischeck);
                        if (Request["chkBuildStatic"] == "checked" || ischeck)
                        {
                            var arrPageType = new string[] { "10", "70" };
                            foreach (var item in arrPageType)
                            {
                                SendBuildStaticDataNotice(item, string.Empty);
                            }
                        }
                    }
                }
                catch
                {
                }
                if (model.Id <= 0)// 表示创建
                {
                    var noticeResult = base.ExternalClient.PublishBulletin(bulletin, CurUser.UserId);
                    return Json(new { status = 1, message = "创建成功" });
                }
                else
                {
                    long bulletinId = model.Id;
                    var noticeResult = base.ExternalClient.UpdateBulletin(bulletinId, bulletin, CurUser.UserId);
                    return Json(new { status = 1, message = "修改成功" });
                }

            }
            catch (Exception ex)
            {
                return Json(new { status = 0, message = ex.Message });
            }
        }
        #endregion

        #region 帮助中心管理
        /// <summary>
        /// 帮助中心首页
        /// </summary>
        /// <returns></returns>
        public ActionResult HelpManage(FormCollection fc)
        {
            if (Request["val"] != null && Request["val"].Contains("reset"))
                fc.Clear();
            try
            {
                if (!CheckRights("NRGL100"))
                    throw new Exception("对不起，您的权限不足！");
                ViewBag.FbBz = CheckRights("FBBZ107");
                ViewBag.JyQy = CheckRights("JYQY107");
                ViewBag.Delete = CheckRights("NSC100");
                ViewBag.Update = CheckRights("NXG100");
                ViewBag.HtmlTitle = string.IsNullOrWhiteSpace(Request["key"]) ? string.Empty : Request["key"];
                ViewBag.UserId = string.IsNullOrWhiteSpace(Request["userId"]) ? string.Empty : Request["userId"];
                ViewBag.MenuId = string.IsNullOrWhiteSpace(Request["menu"]) ? "" : Request["menu"];
                ViewBag.UserListInfo = UserManager.GetAllAdminUsers()?.Select(p =>
                   {
                       return new AdminUserModel() { UserId = p.Id, Name = p.Name, UserName = p.UserName };

                   }).ToList();// GlobalCache.GameClient.GetOpratorCollection(0, 2000, CurrentUser.UserToken);
                if (string.IsNullOrWhiteSpace(Request["startDate"]))
                {
                    ViewBag.StartTime = null;
                }
                else
                {
                    ViewBag.StartTime = Convert.ToDateTime(Request["startDate"]);
                }
                if (string.IsNullOrWhiteSpace(Request["endDate"]))
                {
                    ViewBag.EndTime = null;
                }
                else
                {
                    ViewBag.EndTime = Convert.ToDateTime(Request["endDate"]);
                }
                if (string.IsNullOrWhiteSpace(Request["isPutTop"]))
                {
                    ViewBag.IsPutTop = null;
                }
                else
                {
                    ViewBag.IsPutTop = Convert.ToInt32(Request["isPutTop"]);
                }
                if (string.IsNullOrWhiteSpace(Request["status"]))
                {
                    ViewBag.Status = null;
                }
                else
                {
                    ViewBag.Status = Convert.ToInt32(Request["status"]);
                }
                ViewBag.PageIndex = Request["pageIndex"] == null ? Request["pageIndex"] == null ? base.PageIndex : Convert.ToInt32(Request["pageIndex"]) : Convert.ToInt32(Request["pageIndex"]);
                ViewBag.PageSize = Request["pageSize"] == null ? Request["pageSize"] == null ? base.PageSize : Convert.ToInt32(Request["pageSize"]) : Convert.ToInt32(Request["pageSize"]);
                ViewBag.Description = string.IsNullOrWhiteSpace(Request["Description"]) ? string.Empty : Request["Description"];
                ViewBag.HelpM = base.ExternalClient.BackQueryHelpManagementAdmin(ViewBag.MenuId, ViewBag.HtmlTitle, ViewBag.UserId, ViewBag.StartTime, ViewBag.EndTime, ViewBag.IsPutTop, ViewBag.Status, ViewBag.PageIndex - 1, ViewBag.PageSize, CurUser.UserId);
                ViewBag.HelpType = base.ExternalClient.QueryHelpMenuManagement().HelpList;
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 帮助中心新增修改页面
        /// </summary>
        public ActionResult HelpManageInfo()
        {
            if (!string.IsNullOrWhiteSpace(Request["Did"]) && Request["Type"].ToLower() != "add")
            {
                string Did = Convert.ToString(Request["Did"]);
                ViewBag.Detail = base.ExternalClient.BackQueryHelpManagementById(Did);
            }
            ViewBag.HelpType = base.ExternalClient.QueryHelpMenuManagement().HelpList;
            return View();
        }

        /// <summary>
        /// 帮助中心删除内容
        /// </summary>
        /// <returns></returns>
        [ValidateInput(false)]
        public JsonResult Delete_HelpManage()
        {
            if (!string.IsNullOrWhiteSpace(Request["Did"]))
            {
                string Did = Convert.ToString(Request["Did"]);
                var result = base.ExternalClient.DeleteHelpManagement(Did);
                return Json(new { IsSuccess = true, data = result, Msg = result.Message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { IsSuccess = false, data = string.Empty, Msg = "删除失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 帮助中心删除内容
        /// </summary>
        /// <returns></returns>
        [ValidateInput(false)]
        public JsonResult Update_HelpManage(FormCollection fc)
        {
            if (!string.IsNullOrWhiteSpace(Request["id"]))
            {
                string isEnabled = fc["isEnabled"];
                HelpMenuDetailListInfo help = new HelpMenuDetailListInfo();
                if (!string.IsNullOrWhiteSpace(isEnabled))
                {
                    help.IsEnabled = isEnabled == "0" ? true : false;
                }

                var result = base.ExternalClient.UpdateHelpManagement(help);
                return Json(new { IsSuccess = true, data = result, Msg = result.Message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { IsSuccess = false, data = string.Empty, Msg = "删除失败" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 提交 - 帮助中心修改内容
        /// </summary>
        /// <param name="fc"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        public JsonResult HelpOperate(FormCollection fc)
        {
            try
            {
                var article = new ArticleInfo_Add();
                HelpMenuDetailListInfo helpDetail = new HelpMenuDetailListInfo();
                helpDetail.CreateTime = Convert.ToDateTime(PreconditionAssert.IsNotEmptyString(fc["createTime"], "发布时间不能为空"));
                helpDetail.HMenuid = int.Parse(PreconditionAssert.IsNotEmptyString(fc["category"], "分类不能为空"));
                helpDetail.DisplayName = PreconditionAssert.IsNotEmptyString(fc["title"], "标题不能为空");
                helpDetail.InUserId = CurUser.UserId;
                helpDetail.InUserName = CurUser.UserName;
                helpDetail.Description = PreconditionAssert.IsNotEmptyString(fc["description"], "内容不能为空");
                helpDetail.HtmlTitle = article.KeyWords = fc["keyWords"];
                helpDetail.DescContent = fc["descContent"];
                helpDetail.IsPuttop = string.IsNullOrWhiteSpace(fc["isPutTop"]) ? false : true;//是否置顶
                var result = new CommonActionResult();
                if (!string.IsNullOrWhiteSpace(fc["did"]))
                {
                    string did = fc["did"];
                    helpDetail.DId = int.Parse(did);
                    result = base.ExternalClient.UpdateHelpManagement(helpDetail);
                }
                else
                {
                    result = base.ExternalClient.InsertHelpManagement(helpDetail);
                }
                //同步生成静态页
                SendBuildStaticDataNotice("1000", helpDetail.HMenuid.ToString());
                return Json(new CommonResult { code = (int)ResultCodeEnum.OK, message = result.Message });

            }
            catch (Exception ex)
            {
                return Json(new CommonResult { code = (int)ResultCodeEnum.SystemError, message = ex.Message });
            }
        }

        /// <summary>
        /// 禁用\启用 - 帮助内容
        /// </summary>
        public JsonResult IsDisnableHelp()
        {
            try
            {
                var isEnable = Convert.ToBoolean(PreconditionAssert.IsNotNull(Request["isEnable"], "修改指定参数ID丢失"));
                var Did = PreconditionAssert.IsNotEmptyString(Request["Did"], "修改指定参数ID丢失");
                var result = base.ExternalClient.HelpIsEnableAdmin(Did, isEnable);
                try
                {
                    // new SiteSettingsController().BulletinInner();
                }
                catch
                {
                }
                return Json(new { IsSuccess = result.IsSuccess, Msg = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }
        #endregion

        #region ==文章管理==
        /// <summary>
        /// 文章管理
        /// </summary>
        [CheckPermission("N103")]
        public ActionResult ArticleManage()
        {
            try
            {
                bool fbwz = false;
                bool scwz = false;
                bool xgwz = false;
                bool gxxlh = false;
                if (CheckRights("FBWZ100"))
                    fbwz = true;
                if (CheckRights("SCWZ120"))
                    scwz = true;
                if (CheckRights("XGWZ110"))
                    xgwz = true;
                if (CheckRights("GXXLH130"))
                    gxxlh = true;
                ViewBag.Publish = fbwz;
                ViewBag.Delete = scwz;
                ViewBag.Update = xgwz;
                ViewBag.UpdateSerial = gxxlh;
                ViewBag.GameList = this.GameIssuseClient.QueryGameListAdmin();
                ViewBag.Admins = GlobalCache.ExternalClient.GetSystemUsers(new SystemUserSearchAddition()
                {
                    PageIndex = 1,
                    PageSize = 100000
                });
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult GetArticleInfos(ArticleConditionModel model)
        {
            LayuiGridResult result = new LayuiGridResult() { code = 0 };

            var con = new ArticleCondition()
            {
                Category = model.Category == "ALL"?"" : model.Category,
                StartCreationDate = model.StartCreationDate,
                Status = model.Status,
                PageSize = model.PageSize,
                EndCreationDate = model.EndCreationDate,
                GameCode = model.GameCode,
                OperatorUserId = model.CreatorUserId,
                PageIndex = model.PageIndex,
                Title = model.Title
            };
            var p = GlobalCache.ExternalClient.GetArticleList(con);
            if (p != null)
            {
                result.count = p.TotalCount;
                int i = (model.PageIndex - 1) * model.PageSize;
                result.data = p.ArticleList?.Select(a =>
                {
                    i++;
                    return new
                    {
                        Id = i,
                        ArticleId = a.Id,
                        Category = GlobalCache.GetArticleCategory(a.Category),
                        Title = a.Title,
                        CreateTime = a.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        UpdateUserDisplayName = a.UpdateUserDisplayName,
                        IsPutTop = a.IsPutTop,
                        Status = a.Status,
                        StaticPath = string.Format("{0}{1}", ConfigurationManager.AppSettings["WebSiteUrl"], a.StaticPath)
                    };
                }).ToList();
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 发布文章
        /// </summary>
        public ActionResult ArticleInfo(string id)
        {
            try
            {
                ViewBag.GameList = this.GameIssuseClient.QueryGameListAdmin();
                if (!string.IsNullOrWhiteSpace(Request["articleId"]))
                {
                    string articleId = Request["articleId"];
                    ViewBag.Article = base.ExternalClient.QueryArticleById_Admin(articleId);
                }
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 发布文章
        /// </summary>
        [ValidateInput(false), HttpPost]
        public JsonResult ArticleOperate(ArticleModel model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Category))
                {
                    return Json(new CommonResult { code = (int)ResultCodeEnum.VerifyError, message = "分类不能为空" });
                }
                if (string.IsNullOrWhiteSpace(model.Title))
                {
                    return Json(new CommonResult { code = (int)ResultCodeEnum.VerifyError, message = "标题不能为空" });
                }
                if (string.IsNullOrWhiteSpace(model.Description))
                {
                    return Json(new CommonResult { code = (int)ResultCodeEnum.VerifyError, message = "内容不能为空" });
                }
                if (string.IsNullOrWhiteSpace(model.ImgType))
                {
                    return Json(new CommonResult { code = (int)ResultCodeEnum.VerifyError, message = "图片类型不能为空" });
                }
                else
                {
                    if ((model.ImgType == "1" || model.ImgType == "3") && string.IsNullOrWhiteSpace(model.Url)) {
                        return Json(new CommonResult { code = (int)ResultCodeEnum.VerifyError, message = "请上传图片" });
                    }
                    else if (model.ImgType == "3" && (string.IsNullOrWhiteSpace(model.Url1) || string.IsNullOrWhiteSpace(model.Url2))) {
                        return Json(new CommonResult { code = (int)ResultCodeEnum.VerifyError, message = "需要上传三张图片" });
                    }
                }
                var article = new ArticleInfo_Add();
                article.Category = model.Category;
                article.IsRedTitle = model.IsRedTitle;
                article.CreateUserDisplayName = CurUser.UserName;
                article.CreateUserKey = CurUser.UserId;
                article.Description = model.Description;
                article.Title = model.Title;
                article.KeyWords = model.KeyWords;
                article.DescContent = model.DescContent;
                article.GameCode = model.GameCode;
                article.IsPutTop = model.IsPutTop;//是否置顶
                article.Priority = model.Priority;
                article.Url = model.Url;
                article.Url1 = model.Url1;
                article.Url2 = model.Url2;
                article.ImgType = model.ImgType;
                article.Status = model.Status;
                article.PublishTime = model.PublishTime;
                var result = new CommonActionResult();
                if (!string.IsNullOrWhiteSpace(model.Id))
                {
                    result = GlobalCache.ExternalClient.UpdateArticle(model.Id, article);
                }
                else
                {
                    result = GlobalCache.ExternalClient.SubmitArticle(article);
                }
                try
                {
                    var originCategory = model.OriginalCategory;
                    List<string> gameCodeList = new List<string>();
                    if (!string.IsNullOrEmpty(model.GameCode))
                    {
                        gameCodeList.Add(model.GameCode);
                    }
                    if (!string.IsNullOrWhiteSpace(originCategory) && !gameCodeList.Contains(originCategory))
                    {
                        gameCodeList.Add(originCategory);
                    }
                    this.ArticleInner(gameCodeList.ToArray());
                }
                catch (Exception ex)
                {
                    logger.Error("ArticleOperate ArticleInner", ex);
                }
                try
                {
                    //同步生成静态页
                    if (Request["chkBuildStatic"] != null)
                    {
                        bool ischeck = false;
                        bool.TryParse(Request["chkBuildStatic"], out ischeck);
                        if (Request["chkBuildStatic"] == "checked" || ischeck)
                        {
                            var arrPageType = new string[] { "10", "50", "60" };
                            foreach (var item in arrPageType)
                            {
                                SendBuildStaticDataNotice(item, string.Empty);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("ArticleOperate SendBuildStaticDataNotice", ex);
                    return Json(new CommonResult { code = (int)ResultCodeEnum.APIError, message = "生成静态页面失败," + ex.Message });
                }
                return Json(new CommonResult { code = (int)ResultCodeEnum.OK, message = "保存成功" });
            }
            catch (Exception ex)
            {
                logger.Error("ArticleOperate", ex);
                return Json(new CommonResult { code = (int)ResultCodeEnum.APIError, message = "服务出错," + ex.Message });
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        [HttpPost]
        public JsonResult EnableArticle(string id, int status, string gameCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return Json(new { status = 0, message = "删除编号不能为空" });
                }
                string message = string.Empty;
                if (status == 0)
                {
                    message = "禁用成功";
                    GlobalCache.ExternalClient.DisableArticle(id);
                }
                else if (status == 1)
                {
                    message = "启用成功";
                    GlobalCache.ExternalClient.EnableArticle(id);
                }
                else
                {
                    return Json(new { status = 0, message = "状态值不对" });
                }
                try
                {
                    List<string> gameCodeList = new List<string>();
                    gameCodeList.Add(gameCode);
                    this.ArticleInner(gameCodeList.ToArray());
                }
                catch (Exception ex)
                {
                    logger.Error("DeleteArticle ArticleInner", ex);
                }
                return Json(new { status = 1, message = message });
            }
            catch (Exception ex)
            {
                logger.Error("DeleteArticle", ex);
                return Json(new { status = 0, message = "服务出错" + ex.Message });
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        [HttpPost]
        public JsonResult DeleteArticle(string id, string gameCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return Json(new { status = 0, message = "删除编号不能为空" });
                }
                var result = GlobalCache.ExternalClient.DeleteArticle(id);
                try
                {
                    List<string> gameCodeList = new List<string>();
                    gameCodeList.Add(gameCode);
                    this.ArticleInner(gameCodeList.ToArray());
                }
                catch (Exception ex)
                {
                    logger.Error("DeleteArticle ArticleInner", ex);
                }
                return Json(new { status = 1, message = result.Message });
            }
            catch (Exception ex)
            {
                logger.Error("DeleteArticle", ex);
                return Json(new { status = 0, message = "服务出错" + ex.Message });
            }
        }
        public void ArticleInner(params string[] gameCodes)
        {
            var gameCodeList = new List<string>(new string[] { "" });
            gameCodeList.AddRange(gameCodes);

            var argList = new List<string>(new string[] { string.Format("arg={0}|{1}", "hot_article_home", 10) });
            foreach (var gameCode in gameCodeList)
            {
                argList.Add(string.Format("arg={0}|{1}|{2}", "article_info", gameCode, 7));
                argList.Add(string.Format("arg={0}|{1}|{2}", "article_expert", gameCode, 4));
            }
            if (gameCodeList.Contains("SD11X5") || gameCodeList.Contains("GD11X5") || gameCodeList.Contains("JX11X5"))
            {
                argList.Add(string.Format("arg={0}|{1}|{2}", "article_info", "11X5", 10));
                argList.Add(string.Format("arg={0}|{1}|{2}", "article_expert", "11X5", 10));
            }
            if (gameCodeList.Contains("CQSSC") || gameCodeList.Contains("JXSSC"))
            {
                argList.Add(string.Format("arg={0}|{1}|{2}", "article_info", "SSC", 10));
                argList.Add(string.Format("arg={0}|{1}|{2}", "article_expert", "SSC", 10));
            }
            var url = string.Format("{0}/{1}", ConfigurationManager.AppSettings["WebSiteUrl"], "Cache/Write");
            foreach (var arg in argList)
            {
                var result = PostManager.Post(url, arg, Encoding.UTF8);
                if (result != "1")
                {
                    throw new Exception("更新\"" + arg + "\"失败 -" + result);
                }
            }

            //更新手机网站缓存文件
            //var mobileurl = string.Format("{0}/{1}", ConfigurationManager.AppSettings["WebSiteUrlMobile"], "Cache/Write");
            //var mobilePostResult = PostManager.Post(mobileurl, "arg=article_info|5", Encoding.UTF8);
            //if (mobilePostResult != "1")
            //{
            //    throw new Exception("更新手机站点缓存文件失败。 - " + mobilePostResult);
            //}
        }
        public JsonResult UpdateArticleIndex()
        {
            try
            {
                //     序号描述。ID1,1|ID2,2
                string indexDescription = PreconditionAssert.IsNotEmptyString(Request["indexDescription"], "序号修改ID组异常");
                var result = base.ExternalClient.UpdateArticleIndex(indexDescription);
                var gameCode = string.IsNullOrWhiteSpace(Request["gameCode"]) ? "" : Request["gameCode"];
                try
                {
                    List<string> gameCodeList = new List<string>();
                    gameCodeList.Add(gameCode);
                    this.ArticleInner(gameCodeList.ToArray());
                }
                catch
                {
                }
                return Json(new { IsSuccess = result.IsSuccess, Msg = result.Message });
            }
            catch (Exception ex)
            {
                logger.Error("UpdateArticleIndex", ex);
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }

        #endregion

        #region 广告图

        public ActionResult BannerManage()
        {
            if (!CheckRights("N104"))
                throw new Exception("对不起，您的权限不足！");
            bool tjgg = false;
            bool bcgg = false;
            bool scgg = false;
            bool yjqy = false;
            if (CheckRights("TJGG100"))
                tjgg = true;
            if (CheckRights("SCGG120"))
                scgg = true;
            if (CheckRights("BCGG110"))
                bcgg = true;
            if (CheckRights("BCGG110"))
                yjqy = true;
            ViewBag.Tjgg = tjgg;
            ViewBag.Scgg = scgg;
            ViewBag.Bcgg = bcgg;
            ViewBag.JyQy = yjqy;
            ViewBag.PageIndex = Request["pageIndex"] == null ? Request["pageIndex"] == null ? base.PageIndex : Convert.ToInt32(Request["pageIndex"]) : Convert.ToInt32(Request["pageIndex"]);
            ViewBag.PageSize = Request["pageSize"] == null ? Request["pageSize"] == null ? base.PageSize : Convert.ToInt32(Request["pageSize"]) : Convert.ToInt32(Request["pageSize"]);
            var temp = string.IsNullOrEmpty(Request.QueryString["queryType"]) ? "10" : Request.QueryString["queryType"];
            ViewBag.bannerType = (BannerType)int.Parse(temp);
            ViewBag.Lhlist = ExternalClient.newQuerySitemessageBanngerList_Web(ViewBag.bannerType, ViewBag.PageSize, ViewBag.PageIndex - 1);
            return View();
        }

        /// <summary>
        /// 禁用启用事件
        /// </summary>
        public JsonResult IsDisnableBanner(FormCollection fc)
        {
            try
            {
                string bannerId = PreconditionAssert.IsNotEmptyString(fc["BannerId"], "广告图id不能为空");
                string isEnable = PreconditionAssert.IsNotEmptyString(fc["isEnable"], "广告图状态不能为空");
                var bannerInfo = ExternalClient.GetBannerManagerInfo(bannerId);
                if (bannerInfo != null)
                {
                    bannerInfo.IsEnable = bool.Parse(isEnable);
                    ExternalClient.UpdateBannerInfo(bannerInfo);
                }
                return Json(new CommonResult { code = 200, message = "成功" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new CommonResult { code = 200, message = "失败-" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Delete_BannerManage()
        {
            if (!string.IsNullOrWhiteSpace(Request["bannerId"]))
            {
                string bannerId = Convert.ToString(Request["bannerId"]);
                var result = base.ExternalClient.DeleteBanner(int.Parse(bannerId));
                return Json(new CommonResult { code = 200, data = result, message = result.Message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new CommonResult { code = 200, data = string.Empty, message = "删除失败" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        //提交
        public JsonResult RequestBanner(FormCollection fc)
        {
            try
            {
                var bannerId = fc["bannerId"];
                SiteMessageBannerInfo info = new SiteMessageBannerInfo();
                if (!string.IsNullOrWhiteSpace(fc["bannerId"])) { info.BannerId = int.Parse(fc["bannerId"]); }
                info.BannerTitle = Request.Form["bannerTitle"];
                info.BannerType = (BannerType)int.Parse(fc["bannerType"]);
                info.ImageUrl = fc["bannerImageUrl"];
                info.IsEnable = !string.IsNullOrWhiteSpace(fc["isEnable"]) ? fc["isEnable"] == "1" ? true : false : false;
                info.JumpType = int.Parse(fc["jumpType"]);
                info.JumpUrl = info.JumpType == 2 ? fc["jumpUrl_Activity"] : fc["JumpUrl"];
                info.Operator = this.CurUser.UserName;
                CommonActionResult result = null;
                if (!string.IsNullOrWhiteSpace(Request["bannerId"]))
                {
                    result = ExternalClient.UpdateBannerInfoAdmin(info);
                }
                else
                {
                    result = ExternalClient.AddBannerInfoAdmin(info);
                }
                return Json(new CommonResult { code = 200, message = result.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new CommonResult { code = 200, message = "失败-" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ContentResult RequestBannerByAdd()
        {
            try
            {
                SiteMessageBannerInfo siteInfo = new SiteMessageBannerInfo();
                if (Request["hd_Index"] == null)
                    throw new Exception("序号不能为空！");
                siteInfo.BannerIndex = Convert.ToInt32(Request["hd_Index"]) + 1; siteInfo.BannerTitle = Request.Form["title"];
                siteInfo.BannerType = (BannerType)int.Parse(Request.Form["ggtype"]);
                var loadfile = Request.Files["loadfile"];
                siteInfo.ImageUrl = LoadImageFile(loadfile, "/images/add/");
                siteInfo.IsEnable = true;
                var type = 1;
                int.TryParse(Request.Form["add_jumptype"], out type);
                if (type == 0)
                {
                    type = 1;
                }
                siteInfo.JumpType = type;
                siteInfo.JumpUrl = type == 2 ? Request.Form["sel_url"] : Request.Form["targeturl"];

                ExternalClient.AddBannerInfoAdmin(siteInfo);

                return Content("{" + string.Format("IsSuccess:'{0}',Msg:'{1}'", true, "添加成功") + "}");
            }
            catch (Exception ex)
            {
                return Content("{" + string.Format("IsSuccess:'{0}',Msg:'{1}'", false, ex.Message) + "}");
            }
        }

        public JsonResult RequestBannerByDelete()
        {
            try
            {
                var oldindex = int.Parse(Request.Form["oldindex"]);
                ExternalClient.DeleteBannerAdmin(oldindex);
                return Json(new { IsSuccess = true, Msg = "删除成功" });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }

        public ActionResult BannerManageInfo()
        {
            if (!string.IsNullOrWhiteSpace(Request["id"]) && Request["Type"].ToLower() != "add")
            {
                string id = Convert.ToString(Request["id"]);
                ViewBag.Detail = base.ExternalClient.GetBannerManagerInfo(id);
            }
            return View();
        }

        public JsonResult upload()
        {
            var loadfile = Request.Files["flie"];
            string url = LoadImageFile(loadfile, "/images/add/");
            return Json(new CommonResult { }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 静态文件生成页面

        /// <summary>
        /// 静态文件生成页面
        /// </summary>
        /// <returns></returns>
        public ActionResult StaticPageManager()
        {
            ViewBag.List = StaticPageInfos();
            return View();
        }

        /// <summary>
        /// 生成PC静态页面
        /// </summary>
        /// <returns></returns>
        public JsonResult SendBuildStaticPageNotice()
        {
            try
            {
                var key = Request["key"];
                var pageType = Request["pageType"];

                var logList = new List<string>();
                if (pageType == "0")//生成全部页面
                {
                    //var pageTypeList = new string[] { "10", "20", "30", "40", "50", "60", "70", "80", "90", "901", "902", "903", "904", "905", "906", "907", "908", "909", "910", "911", "912", "913", "914", "915", "916", "917", "918", "919", "920", "921", "922", "923", "924", "100", "300", "400", "500", "700", "800", "1000" };
                    Dictionary<string, string> pageTypeList = GetPageType(1);
                    foreach (var p in pageTypeList)
                    {
                        BuildStatisPage(ref logList, key, p.Key, p.Value);
                        System.Threading.Thread.Sleep(500);
                    }
                }
                else//生成单个页面
                {
                    BuildStatisPage(ref logList, key, pageType, "");
                }
                SaveStaticPageFile(pageType, "PC端");
                return Json(new { IsSuccess = true, Msg = string.Join(Environment.NewLine, logList), Key = key }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 生成m端静态页面
        /// </summary>
        /// <returns></returns>
        public JsonResult SendBuildStaticPageNotice_Wap()
        {
            try
            {
                var key = Request["key"];
                var pageType = Request["pageType"];
                var logList = new List<string>();
                if (pageType == "0")//生成全部页面
                {
                    Dictionary<string, string> pageTypeList = GetPageType(2);
                    foreach (var p in pageTypeList)
                    {
                        BuildStatisPage_Wap(ref logList, key, p.Key, p.Value);
                        System.Threading.Thread.Sleep(500);
                    }
                }
                else//生成单个页面
                {
                    BuildStatisPage_Wap(ref logList, key, pageType);
                }
                SaveStaticPageFile(pageType, "M端");
                return Json(new { IsSuccess = true, Msg = string.Join(Environment.NewLine, logList), Key = key }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 生成APP静态页面
        /// </summary>
        /// <returns></returns>
        public JsonResult SendBuildStaticPageNotice_App()
        {
            try
            {
                var key = Request["key"];
                var pageType = Request["pageType"];
                var code = Encipherment.MD5(string.Format("Home_BuildSpecificPage_{0}", pageType), Encoding.UTF8);
                var urlArray = ConfigurationManager.AppSettings["SendUrl_App"].Split('|');
                var logList = new List<string>();
                foreach (var domain in urlArray)
                {
                    if (string.IsNullOrEmpty(domain)) continue;
                    try
                    {
                        var webSiteUrl = string.Format("{0}/{1}?pageType={2}&code={3}&key={4}", domain, "StaticHtml/BuildSpecificPage", pageType, code, key);
                        var result = PostManager.Get(webSiteUrl, Encoding.UTF8, timeoutSeconds: 60);
                        logList.Add(string.Format("域名{0}生成结果{1}", domain, result));
                    }
                    catch (Exception ex)
                    {
                        logList.Add(string.Format("域名{0}生成结果{1}", domain, ex.Message));
                    }
                }
                return Json(new { IsSuccess = true, Msg = string.Join(Environment.NewLine, logList), Key = key }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 读取
        /// </summary>
        /// <returns></returns>
        public List<StaticPageInfo> StaticPageInfos()
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            if (objCache["staticPageInfos"] != null)
            {
                var lotteryInfo = (List<StaticPageInfo>)objCache["staticPageInfos"];
                if (lotteryInfo != null && lotteryInfo.Count > 0)
                    return lotteryInfo;
            }
            string str = System.IO.File.ReadAllText(Server.MapPath("\\Views\\ContentManage\\StaticPageData.json"), Encoding.UTF8);
            List<StaticPageInfo> list = new List<StaticPageInfo>();
            if (!string.IsNullOrWhiteSpace(str))
            {
                list = JsonConvert.DeserializeObject<List<StaticPageInfo>>(str);
            }
            objCache.Insert("staticPageInfos", list, null, DateTime.Now.AddMinutes(10), System.Web.Caching.Cache.NoSlidingExpiration);  //添加静态页面文件
            return list;
        }

        /// <summary>
        /// 修改or保存
        /// </summary>
        /// <param name="pageType"></param>
        public void SaveStaticPageFile(string pageType,string type)
        {
            string path = Server.MapPath("\\Views\\ContentManage\\StaticPageData.json");
            try
            {
                var list = StaticPageInfos();
                StaticPageInfo info = list.Where(a => a.Value == pageType && a.Type == type).FirstOrDefault();
                int index = list.IndexOf(info);
                info.CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                list.RemoveAt(index);
                list.Insert(index, info);
                System.IO.File.WriteAllText(path, JsonConvert.SerializeObject(list), Encoding.UTF8);
                System.Web.Caching.Cache objCache = HttpRuntime.Cache;
                objCache.Insert("staticPageInfos", list, null, DateTime.Now.AddMinutes(10), System.Web.Caching.Cache.NoSlidingExpiration);
            }
            catch { }
        }

        #endregion
       

        

        #region 生成静态页私有方法
        private void BuildStatisPage(ref List<string> logList, string key, string pageType, string pageName = "")
        {
            var code = Encipherment.MD5(string.Format("Home_BuildSpecificPage_{0}", pageType), Encoding.UTF8);
            var urlArray = ConfigurationManager.AppSettings["SendUrl_Web"].Split('|');
            foreach (var domain in urlArray)
            {
                if (string.IsNullOrEmpty(domain)) continue;

                try
                {
                    var webSiteUrl = string.Format("{0}/{1}?pageType={2}&code={3}&key={4}", domain, "StaticHtml/BuildSpecificPage", pageType, code, key);
                    var str = PostManager.Get(webSiteUrl, Encoding.UTF8, timeoutSeconds: 60);
                    logList.Add(string.Format("域名{0}生成结果{1}, [{2}]", domain, str, pageName));
                }
                catch (Exception ex)
                {
                    logList.Add(string.Format("异常：域名{0}生成结果{1}, [{2}]", domain, ex.Message, pageName));
                }
            }
        }

        private void BuildStatisPage_Wap(ref List<string> logList, string key, string pageType, string pageName = "")
        {
            var code = Encipherment.MD5(string.Format("Home_BuildSpecificPage_{0}", pageType), Encoding.UTF8);
            var urlArray = ConfigurationManager.AppSettings["SendUrl_Wap"].Split('|');
            foreach (var domain in urlArray)
            {
                if (string.IsNullOrEmpty(domain)) continue;
                try
                {
                    var webSiteUrl = string.Format("{0}/{1}?pageType={2}&code={3}&key={4}", domain, "StaticHtml/BuildSpecificPage", pageType, code, key);
                    var result = PostManager.Get(webSiteUrl, Encoding.UTF8, timeoutSeconds: 60);
                    logList.Add(string.Format("域名{0}生成结果{1}, [{2}]", domain, result, pageName));
                }
                catch (Exception ex)
                {
                    logList.Add(string.Format("异常：域名{0}生成结果{1}, [{2}]", domain, ex.Message, pageName));
                }
            }
        }

        /// <summary>
        /// 获取页面类型
        /// </summary>
        /// <param name="tag">1.pc 2.wap</param>
        /// <returns></returns>
        private static Dictionary<string, string> GetPageType(int tag)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (tag == 1)
            {
                dic.Add("10", "首页");
                dic.Add("20", "中奖排行榜");
                dic.Add("30", "开奖结果");
                dic.Add("40", "数字彩最新期开奖号");
                dic.Add("50", "资讯首页和分类列表页");
                dic.Add("60", "资讯详细");
                dic.Add("70", "公告");
                dic.Add("80", "神单主页");
                dic.Add("90", "走势图主页");
                dic.Add("901", "双色球走势图表");
                dic.Add("902", "福彩3D走势图表");
                //dic.Add("903", "七乐彩走势图表");
                //dic.Add("904", "吉林快3走势图表");
                dic.Add("905", "江苏快3走势图表");
                //dic.Add("906", "湖北快3走势图表");
                //dic.Add("907", "山东群英会走势图表");
                //dic.Add("908", "好彩1走势图表");
                //dic.Add("909", "华东15选5走势图表");
                //dic.Add("910", "东方6 + 1走势图表");
                dic.Add("911", "大乐透走势图表");
                dic.Add("912", "排列三走势图表");
                //dic.Add("913", "排列五走势图表");
                //dic.Add("914", "七星彩走势图表");
                dic.Add("915", "山东11选5走势图表");
                dic.Add("916", "广东11选5走势图表");
                dic.Add("917", "江西11选5走势图表");
                //dic.Add("918", "重庆11选5走势图表");
                //dic.Add("919", "辽宁11选5走势图表");
                dic.Add("920", "重庆时时彩走势图表");
                //dic.Add("921", "江西时时彩走势图表");
                dic.Add("922", "广东快乐十分走势图表");
                //dic.Add("923", "湖南快乐十分走势图表");
                //dic.Add("924", "重庆快乐十分走势图表");
                dic.Add("100", "购彩大厅");
                dic.Add("300", "用户注册页面");
                dic.Add("400", "会员中心页面");
                dic.Add("500", "合买大厅");
                dic.Add("700", "活动首页");
                dic.Add("800", "过关统计");
                dic.Add("1000", "帮助中心");
            }
            else
            {
                dic.Add("10", "首页");
                dic.Add("30", "开奖结果");
                dic.Add("40", "数字彩最新期开奖号");
                dic.Add("60", "资讯详细");
                dic.Add("80", "神单主页");
                dic.Add("500", "合买大厅");
                dic.Add("1000", "帮助中心");
            }
            return dic;
        }
        #endregion

        #region 公司信息配置

        [CheckPermission("GSXXPZ")]
        public ActionResult CompanyConfigInfo()
        {
            var model = ExternalClient.GetCompanyConfigInfoList();
            return View(model);
        }

        [HttpPost]
        public JsonResult RequestCompanyConfigInfo(FormCollection fc)
        {
            try
            {
                CompanyConfig companyConfig = new CompanyConfig();
                companyConfig.Id = int.Parse(fc["oldindex"]);
                companyConfig.TopLogoUrl = fc["toplogourl"];
                companyConfig.BottonLogoUrl = fc["bottonlogourl"];
                companyConfig.Remark = fc["weixinurl"];
                companyConfig.AppDownloadUrl = fc["appdownloadurl"];
                //companyConfig.TopLogoUrl = LoadImageFile(Request.Files["toplogourl"], "/images/add/admin/", "header_logo");
                //companyConfig.BottonLogoUrl = LoadImageFile(Request.Files["bottonlogourl"], "/images/add/admin/", "footer_logo");
                //companyConfig.Remark = LoadImageFile(Request.Files["weixinurl"], "/images/add/admin/", "weixin_logo");
                //companyConfig.AppDownloadUrl = LoadImageFile(Request.Files["appdownloadurl"], "/images/add/admin/", "appdownload");
                companyConfig.WebSiteName = fc["websitename"];
                companyConfig.CompanyName = fc["companyname"];
                companyConfig.Icp = fc["icp"];
                companyConfig.Tell = fc["tell"];
                companyConfig.BusinessContactsName = fc["businesscontactsname"];
                companyConfig.BusinessContactsEmail = fc["businesscontactsemail"];
                companyConfig.ServiceName = fc["servicename"];
                companyConfig.ServiceEmail = fc["serviceemail"];
                companyConfig.WebSiteUrl = fc["websiteurl"];
                companyConfig.PaySiteUrl = fc["paysiteurl"];
                companyConfig.MPaySiteUrl = fc["mpaysiteurl"];
                companyConfig.ServiceSiteUrl = fc["servicesiteurl"];
                var ServicePercent = fc["ServicePercent"];
                var CommisstionMoney = "0";
                companyConfig.CustomerWxAccount = fc["CustomerWxAccount"];

                var result = ExternalClient.UpdateCompanyConfigInfo(
                    companyConfig.Id.ToString()
                    , companyConfig.TopLogoUrl
                    , companyConfig.BottonLogoUrl
                    , companyConfig.WebSiteName
                    , companyConfig.CompanyName
                    , companyConfig.Icp
                    , companyConfig.Tell
                    , companyConfig.WebSiteUrl
                    , companyConfig.Remark
                    , companyConfig.BusinessContactsName
                    , companyConfig.BusinessContactsEmail
                    , companyConfig.ServiceName
                    , companyConfig.ServiceEmail
                    , companyConfig.PaySiteUrl
                    , companyConfig.MPaySiteUrl
                    , companyConfig.ServiceSiteUrl
                    , companyConfig.AppDownloadUrl
                    , ServicePercent
                    , CommisstionMoney
                    , companyConfig.CustomerWxAccount
                    );
                return Json(new { success = true, msg = "修改成功！" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "修改失败！" });
            }
        }

        #endregion

        #region 第三方账号配置


        [CheckPermission("DSFZHPZ")]
        public ActionResult ThirdPartyAccountManage()
        {
            ViewBag.Key = string.IsNullOrWhiteSpace(Request["key"]) ? "" : Request["key"];
            ViewBag.PageIndex = string.IsNullOrWhiteSpace(Request["pageIndex"]) ? base.PageIndex : Convert.ToInt32(Request["pageIndex"]);
            ViewBag.PageSize = string.IsNullOrWhiteSpace(Request["pageSize"]) ? base.PageSize : Convert.ToInt32(Request["pageSize"]);
            ThirdPartyAccountCollection model = ExternalClient.ThirdPartyAccount_Load(ViewBag.Key, ViewBag.PageIndex - 1, ViewBag.PageSize);
            ViewBag.ThirdPartyAccountResult = model;
            return View();
        }

        /// <summary>
        /// 暂停第三方账号
        /// </summary>
        [HttpPost]
        public JsonResult StopThirdPartyAccount()
        {
            try
            {
                long id = long.Parse(Request["id"]);
                var result = base.ExternalClient.ThirdPartyAccount_Stop(id);
                return Json(new { IsSuccess = result.IsSuccess, Msg = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }

        /// <summary>
        /// 启用第三方账号
        /// </summary>
        [HttpPost]
        public JsonResult StartThirdPartyAccount()
        {
            try
            {
                long id = long.Parse(Request["id"]);
                var result = base.ExternalClient.ThirdPartyAccount_Start(id);
                return Json(new { IsSuccess = result.IsSuccess, Msg = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }

        /// <summary>
        /// 查看账号配置详情
        /// </summary>
        public ActionResult ThirdPartyAccountInfo(int id)
        {
            try
            {
                ViewBag.ThirdPartyAccountConfig = base.ExternalClient.ThirdPartyAccount_LoadConfig(id);
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 保存第三方账号配置
        /// </summary>
        [ValidateInput(false)]
        public JsonResult SaveThirdPartyAccount(List<ThirdPartyAccountConfigInfo> list)
        {
            try
            {
                foreach (var item in list)
                {
                    if (string.IsNullOrEmpty(item.ConfigKey) || string.IsNullOrEmpty(item.ConfigValue))
                    {
                        //参数不能为空
                        return Json(new { success = false, msg = "参数不能为空" });
                    }
                }
                ThirdPartyAccountConfigCollection model = new ThirdPartyAccountConfigCollection();
                model.ThirdPartyAccountConfigList = list;
                var result = base.ExternalClient.ThirdPartyAccount_ModifyConfig(model);
                return Json(new { success = true, msg = "保存成功" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = ex.Message });
            }
        }

        /// <summary>
        /// 刷新第三方账号Redis
        /// </summary>
        [HttpPost]
        public JsonResult RefreshThirdPartyAccountRedis()
        {
            try
            {
                var result = base.ExternalClient.ThirdPartyAccount_RefreshRedis();
                return Json(new { IsSuccess = result.IsSuccess, Msg = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }

        #endregion

        #region 支付接口配置

        public ActionResult PaymentInterfaceManage()
        {
            //验证是否具有系统配置权限
            if (!CheckRights("ZFJKPZ"))
                throw new Exception("对不起，您的权限不足！");

            //获取参数
            ViewBag.Client = string.IsNullOrEmpty(Request.QueryString["Client"]) ? 0 : int.Parse(Request.QueryString["Client"]);
            ViewBag.Name = string.IsNullOrWhiteSpace(Request["Name"]) ? "" : Request["Name"];
            ViewBag.PaymentPassagewayId = string.IsNullOrEmpty(Request.QueryString["PaymentPassagewayId"]) ? 0 : int.Parse(Request.QueryString["PaymentPassagewayId"]);
            ViewBag.Status = string.IsNullOrEmpty(Request.QueryString["Status"]) ? 0 : int.Parse(Request.QueryString["Status"]);
            ViewBag.PageIndex = string.IsNullOrWhiteSpace(Request["pageIndex"]) ? base.PageIndex : Convert.ToInt32(Request["pageIndex"]);
            ViewBag.PageSize = string.IsNullOrWhiteSpace(Request["pageSize"]) ? base.PageSize : Convert.ToInt32(Request["pageSize"]);

            //加载数据
            ViewBag.PaymentInterfaceResult = ExternalClient.PaymentInterface_Load(ViewBag.Client, ViewBag.Name, ViewBag.PaymentPassagewayId, ViewBag.Status, ViewBag.PageIndex - 1, ViewBag.PageSize);
            return View();
        }

        /// <summary>
        /// 暂停支付接口
        /// </summary>
        public ActionResult StopPaymentInterface(long id)
        {
            try
            {
                var result = base.ExternalClient.PaymentInterface_Stop(id);
                return RedirectToAction("PaymentInterfaceManage");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 启用支付接口
        /// </summary>
        public ActionResult StartPaymentInterface(long id)
        {
            try
            {
                var result = base.ExternalClient.PaymentInterface_Start(id);
                return RedirectToAction("PaymentInterfaceManage");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 删除支付接口
        /// </summary>
        [HttpPost]
        public JsonResult DeletePaymentInterface(FormCollection fc)
        {
            try
            {
                var id = 0l;
                long.TryParse(fc["id"], out id);
                var result = base.ExternalClient.PaymentInterface_Delete(id);
                return Json(new { IsSuccess = true });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }

        /// <summary>
        /// 查看支付接口详情
        /// </summary>
        public ActionResult PaymentInterfaceInfo(long id)
        {
            try
            {
                PaymentInterfaceInfo model;
                if (id > 0)
                {
                    model = base.ExternalClient.PaymentInterface_LoadById(id);
                }
                else
                {
                    model = new PaymentInterfaceInfo();
                }
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 保存支付接口
        /// </summary>
        [HttpPost]
        public JsonResult RequestPaymentInterfaceInfo(FormCollection fc)
        {
            try
            {
                PaymentInterfaceInfo info = new PaymentInterfaceInfo();
                info.Id = int.Parse(fc["Id"]);
                info.LevelOrder = string.IsNullOrWhiteSpace(fc["LevelOrder"]) ? 0 : int.Parse(fc["LevelOrder"]);
                info.Client = string.IsNullOrWhiteSpace(fc["Client"]) ? ClientType.Null : (ClientType)Enum.ToObject(typeof(ClientType), int.Parse(fc["Client"].ToString()));
                info.Name = fc["Name"];
                info.Icon = fc["Icon"];
                info.Remark = fc["Remark"];
                string param = fc["PaymentPassageway"];
                if (!string.IsNullOrWhiteSpace(param))
                {
                    string[] paymentPassagewayArray = param.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var paymentPassageway in paymentPassagewayArray)
                    {
                        string[] PaymentPassagewayArray = paymentPassageway.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                        if (!string.IsNullOrWhiteSpace(info.PaymentPassagewayIds))
                        {
                            info.PaymentPassagewayIds += ",";
                        }
                        if (!string.IsNullOrWhiteSpace(info.PaymentPassagewayNames))
                        {
                            info.PaymentPassagewayNames += ",";
                        }
                        info.PaymentPassagewayIds += PaymentPassagewayArray[0];
                        info.PaymentPassagewayNames += PaymentPassagewayArray[1];
                    }
                }

                bool status = false;
                var Status = fc["Status"];
                if (Status != null && Status.ToUpper().ToString() == "ON")
                {
                    status = true;
                }
                info.Status = status;


                if (info.Id < 1)
                {
                    base.ExternalClient.PaymentInterface_Insert(info);
                }
                else
                {
                    base.ExternalClient.PaymentInterface_Modify(info);
                }

                return Json(new { success = true, msg = "保存成功！" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = "保存失败！" + ex.Message });
            }
        }

        #endregion
    }
}