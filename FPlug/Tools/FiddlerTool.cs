using Fiddler;
using FPlug.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace FPlug.Tools
{
    class FiddlerTool
    {
        #region Fiddler监听事件
        public static void handleRequest(Session session)
        {
            //没有启用插件
            if (!Main.mainData.getEnable())
            {
                return;
            }
            //console日志
            handleConsoleLogRequest(session);
            //HTTPS 转 HTTP
            handleHttpsToHttp(session);
            //HOST映射
            handleHostMapping(session);
            //UA模拟
            replaceUserAgent(session);
            //JS注入--Request端
            invadeJavaScriptRequest(session);
            //禁止缓存
            handleCacheRequest(session);
        }

        public static void handleResponse(Session session)
        {
            //更新版本号
            filterVersionInfo(session);
            //没有启用插件
            if (!Main.mainData.getEnable())
            {
                return;
            }
            //FILE映射
            handleFileMapping(session);
            //console日志
            handleConsoleLogResponse(session);
            //JS注入
            invadeJavaScriptResponse(session);
            //禁止缓存
            handleCacheResponse(session);
        }

        public static void handleWebSocket(object session, WebSocketMessageEventArgs msg)
        {
            //没有启用插件
            if (!Main.mainData.getEnable())
            {
                return;
            }
        }
        #endregion

        #region Request相关函数
        //HTTPS 转 HTTP
        private static void handleHttpsToHttp(Session session)
        {
            List<BaseModel> rules = getValidRulesByType("https");

            //如果没有有效的host配置，直接返回
            if (rules.Count == 0)
            {
                return;
            }

            //获取Url的Path
            string path = getPathFromSession(session.fullUrl);

            //遍历配置去修改映射值
            for (int i = 0; i < rules.Count; i++)
            {
                string url = (rules[i] as HttpsModel).Url.ToString();
                //新建正则表达式来检测
                Regex urlRegex = new Regex(url);
                //判断当前session的path是否在配置中
                if (path.IndexOf(url) >= 0 || urlRegex.IsMatch(path))
                {
                    //修改背景颜色、字体颜色
                    session["ui-color"] = "#FFFFFF";
                    session["ui-backcolor"] = "#FF9933";

                    if (session.HTTPMethodIs("CONNECT"))
                    {
                        session["x-replywithtunnel"] = "FakeTunnel";
                        return;
                    }
                    //只限制https的请求进行替换
                    if (session.isHTTPS)
                    {
                        session.fullUrl = session.fullUrl.Replace("https://", "http://");
                    }
                    break;
                }
            }
        }
        //HOST映射
        private static void handleHostMapping(Session session)
        {
            List<BaseModel> rules = getValidRulesByType("host");

            //如果没有有效的host配置，直接返回
            if (rules.Count == 0)
            {
                return;
            }

            //获取Url的Path
            string path = getPathFromSession(session.fullUrl);

            //遍历配置去修改映射值
            for (int i = 0; i < rules.Count; i++)
            {
                //获取对应的各种参数
                string url = (rules[i] as HostModel).Url.ToString();
                string ip = (rules[i] as HostModel).IP.ToString();
                string port = (rules[i] as HostModel).Port.ToString();
                //新建正则表达式来检测
                Regex urlRegex = new Regex(url);
                //判断当前session的path是否在配置中
                if (path.IndexOf(url) >= 0 || urlRegex.IsMatch(path))
                {
                    //修改背景颜色、字体颜色
                    session["ui-color"] = "#FFFFFF";
                    session["ui-backcolor"] = "#9966CC";

                    if (port.Length > 0)
                    {
                        session["x-overrideHost"] = ip + ":" + port;
                    }
                    else
                    {
                        //映射到对应的ip和端口(这里必须写上端口号，不然https下会有问题)
                        session["x-overrideHost"] = ip + ":" + session.port;
                    }
                    session.bypassGateway = true;
                    break;
                }
            }
        }
        //UA模拟
        private static void replaceUserAgent(Session session)
        {
            ToolModel userAgent = Main.mainData.getToolByType("useragent");

            if (userAgent.Enable && userAgent.Content.Length > 0)
            {
                session.RequestHeaders.Remove("User-Agent");
                session.RequestHeaders.Add("User-Agent", userAgent.Content);
            }
        }
        //console日志--Request端
        private static void handleConsoleLogRequest(Session session)
        {
            //开启工具 && UI容器不为空
            if (Main.mainData.getToolByType("console").Enable && Main.container != null)
            {
                string fullUrl = session.fullUrl;
                
                //如果是日志请求，则直接
                if (fullUrl.Contains("//www.example.com/?serial=") || fullUrl.Contains("//www.example.com?serial="))
                {
                    //直接隐藏session面板对应的对话
                    session["ui-hide"] = "stealth";

                    //获取对应的url
                    NameValueCollection queryString = HttpUtility.ParseQueryString(session.fullUrl);

                    //获取所有参数
                    string serial = queryString.Get(0);
                    string type = queryString.Get("type");
                    string nowurl = queryString.Get("nowurl");
                    string content = session.GetRequestBodyAsString();

                    //存储日志到数组
                    Main.mainData.addLog(nowurl, serial, type, content);
                    //不是当前过滤级别就直接返回
                    if (Container.tabModel.ConsoleType != "all" && Container.tabModel.ConsoleType != type)
                    {
                        return;
                    }
                    //输出日志到面板中去
                    Main.container.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        try
                        {
                            Main.container.addLogToPanel(nowurl, serial, type, content);
                        }
                        catch (Exception e)
                        {
                            FiddlerApplication.Log.LogString("FPlug出现错误(handleConsoleLogRequest函数)：" + e.ToString());
                        }
                    }));
                }
            }
        }
        //JS注入--Request端
        private static void invadeJavaScriptRequest(Session session)
        {
            //开启工具
            if (Main.mainData.getToolByType("invade").Enable)
            {
                string fullUrl = session.fullUrl;

                //JS注入发起的请求
                if (fullUrl.Contains("//www.example.com/?_t=") || fullUrl.Contains("//www.example.com?_t="))
                {
                    //直接隐藏session面板对应的对话
                    session["ui-hide"] = "stealth";
                }
            }
        }
        //禁止缓存--Request端
        private static void handleCacheRequest(Session session)
        {
            if (Main.mainData.getToolByType("cache").Enable)
            {
                //先删除
                session.RequestHeaders.Remove("Pragma");
                session.RequestHeaders.Remove("Cache-Control");
                session.RequestHeaders.Remove("Expires");
                session.RequestHeaders.Remove("If-None-Match");
                session.RequestHeaders.Remove("If-Modified-Since");
                //后增加
                session.RequestHeaders.Add("Pragma", "no-cache");
                session.RequestHeaders.Add("Cache-Control", "no-cache");
            }
        }
        #endregion

        #region Response相关函数
        //版本号查询
        private static void filterVersionInfo(Session session)
        {
            if (Container.tabModel.Version != "")
            {
                return;
            }

            if (session.fullUrl == StaticResourcesTool.versionUrlStr && session.responseCode.ToString() == "200")
            {
                Container.tabModel.Version = session.GetResponseBodyAsString();
            }
        }
        //FILE映射
        private static void handleFileMapping(Session session)
        {
            List<BaseModel> rules = getValidRulesByType("file");

            //如果没有有效的host配置，直接返回
            if (rules.Count == 0)
            {
                return;
            }

            //获取Url
            string fullUrl = session.fullUrl;

            //遍历配置去修改映射值
            for (int i = 0; i < rules.Count; i++)
            {
                string url = (rules[i] as FileModel).Url.ToString();
                string path = (rules[i] as FileModel).Path.ToString();

                //新建正则表达式来检测
                Regex urlRegex = new Regex(url);
                //判断当前session的url是否在配置中
                if (fullUrl.IndexOf(url) >= 0 || urlRegex.IsMatch(fullUrl))
                {
                    //修改背景颜色、字体颜色
                    session["ui-color"] = "#FFFFFF";
                    session["ui-backcolor"] = "#FF6666";

                    try
                    {
                        //根据本地路径读取文件内容
                        string newResBody = readDataFromFile(path);
                        //如果包含callback
                        if (newResBody.Contains("callback"))
                        {
                            //获取callback的名称
                            string cbname = "";
                            if (session.PathAndQuery.IndexOf("?") > 0)
                            {
                                string queryStr = session.PathAndQuery.Substring(session.PathAndQuery.IndexOf("?") + 1);
                                cbname = HttpUtility.ParseQueryString(queryStr).Get("callback");
                            }

                            //如果链接中带有callback字段
                            if (cbname != null && cbname.Length > 0)
                            {
                                //进行替换
                                StringBuilder stringBuilder = new StringBuilder(newResBody);
                                newResBody = stringBuilder.Replace("callback", cbname, newResBody.IndexOf("callback"), 8).ToString();
                                //设置新的返回体
                                session.utilSetResponseBody(newResBody);
                                //直接修改状态返回码为200
                                session.responseCode = 200;
                                //直接return
                                return;
                            }
                        }
                        //没有callback则直接使用api进行替换
                        session.LoadResponseFromFile(path);
                    }
                    catch (Exception e)
                    {
                        //直接修改状态返回码为404
                        session.responseCode = 404;
                        //设置新的返回体
                        session.utilSetResponseBody(e.ToString());
                        //输出错误
                        FiddlerApplication.Log.LogString("FPlug插件：FILE映射错误");
                        FiddlerApplication.Log.LogString(e.ToString());
                        //FiddlerApplication.DoNotifyUser(e.ToString(), "FILE映射错误");
                    }
                    //直接结束循环
                    break;
                }
            }
        }
        //console日志--Response端
        private static void handleConsoleLogResponse(Session session)
        {
            if (Main.mainData.getToolByType("console").Enable)
            {
                //是否是html格式
                if (judgeContentTypeIsHtml(session.ResponseHeaders["Content-Type"].ToString()))
                {
                    string responseBody = session.GetResponseBodyAsString();

                    //是否是document
                    if (judgeIsDocument(responseBody))
                    {
                        session.utilSetResponseBody(StaticResourcesTool.consoleScriptStr + responseBody);
                    }
                }
            }
        }
        //JS注入--Response端
        private static void invadeJavaScriptResponse(Session session)
        {
            //开启工具
            if (Main.mainData.getToolByType("invade").Enable)
            {
                string fullUrl = session.fullUrl;
                
                //JS注入发起的请求
                if (fullUrl.Contains("//www.example.com/?_t=") || fullUrl.Contains("//www.example.com?_t="))
                {
                    //直接隐藏session面板对应的对话
                    session["ui-hide"] = "stealth";

                    //获取对应的url
                    NameValueCollection queryString = HttpUtility.ParseQueryString(session.fullUrl);

                    //获取最新的脚本数据
                    string content = Main.mainData.getJavaScriptFormInvadeData();
                    //没有数据，直接
                    if (content == "")
                    {
                        content = string.Format(StaticResourcesTool.invadeResponseStr, queryString.Get("id"));
                    }

                    //填充内容进去
                    session.utilSetResponseBody(content);
                    
                    //直接修改状态返回码为200
                    session.responseCode = 200;
                }
                else
                {
                    //判断是否是html格式
                    if (judgeContentTypeIsHtml(session.ResponseHeaders["Content-Type"].ToString()))
                    {
                        string responseBody = session.GetResponseBodyAsString();

                        //是否是document
                        if (judgeIsDocument(responseBody))
                        {
                            //添加JS注入Request端脚本
                            session.utilSetResponseBody(responseBody + StaticResourcesTool.invadeRequestStr);
                        }
                    }
                }
            }
        }
        //禁止缓存--Response端
        private static void handleCacheResponse(Session session)
        {
            if (Main.mainData.getToolByType("cache").Enable)
            {
                //先删除
                session.ResponseHeaders.Remove("Pragma");
                session.ResponseHeaders.Remove("Cache-Control");
                session.ResponseHeaders.Remove("Expires");
                //后增加
                session.ResponseHeaders.Add("Pragma", "no-cache");
                session.ResponseHeaders.Add("Cache-Control", "no-cache");
            }
        }
        #endregion

        #region Fiddler相关操作函数
        //变更插件的Icon
        public static void changeFPlugIcon(bool enable)
        {
            if (enable)
            {
                Main.page.ImageIndex = FiddlerApplication.UI.tabsViews.ImageList.Images.IndexOfKey("FPlug_Icon");
            }
            else
            {
                Main.page.ImageIndex = FiddlerApplication.UI.tabsViews.ImageList.Images.IndexOfKey("FPlug_Icon_No");
            }
        }
        //显示隐藏ServerIP
        public static void showHideServerIP(bool show)
        {
            FiddlerApplication.UI.lvSessions.AddBoundColumn("ServerIP", 0, "X-HostIP");
            FiddlerApplication.UI.lvSessions.EnsureColumnIsVisible("ServerIP");

            if (show)
            {
                FiddlerApplication.UI.lvSessions.SetColumnOrderAndWidth("ServerIP", 1, 100);
            }
            else
            {
                FiddlerApplication.UI.lvSessions.SetColumnOrderAndWidth("ServerIP", 1, 0);
            }
        }
        #endregion

        #region 内部工具函数
        //根据类型获取有效的规则
        private static List<BaseModel> getValidRulesByType(string type)
        {
            List<BaseModel> rules = new List<BaseModel>();

            ArrayList items = Main.mainData.getItemAll(type);

            //遍历获取有效的数据
            for (int i = 0, len = items.Count; i < len; i++)
            {
                ItemModel item = items[i] as ItemModel;

                //为false则直接跳过
                if (!item.Enable)
                {
                    continue;
                }

                //遍历所有的规则
                for (int j = 0; j < item.Rules.Count; j++)
                {
                    BaseModel rule = item.Rules[j] as BaseModel;

                    if (rule.Enable)
                    {
                        rules.Add(rule);
                    }
                }
            }

            return rules;
        }
        //根据本地文件路径读取文件内容
        private static string readDataFromFile(string path)
        {
            //读取对应文件的数据
            StreamReader file = new StreamReader(path);
            string content = file.ReadToEnd();
            file.Close();

            //返回内容
            return content;
        }
        //从Session中获取path
        private static string getPathFromSession(string fullUrl)
        {
            string path = fullUrl;

            if (path.IndexOf("?") > 0)
            {
                path = path.Substring(0, path.IndexOf("?"));
            }

            return path;
        }
        //判断Content-Type是否html格式
        private static bool judgeContentTypeIsHtml(string contentType)
        {
            return contentType.ToLower().IndexOf("text/html") >= 0;
        }
        //判断是否是document
        private static bool judgeIsDocument(string body)
        {
            //如果内容长度不够，则直接返回
            if(body.Length <= "<!DOCTYPE".Length)
            {
                return false;
            }

            string doctype = body.Substring(0, "<!DOCTYPE".Length);

            if (doctype == "<!DOCTYPE" || doctype == "<!doctype")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}
