using Fiddler;
using FPlug.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Threading;

namespace FPlug.Tools
{
    class DataTool
    {
        //对应文件夹路径
        private static string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Fiddler2\\FPlug";
        //配置文件路径
        private static string configPath = path + "\\config.json";
        //备份数据路径
        private static string backupPath = path + "\\Backup";

        #region 内部工具--读写方法
        //读取配置文件的数据
        private static JObject readConfigFromFile()
        {
            if (!File.Exists(configPath))
            {
                //文件不存在，则返回null
                return null;
            }
            try
            {
                //读取对应文件的数据
                StreamReader file = new StreamReader(configPath);
                String content = file.ReadToEnd();
                file.Close();
                //解析数据
                JObject data = JObject.Parse(content);
                //返回数据
                return data;
            }
            catch (Exception e)
            {
                Fiddler.FiddlerApplication.Log.LogString("FPlug出现错误(readConfigFromFile函数)：" + e.ToString());
                return null;
            }
        }
        
        //备份配置文件
        private static void backupConfigFile()
        {
            try
            {
                int fileNum = Directory.GetFiles(backupPath, "*.json").Length;

                if (fileNum < 10)
                {
                    FileStream fs = new FileStream(backupPath + "\\backup_" + (fileNum + 1) + ".json", FileMode.Create);
                    StreamWriter sw = new StreamWriter(fs);
                    //开始写入
                    sw.Write(formatConfigData().ToString());
                    //清空缓冲区
                    sw.Flush();
                    //关闭流
                    sw.Close();
                    fs.Close();
                }
                else
                {
                    //首先删除第一个文件
                    File.Delete(backupPath + "\\backup_1.json");
                    //然后将之前的全部改名
                    for (int i = 1; i < 10; i++)
                    {
                        File.Move(backupPath + "\\backup_" + (i + 1) + ".json", backupPath + "\\backup_" + i + ".json");
                    }
                    //重新写入最新备份文件
                    FileStream fs = new FileStream(backupPath + "\\backup_10.json", FileMode.Create);
                    StreamWriter sw = new StreamWriter(fs);
                    //开始写入
                    sw.Write(formatConfigData().ToString());
                    //清空缓冲区
                    sw.Flush();
                    //关闭流
                    sw.Close();
                    fs.Close();
                }
            }
            catch (Exception e)
            {
                Fiddler.FiddlerApplication.Log.LogString("FPlug出现错误(backupConfigFile函数)：" + e.ToString());
            }
        }
        #endregion

        #region 内部工具--配置转JSON
        //格式化配置数据成JSON格式
        private static JObject formatConfigData()
        {
            JObject data = new JObject();
            
            //添加骨架
            data.Add("enable", Main.mainData.getEnable());
            data.Add("host", formatItemData("host"));
            data.Add("file", formatItemData("file"));
            data.Add("https", formatItemData("https"));
            data.Add("tools", formatToolData());

            return data;
        }
        
        //格式化Item数据
        private static JArray formatItemData(string type)
        {
            JArray result = new JArray();
            //获取所有数据
            ArrayList items = Main.mainData.Data[type] as ArrayList;
            //遍历添加配置数据到result
            for (int i = 0, len = items.Count; i < len; i++)
            {
                //获取对应的Item
                ItemModel item = items[i] as ItemModel;
                //生成Json数据
                JObject temp = new JObject();
                temp.Add("enable", item.Enable);
                temp.Add("name", item.Name);
                temp.Add("rules", formatRuleData(item.Rules, type));
                //填充进数据中
                result.Add(temp);
            }
            //返回数据
            return result;
        }
        
        //格式化Host数据
        private static JArray formatRuleData(ArrayList rules, string type)
        {
            JArray result = new JArray();

            for (int i = 0, len = rules.Count; i < len; i++)
            {
                if (type == "host")
                {
                    HostModel rule = rules[i] as HostModel;
                    //生成Json数据
                    JObject temp = new JObject();
                    temp.Add("enable", rule.Enable);
                    temp.Add("ip", rule.IP);
                    temp.Add("port", rule.Port);
                    temp.Add("url", rule.Url);
                    //填充进数组中
                    result.Add(temp);
                }
                else if (type == "file")
                {
                    FileModel rule = rules[i] as FileModel;
                    //生成Json数据
                    JObject temp = new JObject();
                    temp.Add("enable", rule.Enable);
                    temp.Add("url", rule.Url);
                    temp.Add("path", rule.Path);
                    //填充进数组中
                    result.Add(temp);
                }
                else if(type == "https")
                {
                    HttpsModel rule = rules[i] as HttpsModel;
                    //生成Json数据
                    JObject temp = new JObject();
                    temp.Add("enable", rule.Enable);
                    temp.Add("url", rule.Url);
                    //填充进数组中
                    result.Add(temp);
                }
            }

            return result;
        }

        //格式化Tool数据
        private static JObject formatToolData()
        {
            JObject result = new JObject();

            //获取所有tools
            Dictionary<string, ToolModel> tools = Main.mainData.getItemAll("tools")[0] as Dictionary<string, ToolModel>;

            //获取key
            string[] keys = StaticResourcesTool.keys;

            //遍历添加数据
            for(int i = 0, len = keys.Length; i < len; i++)
            {
                ToolModel tool = tools[keys[i]] as ToolModel;

                JObject ceil = new JObject();
                ceil.Add("enable", tool.Enable);
                ceil.Add("content", tool.Content);

                result.Add(keys[i], ceil);
            }

            return result;
        }
        #endregion

        #region 内部工具--解析JSON
        //解析Item数据
        private static ArrayList parseItemData(JArray items, string type)
        {
            ArrayList result = new ArrayList();

            for (int i = 0, len = items.Count; i < len; i++)
            {
                JObject item = items[i] as JObject;

                //生成Item数据
                ItemModel temp = new ItemModel(i, (bool)item["enable"], item["name"].ToString(), false);
                //生成Rule数组
                temp.Rules = parseRuleData(item["rules"] as JArray, i, type);

                result.Add(temp);
            }

            return result;
        }
        //解析Rule数据
        private static ArrayList parseRuleData(JArray rules, int parentIndex, string type)
        {
            ArrayList result = new ArrayList();

            for (int i = 0, len = rules.Count; i < len; i++)
            {
                JObject rule = rules[i] as JObject;
                if (type == "host")
                {
                    HostModel temp = new HostModel(parentIndex, i, (bool)rule["enable"], rule["ip"].ToString(), rule["port"].ToString(), rule["url"].ToString());
                    result.Add(temp);
                }
                else if (type == "file")
                {
                    FileModel temp = new FileModel(parentIndex, i, (bool)rule["enable"], rule["url"].ToString(), rule["path"].ToString());
                    result.Add(temp);
                }
                else if (type == "https")
                {
                    HttpsModel temp = new HttpsModel(parentIndex, i, (bool)rule["enable"], rule["url"].ToString());
                    result.Add(temp);
                }
            }

            return result;
        }
        //解析Tool数据
        private static ArrayList parseToolData(JObject source)
        {
            ArrayList result = new ArrayList();
            Dictionary<string, ToolModel> tools = new Dictionary<string, ToolModel>();

            //获取key
            string[] keys = StaticResourcesTool.keys;
            
            //遍历初始化
            for (int i = 0, len = keys.Length; i < len; i++)
            {
                JObject tool = source[keys[i]] as JObject;

                if (tool == null)
                {
                    tools.Add(keys[i], new ToolModel(false, ""));
                }
                else
                {
                    tools.Add(keys[i], new ToolModel((bool)tool["enable"], tool["content"].ToString()));

                    //serverip判断是否展示
                    if (keys[i] == "serverip" && (bool)tool["enable"])
                    {
                        try
                        {
                            //初始化一个计时器
                            DispatcherTimer timer = new DispatcherTimer();
                            //设置计时器时间
                            timer.Interval = TimeSpan.FromMilliseconds(2500);
                            //挂载事件
                            timer.Tick += delegate (object sender, EventArgs e)
                            {
                                //显示ServerIP面板
                                FiddlerTool.showHideServerIP(true);
                                //结束倒计时
                                timer.Stop();
                            };
                            //使用计时器
                            timer.Start();
                        }
                        catch (Exception e)
                        {
                            FiddlerApplication.Log.LogString("serveripTimer倒计时出现错误(parseToolData函数)：" + e.ToString());
                        }
                    }
                }
            }

            //加入数组中
            result.Add(tools);
            //返回数据
            return result;
        }
        #endregion

        #region 暴露出去的方法
        //初始化配置数据
        public static Dictionary<string, ArrayList> initConfigData()
        {
            Dictionary<string, ArrayList> result = new Dictionary<string, ArrayList>();
            //获取配置数据
            JObject config = readConfigFromFile();

            if (config == null)
            {
                result.Add("enable", new ArrayList() { true });
                result.Add("host", new ArrayList());
                result.Add("file", new ArrayList());
                result.Add("https", new ArrayList());
                result.Add("tools", parseToolData(new JObject()));
            }
            else
            {
                result.Add("enable", new ArrayList() { (bool)config["enable"] });
                result.Add("host", parseItemData(config["host"] as JArray, "host"));
                result.Add("file", parseItemData(config["file"] as JArray, "file"));
                result.Add("https", parseItemData(config["https"] as JArray, "https"));
                result.Add("tools", parseToolData(config["tools"] as JObject));
            }

            return result;
        }

        //改变所有Rule的ParentIndex
        public static void changeRuleParentIndex(int parentIndex)
        {
            ArrayList rules = Main.mainData.getRuleAll(parentIndex);

            for (int i = 0, len = rules.Count; i < len; i++)
            {
                BaseModel rule = rules[i] as BaseModel;
                rule.ParentIndex = parentIndex;
            }
        }

        //初始化备份文件夹
        public static void initBackUpFolder()
        {
            //备份文件夹不存在直接创建
            if (!Directory.Exists(backupPath))
            {
                Directory.CreateDirectory(backupPath);
            }
        }

        //将配置数据写到本地
        public static void writeConfigToFile()
        {
            try
            {
                FileStream fs = new FileStream(configPath, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);
                //开始写入
                sw.Write(formatConfigData().ToString());
                //清空缓冲区
                sw.Flush();
                //关闭流
                sw.Close();
                fs.Close();
                //数据备份
                backupConfigFile();
            }
            catch (Exception e)
            {
                Fiddler.FiddlerApplication.Log.LogString("FPlug出现错误(writeConfigToFile函数)：" + e.ToString());
            }
        }
        #endregion
    }
}
