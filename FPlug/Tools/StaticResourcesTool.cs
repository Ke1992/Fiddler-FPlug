namespace FPlug.Tools
{
    public class StaticResourcesTool
    {
        //当前版本
        public static string version = "v1.0.4";

        //Header替换Coded的Key
        public static string responseCodeKey = "Response-Code";

        //工具key数组
        public static string[] keys = new string[] { "serverip", "cache", "vconsole", "console", "invade" };

        //Header替换Key数组
        public static string[] headerKeys = new string[] { "Custom", responseCodeKey, "Date", "Accept", "Cookie", "Referer", "Expires", "User-Agent", "Content-Type", "Cache-Control", "Accept-Encoding", "Accept-Language", "Content-Encoding", "Access-Control-Allow-Origin", "Access-Control-Allow-Credentials" };

        //查询版本URL
        public static string versionUrlStr = "https://raw.githubusercontent.com/Ke1992/FPlug/master/version.js";

        //查询版本请求信息
        public static string versionRequestStr = "GET https://raw.githubusercontent.com/Ke1992/FPlug/master/version.js HTTP/1.1\n" +
            "Host: raw.githubusercontent.com\n" +
            "Cache-Control: max-age=0\n" +
            "User-Agent: Fiddler\n" +
            "Accept-Language: zh-CN,zh;q=0.8\n\n";

        //脚本注入到html页面中，抓取页面的console输出，发出POST请求，Fiddler捕获以后打印
        public static string consoleScriptStr = "<!DOCTYPE HTML><script type='text/javascript'>" +
            "(function(){window.consoleTotalNum=0;window.console.logOld=window.console.log;window.console.log=function(){window.consoleTotalNum++;window.console.logOld.apply(this,arguments);sendLogToFiddler('log',arguments)};window.console.errorOld=window.console.error;window.console.error=function(){window.consoleTotalNum++;window.console.errorOld.apply(this,arguments);sendLogToFiddler('error',arguments)};window.console.warnOld=window.console.warn;window.console.warn=function(){window.consoleTotalNum++;window.console.warnOld.apply(this,arguments);sendLogToFiddler('warn',arguments)};function sendLogToFiddler(type,param){if(!param.length){return false}var data='',xhr=new XMLHttpRequest(),url=location.protocol+'//www.example.com',nowurl=location.protocol+'//'+location.host+location.pathname;for(var i=0,len=param.length;i<len;i++){if(param[i]&&Object.prototype.toString.call(param[i])=='[object Object]'){data+=JSON.stringify(param[i])+'   '}else if(param[i]){data+=param[i].toString()+'   '}}xhr.open('POST',url+'?serial='+window.consoleTotalNum+'&type='+type+'&nowurl='+nowurl,true);xhr.setRequestHeader('Content-type','application/x-www-form-urlencoded');xhr.send(data.trim())}}());" +
            "</script>";

        //JS注入--请求端代码，用来定时发起固定请求
        public static string invadeRequestStr = "<script type='text/javascript'>" +
            "(function(){var total=1;setInterval(function(){var FPlugScript=document.createElement('script');FPlugScript.id='FPlug_script_'+(total++);FPlugScript.src=location.protocol+'//www.example.com?_t='+new Date().getTime()+'&id='+FPlugScript.id;document.body&&document.body.appendChild(FPlugScript)},2000)})();" +
            "</script>";

        //JS注入--响应端代码，用来及时清除script标签，防止过多
        public static string invadeResponseStr = "document.body && document.body.removeChild(document.getElementById('{0}'));";

        //vConsole注入
        public static string vConsoleStr = "<script type='text/javascript' src='//cdnjs.cloudflare.com/ajax/libs/vConsole/3.3.0/vconsole.min.js'></script>" +
            "<script type='text/javascript'>" +
                "VConsole && new VConsole();" +
            "</script>";
    }
}
