using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Configuration;
using Common.Lottery;
using Common.Lottery.OpenDataGetters;
using Common.XmlAnalyzer;


/// <summary>
/// 站点字符串、地址类
/// </summary>
public static class SiteString
{
    #region 彩种相关信息
    /// <summary>
    /// 彩种分类字符串数组
    /// </summary>
    public static string[] gameList(string type)
    {
        switch (type.ToLower())
        {
            case "gpc":
                return new string[] { "cqssc", "jxssc", "sd11x5", "gd11x5", "jx11x5" };
            case "jjc":
                return new string[] { "jczq", "jclq", "bjdc", "ctzq" };
            case "szc":
                return new string[] { "ssq", "dlt", "fc3d", "pl3" };
            case "dpc":
                return new string[] { "ssq", "dlt" };
            case "day":
                return new string[] { "fc3d", "pl3" };
            default:
                return new string[] { };
        }
    }

    /// <summary>
    /// 获取彩种开奖日期
    /// </summary>
    /// <param name="gameCode"></param>
    /// <returns></returns>
    public static string lotteryDay(string gameCode)
    {
        var ret = "不定期";
        switch (gameCode.ToLower())
        {
            case "cqssc":
            case "jsks":
            case "jxssc":
            case "gdklsf":
                ret = "10分钟一期";
                break;
            case "sd11x5":
            case "gd11x5":
            case "jx11x5":
                ret = "12分钟一期";
                break;
            case "sdqyh":
                ret = "15分钟一期";
                break;
            case "fc3d":
            case "pl3":
                ret = "每日开奖";
                break;
            case "ssq":
                ret = "二 四 日";
                break;
            case "dlt":
                ret = "一 三 六";
                break;
        }
        return ret;
    }

    #endregion

    #region 字符串加密
    /// <summary>
    /// 加密字符串，隐藏指定位置用替换符代替
    /// </summary>
    /// <param name="objString">目标字符串</param>
    /// <param name="startIndex">起始隐藏位置</param>
    /// <param name="endIndex">结束隐藏位置</param>
    /// <param name="perch">替换符位数，默认3位</param>
    /// <param name="perchString">替换符号，默认*</param>
    /// <returns></returns>
    public static string EncodeString(string objString, int startIndex = 0, int endIndex = 0, int perch = 3, string perchString = "*")
    {
        if (endIndex > 0 && endIndex > startIndex && endIndex <= objString.Length)
        {
            var perchtmp = "";
            for (int i = 0; i < perch; i++)
            {
                perchtmp += perchString;
            }
            return objString.Substring(0, startIndex) + perchtmp + objString.Substring(endIndex);
        }
        else
        {
            return objString;
        }
    }

    /// <summary>
    /// 加密手机号，只显示前三位和后四位
    /// </summary>
    /// <param name="mobile">手机号码</param>
    /// <returns>加密后的手机号码</returns>
    public static string EncodeMobile(string mobile)
    {
        return EncodeString(mobile, 3, mobile.Length - 4);
    }

    /// <summary>
    /// 加密身份证号码，只显示前五位和后四位
    /// </summary>
    /// <param name="mobile">身份证号码</param>
    /// <returns>加密后的身份证号码</returns>
    public static string EncodeIdCardNumber(string idCard)
    {
        return EncodeString(idCard, 5, idCard.Length - 4);
    }

    /// <summary>
    /// 加密银行卡号，隐藏最后6位
    /// </summary>
    /// <param name="mobile">银行卡号</param>
    /// <returns>加密后的银行卡号</returns>
    public static string EncodeBankCardNumber(string bankCard)
    {
        if (string.IsNullOrEmpty(bankCard)) return string.Empty;
        return EncodeString(bankCard, bankCard.Length - 6, bankCard.Length - 1);
    }

    /// <summary>
    /// 加密邮箱地址，只显示邮箱来源及一部分自定义地址
    /// </summary>
    /// <param name="mobile">邮箱地址</param>
    /// <returns>加密后的邮箱地址</returns>
    public static string EncodeEmail(string email)
    {
        var emailArray = email.Split('@');
        if (emailArray.Length > 1)
        {
            return EncodeString(emailArray[0], 3, emailArray[0].Length) + "@" + emailArray[1];
        }
        else
        {
            return email;
        }
    }

    /// <summary>
    /// 隐藏用户名
    /// </summary>
    /// <param name="userName">用户名</param>
    /// <param name="showCount">显示长度</param>
    /// <returns>已隐藏部分用户名</returns>
    public static string HideUserName(string userName, int hideCount = 2)
    {
        if (string.IsNullOrEmpty(userName))
        {
            return "";
        }
        hideCount = hideCount > userName.Length ? userName.Length : hideCount;
        return EncodeString(userName, userName.Length - hideCount, userName.Length, hideCount);
    }
    /// <summary>
    /// 隐藏用户名-全站通用
    /// </summary>
    /// <param name="userName">用户名</param>
    /// <param name="showCount">显示长度</param>
    /// <returns>已隐藏部分用户名</returns>
    public static string AllHideUser(string userName, int showCuont = 2)
    {
        try
        {
            return EncodeString(userName, showCuont, userName.Length);
        }
        catch
        {
            return userName;
        }

    }
    #endregion

    #region 竞彩编码
    /// <summary>
    /// 北单投注编码及显示码
    /// </summary>
    /// <param name="type">玩法类型</param>
    /// <param name="isCode">是否投注编码</param>
    /// <returns>返回投注编码或显示码</returns>
    public static string ANTECODES_BJDC(string type, bool isCode = false)
    {
        switch (type)
        {
            case "spf":
                return isCode ? "3,1,0" : "胜,平,负";
            case "zjq":
                return isCode ? "0,1,2,3,4,5,6,7" : "0,1,2,3,4,5,6,7+";
            case "sxds":
                return isCode ? "SD,SS,XD,XS" : "上单,上双,下单,下双";
            case "bf":
                return isCode ? "00,01,02,03,10,11,12,13,20,21,22,23,30,31,32,33,40,41,42,04,14,24,X0,XX,0X" : "0:0,0:1,0:2,0:3,1:0,1:1,1:2,1:3,2:0,2:1,2:2,2:3,3:0,3:1,3:2,3:3,4:0,4:1,4:2,0:4,1:4,2:4,胜其他,平其他,负其他";
            case "bqc":
                return isCode ? "33,31,30,13,11,10,03,01,00" : "胜胜,胜平,胜负,平胜,平平,平负,负胜,负平,负负";
            default:
                return isCode ? "3,1,0" : "胜,平,负";
        }
    }

    /// <summary>
    /// 竞彩足球投注编码及显示码
    /// </summary>
    /// <param name="type">玩法类型</param>
    /// <param name="isCode">是否投注编码</param>
    /// <returns>返回投注编码或显示码</returns>
    public static string ANTECODES_JCZQ(string type, bool isCode = false)
    {
        switch (type)
        {
            case "spf":
                return isCode ? "3,1,0" : "胜,平,负";
            case "zjq":
                return isCode ? "0,1,2,3,4,5,6,7" : "0,1,2,3,4,5,6,7+";

            case "bf":
                return isCode ? "00,01,02,03,10,11,12,13,20,21,22,23,30,31,32,33,40,41,42,04,14,24,50,51,52,05,15,25,X0,XX,0X" : "0:0,0:1,0:2,0:3,1:0,1:1,1:2,1:3,2:0,2:1,2:2,2:3,3:0,3:1,3:2,3:3,4:0,4:1,4:2,0:4,1:4,2:4,5:0,5:1,5:2,0:5,1:5,2:5,胜其他,平其他,负其他";
            case "bqc":
                return isCode ? "33,31,30,13,11,10,03,01,00" : "胜胜,胜平,胜负,平胜,平平,平负,负胜,负平,负负";
            default:
                return isCode ? "3,1,0" : "胜,平,负";
        }
    }

    /// <summary>
    /// 竞彩篮球投注编码及显示码
    /// </summary>
    /// <param name="type">玩法类型</param>
    /// <param name="isCode">是否投注编码</param>
    /// <returns>返回投注编码或显示码</returns>
    public static string ANTECODES_JCLQ(string type, bool isCode = false)
    {
        switch (type)
        {
            case "sf":
            case "rfsf":
                return isCode ? "3,0" : "主胜,客胜";
            case "sfc":
                return isCode ? "01,02,03,04,05,06,11,12,13,14,15,16" : "胜1-5分,胜6-10分,胜11-15分,胜16-20分,胜21-25分,胜26分以上,负1-5分,负6-10分,负11-15分,负16-20分,负21-25分,负26分以上";
            case "dxf":
                return isCode ? "3,0" : "大,小";
            default:
                return isCode ? "3,0" : "主胜,客胜";
        }
    }

    /// <summary>
    /// 传统足球投注编码及显示码
    /// </summary>
    /// <param name="type">玩法类型</param>
    /// <param name="isCode">是否投注编码</param>
    /// <returns>返回投注编码或显示码</returns>
    public static string ANTECODES_CTZQ(string type, bool isCode = false)
    {
        switch (type)
        {
            case "t6bqc":
            case "tr9":
            case "t14c":
                return isCode ? "3,1,0" : "胜,平,负";
            case "t4cjq":
                return isCode ? "0,1,2,3" : "0,1,2,3+";
            default:
                return isCode ? "3,1,0" : "胜,平,负";
        }
    }

    /// <summary>
    /// 根据投注编码显示编码名称
    /// </summary>
    /// <param name="type">玩法类型</param>
    /// <param name="value">投注编码</param>
    /// <returns>显示编码</returns>
    public static string ANTECODES_NAME(string type, string value)
    {
        try
        {
            return SportAnalyzer.GetMatchResultDisplayName(type, value);
        }
        catch
        {
            return value;
        }
    }
    #endregion

    //金额转换
    public static string RMB(decimal monery)
    {
        try
        {
            return monery.ToString("c");
        }
        catch (Exception)
        {
            return monery.ToString();
        }
    }
}