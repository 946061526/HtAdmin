using Common.Cryptography;
using Common.Net;
using Common.Utilities;
using External.Core.Login;
using GameBiz.Core;
using HTAdmin.Filters;
using HTAdmin.IdentityManager;
using HTAdmin.IdentityManager.Dtos;
using HTAdmin.Models;
using log4net;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace HTAdmin.Controllers
{
    public class SiteSettingsController : BaseController
    {
        private ILog logger = LogManager.GetLogger(typeof(SiteSettingsController));

        #region ===属性注入===
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
        #endregion

        #region App版本管理


        public ActionResult AppVersionManage()
        {
            ViewBag.sel_client = Request["sel_client"] ?? "";
            ViewBag.verNo = Request["verNo"] ?? "";
            ViewBag.content = Request["content"] ?? "";
            ViewBag.sel_force = Request["sel_force"] ?? "";

            ViewBag.PageIndex = string.IsNullOrEmpty(Request["pageIndex"]) ? base.PageIndex : int.Parse(Request["pageIndex"]);
            ViewBag.PageSize = string.IsNullOrEmpty(Request["pageSize"]) ? base.PageSize : int.Parse(Request["pageSize"]);

            ViewBag.List = base.ExternalClient.QueryVersionUpdateByPage(ViewBag.sel_client
                , ViewBag.verNo
                , ViewBag.content
                , ViewBag.sel_force
                , ViewBag.PageIndex
                , ViewBag.PageSize);
            return View();
        }

        public ActionResult AppVersionAdd()
        {
            ViewBag.sel_client = "";
            ViewBag.verNo = "";
            ViewBag.CheckVersionNumber = "";
            ViewBag.apkUrl = "";
            ViewBag.content = "";
            ViewBag.isForce = "Y";
            ViewBag.fullPath = "";
            ViewBag.id = Request["id"] == null ? "" : Request["id"].ToString();
            if (ViewBag.id != "")//编辑，需初始化数据
            {
                try
                {
                    External.Core.Version.ReturnModel ReturnModel = base.ExternalClient.QueryVersionUpdate("", "", ViewBag.id);
                    if (ReturnModel != null)
                    {
                        ViewBag.sel_client = ReturnModel.model.clienttype.ToString();
                        ViewBag.verNo = ReturnModel.model.VersionNo;
                        ViewBag.CheckVersionNumber = ReturnModel.model.CheckVersionNumber;
                        var url = ReturnModel.model.clienttype == 1 ? ReturnModel.model.AndroidUpdateUrl : ReturnModel.model.IosUpdateUrl;
                        ViewBag.apkUrl = url;
                        ViewBag.content = ReturnModel.model.UpdateDes;
                        ViewBag.isForce = ReturnModel.model.IsHardUpdate;

                        ////文件保存目录
                        //string fullPath = string.Format("{0}/{1}/{2}/", UploadFileRequestUrl, WebSiteEName, FileSavePath);//比如：http:img.cai.com/HtAdmin/Upload/files
                        //ViewBag.fullPath = fullPath + url;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            return View();
        }
        /// <summary>
        /// 新增/修改版本信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AppVersionAddJson()
        {
            try
            {
                string id = Request["id"] == null ? "" : Request["id"].ToString();
                External.Core.Version.VersionUpdateCollection entity = new External.Core.Version.VersionUpdateCollection()
                {
                    VId = id == "" ? 0 : Convert.ToInt32(id),
                    VersionNo = Request.Form["verNo"].ToString(),
                    CheckVersionNumber = Convert.ToInt32(Request.Form["checkVersionNumber"]),
                    IosUpdateUrl = Request.Form["apkUrl"].ToString(),
                    AndroidUpdateUrl = Request.Form["apkUrl"].ToString(),
                    IsHardUpdate = Request.Form["isForce"].ToString(),
                    UpdateDes = Request.Form["content"].ToString(),
                    clienttype = Convert.ToInt32(Request["sel_client"]),
                    created_time = DateTime.Now
                };
                Common.Communication.CommonActionResult result = new Common.Communication.CommonActionResult();
                if (id != "")//编辑
                    result = ExternalClient.UpdateVersionManager(entity);
                else //新增
                    result = ExternalClient.InsertVersionManager(entity);

                return Json(new { IsSuccess = result.IsSuccess, Msg = result.Message });
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }

        /// <summary>
        /// 删除版本信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AppVersionDelete()
        {
            string id = Request["id"] == null ? "" : Request["id"].ToString();
            if (id == "")
                return Json(new { IsSuccess = false, Msg = "参数错误" });
            else
            {
                var result = ExternalClient.DeleteVersionManager(id);
                return Json(new { IsSuccess = result.IsSuccess, Msg = result.Message });
            }
        }
        /// <summary>
        /// 版本更新记录
        /// </summary>
        /// <returns></returns>
        public ActionResult AppVersionHistory()
        {
            string t = Request["t"] == null ? "1" : Request["t"].ToString();
            ViewBag.Title = t == "1" ? "Android版本更新记录" : "IOS版本更新记录";
            External.Core.Version.EVersionUpdate_Collection list = ExternalClient.QueryVersionUpdateByPage(t, "", "", "", 1, 100);
            ViewBag.List = list.EVersionUpdateList;
            return View();
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="file">文件</param>
        /// <param name="uploadfile">保存目录</param>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UploadFileApp(HttpPostedFileBase file, string uploadfile = "", string fileName = "")
        {
            if (uploadfile == "")
                uploadfile = "/files/appPackages/";
            return base.UploadFile(file, uploadfile, fileName);
        }
        #endregion

        #region 后台日志查询
        public ActionResult SystemOperLog()
        {

            var list =  UserManager.GetAdminUsers(new UserSearchAdditionDto() {
                PageSize = 10000,
                PageIndex = 1,
            });
            if(list!=null && list.Data !=null && list.TotalCount > 0)
            {
                ViewBag.Admins = list.Data;
            }
            return View();
        }

        [CheckPermission("N101")]
        public JsonResult GetSystemOperLogInfos(SysOperationLogInfoModel info)
        {
            LayuiGridResult result = new LayuiGridResult() { code = 0 };
            var list = new AdminOperLogManager().GetAdminOperationLogList(info.StartCreationDate,info.EndCreationDate, info.Name, info.Description, info.OperationName, info.PageIndex - 1, info.PageSize);
            result = new LayuiGridResult { data = list.Data, msg = "成功",count = list.TotalCount };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}