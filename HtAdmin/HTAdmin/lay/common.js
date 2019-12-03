
// 获取彩种名称
function GetGameCodeName(strGameCode)
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
		default: return '';
	}
}

function GameName(gamecode, type = "")
{
    if (gamecode == null) {
        return "";
    }
    type = type == null ? gamecode : type;
    //根据彩种编号获取彩种名称
    switch (gamecode.toLowerCase()) {
        case "cqssc": return "重庆时时彩";
        case "sd11x5": return "山东11选5";
        case "gd11x5": return "广东11选5";
        case "jx11x5": return "江西11选5";
        case "pl3": return "排列三";
        case "fc3d": return "福彩3D";
        case "ssq": return "双色球";
        case "qxc": return "七星彩";
        case "qlc": return "七乐彩";
        case "dlt": return "大乐透";
        case "sdqyh": return "群英会";
        case "gdklsf": return "广东快乐十分";
        case "gxklsf": return "广西快乐十分";
        case "jsks": return "江苏快3";
        case "sdklpk3": return "山东快乐扑克3";
        case "ozb": return "欧洲杯";
        case "sjb": return "世界杯";
        case "bjpk10": return "北京PK10";
        case "jczq":
            switch (type.toLowerCase()) {
                case "spf": return "竞彩让球胜平负";
                case "brqspf": return "竞彩胜平负";
                case "bf": return "竞彩比分";
                case "zjq": return "竞彩总进球数";
                case "bqc": return "竞彩半全场";
                case "hh": return "足球混合过关";
                default: return "竞彩足球";
            }
        case "jclq":
            switch (type.toLowerCase()) {
                case "sf": return "篮球胜负";
                case "rfsf": return "篮球让分胜负";
                case "sfc": return "篮球胜分差";
                case "dxf": return "篮球大小分";
                case "hh": return "篮球混合过关";
                default: return "竞彩篮球";
            }
        case "ctzq":
            switch (type.toLowerCase()) {
                case "t14c": return "14场胜负";
                case "tr9": return "任9场";
                case "t6bqc": return "6场半全";
                case "t4cjq": return "4场进球";
                default: return "传统足球";
            }
        case "bjdc":
            switch (type.toLowerCase()) {
                case "sxds": return "单场上下单双";
                case "spf": return "单场胜平负";
                case "zjq": return "单场总进球";
                case "bf": return "单场比分";
                case "bqc": return "单场半全场";
                default: return "北京单场";
            }
        default: return gamecode;
    }
}

// 购彩方式
function GetBuyTypeName(buyType)
{
    switch (buyType) {
        case 1:
            return "自购";
        case 2:
            return "追号";
        case 3:
        default:
            return "自购";
    }
}

//分析url
function parseURL(url) {
    var a = document.createElement('a');
    a.href = url;
    return {
        source: url,
        protocol: a.protocol.replace(':', ''),
        host: a.hostname,
        port: a.port,
        query: a.search,
        params: (function () {
            var ret = {},
                seg = a.search.replace(/^\?/, '').split('&'),
                len = seg.length, i = 0, s;
            for (; i < len; i++) {
                if (!seg[i]) { continue; }
                s = seg[i].split('=');
                ret[s[0]] = s[1];
            }
            return ret;

        })(),
        file: (a.pathname.match(/\/([^\/?#]+)$/i) || [, ''])[1],
        hash: a.hash.replace('#', ''),
        path: a.pathname.replace(/^([^\/])/, '/$1'),
        relative: (a.href.match(/tps?:\/\/[^\/]+(.+)/) || [, ''])[1],
        segments: a.pathname.replace(/^\//, '').split('/')
    };
}

//替换myUrl中的同名参数值 
function replaceUrlParams(myUrl, newParams) {
    if (newParams.length == 0) {
        myUrl.params = {};
    }
    else {
        for (var x in newParams) {
            var hasInMyUrlParams = false;
            for (var y in myUrl.params) {
                if (x.toLowerCase() == y.toLowerCase()) {
                    myUrl.params[y] = newParams[x];
                    hasInMyUrlParams = true;
                    break;
                }
            }
            //原来没有的参数则追加 
            if (!hasInMyUrlParams) {
                myUrl.params[x] = newParams[x];
            }
        }
    }
    var _result = myUrl.protocol + "://" + myUrl.host + ":" + myUrl.port + myUrl.path + "?";

    for (var p in myUrl.params) {
        _result += (p + "=" + myUrl.params[p] + "&");
    }

    if (_result.substr(_result.length - 1) == "&") {
        _result = _result.substr(0, _result.length - 1);
    }

    if (myUrl.hash != "") {
        _result += "#" + myUrl.hash;
    }
    return _result;
}

// {key: value, key2, value}
function publicPagingList(obj) {
    //获取url
    var url = location.href;
    //替换或追加url
    var myURL = parseURL(url);
    var _newUrl = replaceUrlParams(myURL, obj);
    //window.location.href = encodeURIComponent(_newUrl);
    window.location.href = _newUrl;
}

function PageIndexChange(obj, max) {
    var val = obj.value;
    if (val > max)
        obj.value = max;
    else if (val < 1)
        obj.value = 1;
}
$("#btn_ToPage").click(function () {
    var pageIndex = $("#pageIndex").val();
    publicPagingList({ pageIndex: pageIndex });
});

$("#pageSizeList").change(function () {
    var pageSize = $(this).val()
    publicPagingList({ pageSize: pageSize, pageIndex: 1 });
});

//公用的iframe弹窗
function LayuiOpen(title, url, callback1, callback2) {
    layer.open({
        type: 2,// 表示是iframe弹框
        title: title,
        content: url,
        area: ['800px', '700px'],
        end: function () {
            callback1();
        },
        cancel: function () {
            callback2();
        }
    });
}

//公用关闭iframe
function LayuiCancel() {
    console.log(1);
    var index = parent.layer.getFrameIndex(window.name);
    parent.layer.close(index);//关闭当前页
}

