using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HTAdmin.Models
{
    [Serializable]
    public class ResultData
    {
        /// <summary>
        /// 返回值 - key value
        /// </summary>
        public string Data { get; set; }
    }

    [Serializable]
    public class PayResultData : ResultData
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderId { get; set; }
    }

    [Serializable]
    public class CommonResult
    {
        public CommonResult()
        {
            code = (int)ResultCodeEnum.OK;
            message = "成功";
            data = "";
        }
        /// <summary>
        /// 接口返回码
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 消息说明
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// 返回数据
        /// </summary>
        public object data { get; set; }
    }
    /// <summary>
    /// 状态码类型
    /// </summary>
    public enum ResultCodeEnum : int
    {
        /// <summary>
        /// 返回正确
        /// </summary>
        OK = 200,
        /// <summary>
        /// 接口未授权（未登录）
        /// </summary>
        NoAuth = 401,
        /// <summary>
        /// 接口报错
        /// </summary>
        APIError = 501,
        /// <summary>
        /// 参数检测失败，请求参数有误
        /// </summary>
        VerifyError = 400,
        /// <summary>
        /// 接口提示信息
        /// </summary>
        SystemInfo = 200,
        /// <summary>
        /// 未找到接口
        /// </summary>
        NotFound = 404,
        /// <summary>
        /// 系统内部错误
        /// </summary>
        SystemError = 500,
        /// <summary>
        /// 需要进行极验验证
        /// </summary>
        NeedGeetest = 4001,
        /// <summary>
        /// 极验验证错误
        /// </summary>
        GeetestWrong = 4002,
        /// <summary>
        /// 账户被锁
        /// </summary>
        UserLockout = 600,
        /// <summary>
        /// 密码错误
        /// </summary>
        PasswordError = 601,

    }
}