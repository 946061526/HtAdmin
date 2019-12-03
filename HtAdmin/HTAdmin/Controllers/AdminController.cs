using Common.Cryptography;
using Common.Net;
using Common.Utilities;
using GameBiz.Core;
using HTAdmin.IdentityManager;
using HTAdmin.IdentityManager.Dtos;
using HTAdmin.Models;
using log4net;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HTAdmin.Controllers
{
    [Authorize]
    public class AdminController : BaseController
    {
        private ILog logger = LogManager.GetLogger(typeof(SiteSettingsController));
        // 暂时不显示的权限名称
        private static List<string> excludeFuncIds = new List<string>()
        {
            "M101", // 名家管理
            "S101", // 用户服务
            "C107", // 自动对比网站与出票中心订单数据
            "C108", // 报表导出列表
            "C111", // 成长值明细
            "J103", // 修改票数据
            "J108", //查询出票列表
            "J109", //打票机网点管理列表
            "J120", //查询虚拟订单列表
            "J110", //查询互爱出票列表
            "J130", //延误开奖订单列表
            "CF001", //彩富广告图管理
        };

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
        private AdminRoleManager _roleManager;
        public AdminRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<AdminRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        private AdminPermissionManager _permissionManager;
        public AdminPermissionManager PermissionManager
        {
            get
            {
                return _permissionManager ?? HttpContext.GetOwinContext().Get<AdminPermissionManager>();
            }
            private set
            {
                _permissionManager = value;
            }
        }
        #endregion
        // GET: SiteSettings
        public ActionResult Index()
        {
            return View();
        }

        //[CheckPermission("X101")]
        public ActionResult RoleManage()
        {
            bool addRole = false;
            bool updateRole = false;
            if (CheckRights("TJJS100") || true)
                addRole = true;
            if (CheckRights("XGJS110") || true)
                updateRole = true;
            ViewBag.AddRole = addRole;
            ViewBag.UpdateRole = updateRole;

            return View();
        }
        public JsonResult GetSystemRoles()
        {
            LayuiGridResult ret = new LayuiGridResult();
            if (!CheckRights("VIEWROLE"))
            {
                ret.code = 1;
                ret.msg = "您没有查看角色的权限";
                return Json(ret, JsonRequestBehavior.AllowGet);
            }

            var pageIndex = string.IsNullOrEmpty(Request["page"]) ? 1 : int.Parse(Request["page"]);
            var pageSize = string.IsNullOrEmpty(Request["limit"]) ? 10 : int.Parse(Request["limit"]);
            //RoleInfo_QueryCollection list = GlobalCache.ExternalClient.GetSystemRoleCollection(CurrentUser.UserToken);
            var pers = PermissionManager.GetAllPermissions();
            var secPers = pers.Where(p => p.ParentId == "0");
            int count = 0;
            int index = (pageIndex - 1) * pageSize + 1;
            List<RoleInfo_Query> list = RoleManager.GetAdminRolesByPagination(pageIndex, pageSize, out count)
                .Select(p =>
                {
                    var curPers = p.Permissions.Select(r => r.PermissionId).ToList();
                    var per = secPers.Where(n => curPers.Contains(n.Id)).Select(a =>
                    {
                        return new RoleFunctionInfo()
                        {
                            DisplayName = a.Name,
                            ParentId = a.ParentId,
                            ParentPath = a.ParentPath,
                            FunctionId = a.Id
                        };
                    }).ToList();

                    return new RoleInfo_Query()
                    {
                        RoleId = p.Id,
                        FunctionList = per,
                        IsAdmin = IdentityManager.RoleType.SuperAdmin == p.Type,
                        RoleName = p.Name,
                        RowId = index++
                    };
                }).ToList();
            //GlobalCache.GameClient.GetSystemRoleInfoList(CurrentUser.UserToken, pageIndex, pageSize);
            ret.code = 0;
            ret.count = count;
            ret.data = list;
            return Json(ret, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取角色信息（包括权限信息）
        /// </summary>
        /// <returns></returns>
        public ActionResult SetRoleFunction()
        {
            try
            {
                var edit = !string.IsNullOrWhiteSpace(Request["roleId"]);
                if (edit)
                {
                    if (!CheckRights("UPDATEROLE"))
                    {
                        HttpUnauthorizedResult result = new HttpUnauthorizedResult();
                        return result;
                    }
                }
                else
                {
                    if (!CheckRights("TJJS100"))
                    {
                        HttpUnauthorizedResult result = new HttpUnauthorizedResult();
                        return result;
                    }
                }

                var roleModel = new RoleModel();
                List<AdminPermissionModel> functionList = PermissionManager.GetAllPermissions().Select(p => new AdminPermissionModel()
                {
                    ParentId = p.ParentId,
                    ParentPath = p.ParentPath,
                    Name = p.Name,
                    Id = p.Id,
                    IsEnable = p.IsEnable
                }).ToList();  //GlobalCache.ExternalClient.QueryLowerLevelFuncitonList();

                if (functionList != null)
                {
                    functionList = functionList.Where(f => excludeFuncIds.Contains(f.Id) == false)
                        .ToList();
                }
                ViewBag.AllFunctionList = functionList;
                List<PermissionModel> permissions = new List<PermissionModel>();
                var firstPermissions = functionList.Where(p => p.ParentId == "0"); //获取一级权限列表
                if (edit)
                {
                    roleModel = RoleManager.GetAdminRoleByRoleId(Request["roleId"]).Select(p =>
                    {
                        return new RoleModel
                        {
                            RoleId = p.Id,
                            RoleName = p.Name,
                            RoleType = p.Type,
                            Permissions = p.Permissions?.Select(per => per.PermissionId).ToList()
                        };
                    }).FirstOrDefault();
                    ViewBag.RoleModel = roleModel;
                    // D101 代理管理  M101 名家管理  S101用户服务
                    var query = from f in functionList
                                where f.ParentId != "0" && roleModel.Permissions.Contains(f.Id)
                                select new AdminPermissionModel
                                {
                                    Id = f.Id == null ? string.Empty : f.Id,
                                    Name = f.Name == null ? string.Empty : f.Name,
                                    ParentId = f.ParentId == null ? string.Empty : f.ParentId,
                                    ParentPath = f.ParentPath == null ? string.Empty : f.ParentPath,
                                };//查询当前角色下的所有列表权限点

                    foreach (var item in firstPermissions)
                    {
                        var per = CreatePermissionModel(item, query.ToList(), roleModel.RoleType);
                        permissions.Add(per);
                        GetPermissionModels(per, item, query.ToList(), functionList, roleModel.RoleType);
                    }
                }
                else
                {
                    foreach (var item in firstPermissions)
                    {
                        var per = CreatePermissionModel(item, null, roleModel.RoleType);
                        permissions.Add(per);
                        GetPermissionModels(per, item, null, functionList, roleModel.RoleType);
                    }
                }
                ViewBag.Permissions = JsonConvert.SerializeObject(permissions);
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        ///  添加角色和对应的权限 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddRoleFunction(OperationRoleModel model)
        {
            CommonResult r = new CommonResult();
            try
            {
                if (!CheckRights("TJJS100"))
                {
                    return Json(new { IsSuccess = false, Msg = "您没有添加角色的权限" });
                }
                var id = Guid.NewGuid().ToString();
                AdminRole roleInfo = new AdminRole();
                roleInfo.Name = PreconditionAssert.IsNotEmptyString(model.RoleName, "角色名不能为空");
                roleInfo.Id = id;
                roleInfo.Type = model.RoleType;
                roleInfo.UpdateDate = DateTime.Now;
                roleInfo.CreationDate = DateTime.Now;
                roleInfo.CreatorUserId = CurUser.UserId;
                roleInfo.UpdatorUserId = CurUser.UserId;

                if (roleInfo.Type == IdentityManager.RoleType.SuperAdmin)
                {
                    roleInfo.Permissions = PermissionManager.GetAllPermissions()?.Select(p =>
                    {
                        return new AdminRolePermission()
                        {
                            RoleId = id,
                            PermissionId = p.Id,
                            CreationDate = DateTime.Now
                        };
                    }).ToList();
                }
                else
                {
                    roleInfo.Permissions = model.FunctionIds.Select(p =>
                    {
                        return new AdminRolePermission()
                        {
                            RoleId = id,
                            PermissionId = p,
                            CreationDate = DateTime.Now
                        };
                    }).ToList();

                }
                RoleManager.AddRole(roleInfo);
                return Json(r);
            }
            catch (Exception ex)
            {
                logger.Error("AddRoleFunciton", ex);
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }
        /// <summary>
        /// 更新角色权限信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UpdateRoleFunciton(OperationRoleModel model)
        {
            CommonResult r = new CommonResult();
            try
            {
                if (!CheckRights("UPDATEROLE"))
                {
                    r.code = (int)ResultCodeEnum.NoAuth;
                    r.message = "您没有修改角色的权限";
                    return Json(r);
                }
                var roleInfo = new AdminRole();
                roleInfo.Name = PreconditionAssert.IsNotEmptyString(model.RoleName, "角色名不能为空");
                roleInfo.Id = PreconditionAssert.IsNotEmptyString(model.RoleId, "角色编号不能为空");
                var roleById = RoleManager.FindById(model.RoleId);
                if (roleById == null)
                {
                    r.code = (int)ResultCodeEnum.NotFound;
                    r.message = "指定的角色不存在";
                    return Json(r);
                }
                var roleCount = RoleManager.GetRoleCountByRoleName(model.RoleName);
                if (roleCount >= 2)
                {
                    r.code = (int)ResultCodeEnum.NotFound;
                    r.message = "指定的角色已经存在";
                    return Json(r);
                }
                if (IdentityManager.RoleType.SuperAdmin == model.RoleType)
                {
                    var res = RoleManager.UpdateRole(roleInfo);// GlobalCache.ExternalClient.UpdateSystemRole(roleInfo, CurrentUser.UserToken);
                    if (res > 0)
                    {
                        r.code = (int)ResultCodeEnum.OK;
                        r.message = "更新成功";
                    }
                    else
                    {
                        r.code = (int)ResultCodeEnum.SystemError;
                        r.message = "更新失败";
                    }
                    return Json(r);
                }
                var funIds = (List<string>)(PreconditionAssert.IsNotNull(model.FunctionIds, "功能编号集异常"));

                var pers = funIds.Select(p =>
                {
                    return new AdminRolePermission()
                    {
                        RoleId = model.RoleId,
                        PermissionId = p,
                        CreationDate = DateTime.Now
                    };
                }).ToList();

                AdminRolePermissionManager m = new AdminRolePermissionManager();
                m.DeleteAndAddRolePermissions(model.RoleId, pers);
                AdminOperLogManager operLog = new AdminOperLogManager();
                operLog.AddOperationLog(new AdminOperationLog()
                {
                    CreationDate = DateTime.Now,
                    Description = "操作员【" + User.Identity.Name + "】修改角色，角色名称:" + model.RoleName + "，角色编号:" + model.RoleId,
                    OperationName = "角色管理",
                    OperUserId = CurUser.UserId,
                    UserId = ""
                });

                RoleManager.UpdateRole(roleInfo);
                return Json(r);
            }
            catch (Exception ex)
            {
                logger.Error("UpdateRoleFunciton", ex);
                return Json(new { IsSuccess = false, Msg = ex.Message });
            }
        }
        private void GetPermissionModels(PermissionModel permission, AdminPermissionModel item, List<AdminPermissionModel> list, List<AdminPermissionModel> all, IdentityManager.RoleType roleType)
        {
            var subList = all.Where(p => p.ParentId == item.Id);
            if (subList.Count() > 0)
            {
                foreach (var sub in subList)
                {
                    var per = CreatePermissionModel(sub, list, roleType);
                    permission.Data.Add(per);
                    if (all.Where(p => p.ParentId == sub.Id).Count() > 0)
                    {
                        GetPermissionModels(per, sub, list, all, roleType);
                    }
                }
            }
        }

        private PermissionModel CreatePermissionModel(AdminPermissionModel item, List<AdminPermissionModel> list, IdentityManager.RoleType roleType)
        {
            bool check = false;
            bool disabled = false;
            bool visible = true;
            if (list != null)
            {
                var t = list.FirstOrDefault(p => p.Id == item.Id);
                if (t != null || roleType == IdentityManager.RoleType.SuperAdmin)
                {
                    check = true;
                }
            }
            // Q101是后台管理人员必须要拥有的权限，所以添加一个后台管理人员，那么这个权限必须要有
            if (item.Id == "Q101")
            {
                check = true;
                disabled = true;
                visible = false;
            }
            var per = new PermissionModel()
            {
                Title = item.Name,
                Value = item.Id,
                Checked = check,
                Disabled = disabled,
                Visible = visible,
                Data = new List<PermissionModel>()
            };
            return per;
        }

        // [CheckPermission("X102")]
        public ActionResult SysOperatorManage()
        {
            bool addUser = false;
            bool updateUser = false;
            if (CheckRights("TJRY100"))
                addUser = true;
            if (CheckRights("XGRY110"))
                updateUser = true;
            ViewBag.AddUser = addUser;
            ViewBag.UpdateUser = updateUser;
            var pagination = new PaginationModel()
            {
                PageIndex = string.IsNullOrWhiteSpace(Request["pageIndex"]) ? base.PageIndex : Convert.ToInt32(Request["pageIndex"]),
                PageSize = string.IsNullOrWhiteSpace(Request["pageSize"]) ? base.PageSize : Convert.ToInt32(Request["pageSize"])
            };

            ViewBag.RoleListInfo = GetRoleModels();
            return View();
        }

        [HttpPost]
        public JsonResult DeleteSystemRole(string roleId)
        {
            CommonResult r = new CommonResult();
            try
            {
                if (!string.IsNullOrWhiteSpace(roleId))
                {
                    var result = RoleManager.DeleteRole(roleId);
                    if (result > 0)
                    {
                        r.code = (int)ResultCodeEnum.OK;
                        r.message = "成功删除角色";
                    }
                    else
                    {
                        r.code = (int)ResultCodeEnum.SystemError;
                        r.message = "角色删除失败";
                    }
                }
                else
                {
                    r.code = (int)ResultCodeEnum.VerifyError;
                    r.message = "userid不可以为空";
                }
            }
            catch (Exception ex)
            {
                r.code = (int)ResultCodeEnum.SystemError;
                r.message = ex.Message;
            }
            return Json(r);
        }

        public JsonResult GetSystemUsers()
        {
            var pageIndex = string.IsNullOrEmpty(Request["page"]) ? 1 : int.Parse(Request["page"]);
            var pageSize = string.IsNullOrEmpty(Request["limit"]) ? 10 : int.Parse(Request["limit"]);

            UserSearchAdditionDto model = new UserSearchAdditionDto()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                LoginIp = string.IsNullOrWhiteSpace(Request["LoginIp"]) ? null : Request["LoginIp"],
                Mobile = string.IsNullOrWhiteSpace(Request["Mobile"]) ? null : Request["Mobile"],
                RoleId = string.IsNullOrWhiteSpace(Request["RoleId"]) ? null : Request["RoleId"],
                RealName = string.IsNullOrWhiteSpace(Request["RealName"]) ? null : Request["RealName"]
            };
            bool enabled = false;
            if (!bool.TryParse(Request["IsEnable"], out enabled))
            {
                model.IsEnable = null;
            }
            else
            {
                model.IsEnable = enabled;
            }
            DateTime loginTime = DateTime.Now;
            if (!DateTime.TryParse(Request["LoginTime"], out loginTime))
            {
                model.LoginTime = null;
            }
            else
            {
                model.LoginTime = loginTime;
            }
            var infos = UserManager.GetAdminUsers(model);
            LayuiGridResult ret = new LayuiGridResult();
            ret.code = 0;
            ret.count = infos.TotalCount;
            if (infos.Data != null && infos.Data.Count > 0)
            {
                int id = (model.PageIndex - 1) * model.PageSize + 1;
                ret.data = infos.Data.Select(p =>
                {
                    return new
                    {
                        UserId = p.UserId,
                        RoleId = p.RoleId,
                        RoleName = p.RoleName,
                        RowId = id++,
                        LoginName = p.LoginName,
                        Mobile = p.Mobile,
                        RealName = p.RealName,
                        LoginIp = p.LoginIp,
                        IsEnable = p.IsEnable,
                        IpDisplayName = p.IpDisplayName,
                        LoginTime = p.LoginTime?.ToString("yyyy-MM-dd")
                    };
                });
            }

            return Json(ret, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult AddSystemUser(string userId)
        {
            ViewBag.RoleListInfo = RoleManager.GetAllRoles().Select(p =>
            {
                return new RoleModel
                {
                    RoleId = p.Id,
                    RoleName = p.Name
                };
            }).ToList();
            if (!string.IsNullOrWhiteSpace(userId))
            {
                var user = UserManager.GetAdminUserInfo(userId);
                AdminUserModel userInfo = new AdminUserModel()
                {
                    UserId = user.Id,
                    Name = user.Name,
                    IsEnable = user.IsEnable,
                    Mobile = user.Mobile,
                    Password = user.Password,
                    RoleId = user.RoleIds.FirstOrDefault()
                };
                ViewBag.SystemUserInfo = userInfo;
            }
            return View();
        }

        [HttpPost]
        public JsonResult AddSystemUser(SystemUserModel model)
        {
            JsonResult json = new JsonResult();
            try
            {
                var result = UserManager.FindByNameAsync(model.Mobile).Result;
                bool exist = result != null;
                if (exist)
                {
                    json.Data = new { status = 0, message = "该手机号码已经存在" };
                    return json;
                }

                if (ModelState.IsValid)
                {
                    var id = Guid.NewGuid().ToString();
                    AdminUser regInfo = new AdminUser()
                    {
                        AccessFailedCount = 0,
                        CreationDate = DateTime.Now,
                        CreatorUserId = CurUser.UserId,
                        PhoneNumber = model.Mobile,
                        Type = model.RoleType,
                        UpdateDate = DateTime.Now,
                        UpdatorUserId = CurUser.UserId,
                        UserName = model.Mobile,
                        PasswordHash = new PasswordHasher().HashPassword(model.Password),
                        Name = model.RealName,
                        IsEnable = true,
                        Email = "luffy@admin.com",
                        Id = id,
                        SecurityStamp=Guid.NewGuid().ToString(),
                        LockoutEnabled = false,
                        PhoneNumberConfirmed = true
                    };
                    var count = UserManager.AddUserAndUserRole(regInfo, model.RoleId);
                    if (count > 0)
                    {
                        json.Data = new { status = 1, message = "成功添加用户" };
                    }
                    else
                    {
                        json.Data = new { status = 2, message = "添加用户失败" };
                    }
                }
                else
                {
                    json.Data = new { status = 0, message = "字段为按要求填写" };
                }
            }
            catch (Exception ex)
            {
                logger.Error("AddSystemUser", ex);
                json.Data = new { status = 0, message = "服务端出错" };
            }
            return json;
        }
        [HttpPost]
        public JsonResult UpdateSystemUser(SystemUserModel model)
        {
            CommonResult r = new CommonResult();
            try
            {
                if (ModelState.IsValid)
                {
                    AdminUserDto userInfo = new AdminUserDto()
                    {
                        IsEnable = model.Enabled,
                        Mobile = model.Mobile,
                        Password = new PasswordHasher().HashPassword(model.Password),
                        Name = model.RealName,
                        RoleIds = new List<string>() { model.RoleId },
                        Id = model.UserId
                    };
                    int acount = UserManager.UpdateUserAndUserRole(userInfo);
                    if (acount > 0)
                    {
                        r.message = "保存成功";
                        r.code = (int)ResultCodeEnum.OK;
                    }
                    else
                    {
                        r.message = "保存失败";
                        r.code = (int)ResultCodeEnum.SystemError;
                    }
                }
                else
                {
                    r.message = "字段为按要求填写";
                    r.code = (int)ResultCodeEnum.VerifyError;
                }
            }
            catch (Exception ex)
            {
                r.message = ex.Message;
                r.code = (int)ResultCodeEnum.SystemError;
            }
            return Json(r);
        }
        [HttpPost]
        public JsonResult EnableSystemUser(string userid, bool enable)
        {
            CommonResult r = new CommonResult();
            try
            {
                if (!string.IsNullOrWhiteSpace(userid))
                {
                    var user = UserManager.FindById(userid);
                    if (user == null)
                    {
                        r.code = (int)ResultCodeEnum.NotFound;
                        r.message = "用户不存在";
                    }
                    else
                    {
                        if (enable)
                        {
                            UserManager.EnableUser(userid, false);
                        }
                        else
                        {
                            UserManager.EnableUser(userid, true);
                        }
                    }
                }
                else
                {
                    r.code = (int)ResultCodeEnum.VerifyError;
                    r.message = "userid不可以为空";
                }
            }
            catch (Exception ex)
            {
                r.code = (int)ResultCodeEnum.SystemError;
                r.message = "服务端出错";
            }
            return Json(r);
        }
        [HttpPost]
        public JsonResult DeleteSystemUser(string userid)
        {
            CommonResult r = new CommonResult();
            try
            {
                if (!string.IsNullOrWhiteSpace(userid))
                {
                    var user = UserManager.FindById(userid);
                    if (user == null)
                    {
                        r.code = (int)ResultCodeEnum.NotFound;
                        r.message = "用户不存在";
                    }
                    else
                    {
                        var count = UserManager.DeleteUser(userid);
                        if (count <= 0)
                        {
                            r.code = (int)ResultCodeEnum.NotFound;
                            r.message = "删除失败";
                        }

                    }
                }
                else
                {
                    r.code = (int)ResultCodeEnum.VerifyError;
                    r.message = "userid不可以为空";
                }
            }
            catch (Exception ex)
            {
                r.code = (int)ResultCodeEnum.SystemError;
                r.message = "系统出错";
            }
            return Json(r);
        }

        private List<RoleModel> GetRoleModels()
        {
            return RoleManager.GetAllRoles()?.Select(p =>
            {
                return new RoleModel()
                {
                    RoleId = p.Id,
                    RoleName = p.Name
                };
            }).ToList();
        }

    }
}