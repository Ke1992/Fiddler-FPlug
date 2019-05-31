using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using FPlug.Tools;

namespace FPlug.Models
{
    public class MainModel
    {
        //当前显示的tab
        public string type = "host";
        //全部配置数据
        private Dictionary<string, ArrayList> data;
        public Dictionary<string, ArrayList> Data
        {
            get
            {
                return data;
            }
        }
        //全部Log日志
        private JArray logs = new JArray();
        public JArray Logs
        {
            get
            {
                return logs;
            }
        }
        //全部JS注入数据
        private List<string> invadeData = new List<string>();

        #region 构造函数
        public MainModel()
        {
            //初始化备份文件夹
            DataTool.initBackUpFolder();

            //初始化数据
            data = DataTool.initConfigData();
        }
        #endregion

        #region Enable--数据处理函数
        public bool getEnable()
        {
            return (bool)(data["enable"] as ArrayList)[0];
        }
        public bool changeEnable()
        {
            bool enable = (bool)(data["enable"] as ArrayList)[0];

            (data["enable"] as ArrayList)[0] = !enable;

            //重新写入文件
            DataTool.writeConfigToFile();

            return !enable;
        }
        #endregion

        #region Item--数据处理函数区域
        //获取Item所有数据
        public ArrayList getItemAll(string itemType = "")
        {
            if (itemType != "")
            {
                return data[itemType] as ArrayList;
            }
            else
            {
                return data[type] as ArrayList;
            }
        }

        //新增Item数据
        public ItemModel addItemToData(string name)
        {
            ArrayList items = data[type] as ArrayList;

            //创建数据
            ItemModel item = new ItemModel(items.Count, false, name, false);

            //添加数据
            items.Add(item);

            //重新写入文件
            DataTool.writeConfigToFile();

            //返回数据
            return item;
        }

        //获取Item数据
        public ItemModel getItemByIndex(int index)
        {
            ArrayList items = data[type] as ArrayList;

            return items[index] as ItemModel;
        }

        //修改Item数据
        public void modifyItemByIndex(int index, string name)
        {
            ArrayList items = data[type] as ArrayList;

            //获取数据
            ItemModel item = items[index] as ItemModel;

            //更新数据
            item.Name = name;

            //重新写入文件
            DataTool.writeConfigToFile();
        }

        //变更Item的Enable数据
        public void changeItemEnableByIndex(int index)
        {
            //获取数据
            ItemModel item = getItemByIndex(index);

            //变更状态
            item.Enable = !item.Enable;

            //重新写入文件
            DataTool.writeConfigToFile();
        }

        //禁止所有Item
        public void disabledAllItemFromData()
        {
            ArrayList items = data[type] as ArrayList;

            for (int i = 0, len = items.Count; i < len; i++)
            {
                ItemModel item = items[i] as ItemModel;

                //更新数据
                item.Enable = false;
            }

            //重新写入文件
            DataTool.writeConfigToFile();
        }

        //删除Item
        public void deleteItemByIndex(int index)
        {
            ArrayList items = data[type] as ArrayList;

            //删除对应的数据
            items.RemoveAt(index);

            //遍历修改下标值
            for (int i = 0, len = items.Count; i < len; i++)
            {
                ItemModel item = items[i] as ItemModel;
                item.Index = i;
                //修改Rule的parentIndex
                DataTool.changeRuleParentIndex(i);
            }

            //重新写入文件
            DataTool.writeConfigToFile();
        }

        //移动Item
        public void moveItemByType(int index, string moveType)
        {
            ArrayList items = data[type] as ArrayList;

            //第一个数据
            if (index == 0 && (moveType == "up" || moveType == "top"))
            {
                return;
            }

            //最后一个数据
            if (index == items.Count - 1 && moveType == "down")
            {
                return;
            }

            //移动数据
            if (moveType == "up")
            {
                //上移
                items.Insert(index - 1, items[index]);
                items.RemoveAt(index + 1);
            }
            else if (moveType == "down")
            {
                //下移
                items.Insert(index, items[index + 1]);
                items.RemoveAt(index + 2);
            }
            else
            {
                //置顶
                items.Insert(0, items[index]);
                items.RemoveAt(index + 1);
            }

            //遍历修改下标值
            for (int i = 0, len = items.Count; i < len; i++)
            {
                ItemModel item = items[i] as ItemModel;
                item.Index = i;
                //修改Rule的parentIndex
                DataTool.changeRuleParentIndex(i);
            }

            //重新写入文件
            DataTool.writeConfigToFile();
        }
        #endregion

        #region Rule--数据处理函数区域
        //获取Item下所有的Rule
        public ArrayList getRuleAll(int parentIndex)
        {
            ArrayList items = data[type] as ArrayList;
            return (items[parentIndex] as ItemModel).Rules as ArrayList;
        }

        //新增Rule数据
        public T addRuleToItem<T>(int parentIndex, JObject param)
        {
            object rule = null;

            ArrayList items = data[type] as ArrayList;
            ArrayList rules = (items[parentIndex] as ItemModel).Rules as ArrayList;

            if (type == "host")
            {
                //获取所有的参数
                string ip = param["ip"].ToString();
                string port = param["port"].ToString();
                string url = param["url"].ToString();

                //新建数据
                rule = new HostModel(parentIndex, rules.Count, true, ip, port, url);
            }
            else if (type == "file")
            {
                //获取所有的参数
                string url = param["url"].ToString();
                string path = param["path"].ToString();

                //新建数据
                rule = new FileModel(parentIndex, rules.Count, true, url, path);
            }
            else if (type == "https")
            {
                //获取所有的参数
                string url = param["url"].ToString();

                //新建数据
                rule = new HttpsModel(parentIndex, rules.Count, true, url);
            }
            else if (type == "header")
            {
                //获取所有的参数
                string type = param["type"].ToString();
                string url = param["url"].ToString();
                string key = param["key"].ToString();
                string value = param["value"].ToString();

                //新建数据
                rule = new HeaderModel(parentIndex, rules.Count, true, type, url, key, value);
            }

            //添加数据
            rules.Add(rule);

            //重新写入文件
            DataTool.writeConfigToFile();

            return (T)rule;
        }

        //获取Rule数据
        public T getRuleByIndex<T>(int parentIndex, int index)
        {
            ArrayList items = data[type] as ArrayList;
            ArrayList rules = (items[parentIndex] as ItemModel).Rules as ArrayList;

            return (T)rules[index];
        }

        //修改Item数据
        public void modifyRuleByIndex(int parentIndex, int index, JObject param)
        {
            ArrayList items = data[type] as ArrayList;
            ArrayList rules = (items[parentIndex] as ItemModel).Rules as ArrayList;

            if (type == "host")
            {
                //获取规则
                HostModel rule = rules[index] as HostModel;
                //更新数据
                rule.IP = param["ip"].ToString();
                rule.Port = param["port"].ToString();
                rule.Url = param["url"].ToString();
            }
            else if (type == "file")
            {
                //获取规则
                FileModel rule = rules[index] as FileModel;
                //更新数据
                rule.Url = param["url"].ToString();
                rule.Path = param["path"].ToString();
            }
            else if (type == "https")
            {
                //获取规则
                HttpsModel rule = rules[index] as HttpsModel;
                //更新数据
                rule.Url = param["url"].ToString();
            }
            else if (type == "header")
            {
                //获取规则
                HeaderModel rule = rules[index] as HeaderModel;
                //更新数据
                rule.Type = param["type"].ToString();
                rule.Url = param["url"].ToString();
                rule.Key = param["key"].ToString();
                rule.Value = param["value"].ToString();
            }

            //重新写入文件
            DataTool.writeConfigToFile();
        }

        //变更Item的Enable数据
        public void changeRuleEnableByIndex(int parentIndex, int index)
        {
            //获取数据
            BaseModel rule = Main.mainData.getRuleByIndex<BaseModel>(parentIndex, index);

            //变更状态
            rule.Enable = !rule.Enable;

            //重新写入文件
            DataTool.writeConfigToFile();
        }

        //删除对应的Item数据
        public void deleteRuleByIndex(int parentIndex, int index)
        {
            ArrayList items = data[type] as ArrayList;
            ArrayList rules = (items[parentIndex] as ItemModel).Rules as ArrayList;

            //删除对应数据
            rules.RemoveAt(index);

            //遍历修改下标值
            for (int i = 0, len = rules.Count; i < len; i++)
            {
                BaseModel rule = rules[i] as BaseModel;
                rule.Index = i;
            }

            //重新写入文件
            DataTool.writeConfigToFile();
        }

        //移动Rule
        public void moveRuleByType(int parentIndex, int index, string moveType)
        {
            ArrayList items = data[type] as ArrayList;
            ArrayList rules = (items[parentIndex] as ItemModel).Rules as ArrayList;

            //第一个数据
            if (index == 0 && (moveType == "up" || moveType == "top"))
            {
                return;
            }

            //最后一个数据
            if (index == rules.Count - 1 && moveType == "down")
            {
                return;
            }

            //移动数据
            if (moveType == "up")
            {
                //上移
                rules.Insert(index - 1, rules[index]);
                rules.RemoveAt(index + 1);
            }
            else if (moveType == "down")
            {
                //下移
                rules.Insert(index, rules[index + 1]);
                rules.RemoveAt(index + 2);
            }
            else
            {
                //置顶
                rules.Insert(0, rules[index]);
                rules.RemoveAt(index + 1);
            }

            //遍历修改下标值
            for (int i = 0, len = rules.Count; i < len; i++)
            {
                BaseModel rule = rules[i] as BaseModel;
                rule.Index = i;
            }

            //重新写入文件
            DataTool.writeConfigToFile();
        }
        #endregion

        #region Tool--数据处理函数区域
        //变更Tool的Enable数据
        public bool changeItemEnableByType(string type)
        {
            Dictionary<string, ToolModel> tools = (data["tools"] as ArrayList)[0] as Dictionary<string, ToolModel>;

            //获取数据
            ToolModel tool = tools[type] as ToolModel;

            //变更属性
            tool.Enable = !tool.Enable;

            //重新写入文件
            DataTool.writeConfigToFile();

            return tool.Enable;
        }
        //变更Tool的Content数据
        public void changeItemContentByType(string type, string content)
        {
            Dictionary<string, ToolModel> tools = (data["tools"] as ArrayList)[0] as Dictionary<string, ToolModel>;

            //获取数据
            ToolModel tool = tools[type] as ToolModel;

            //变更属性
            tool.Content = content;

            //重新写入文件
            DataTool.writeConfigToFile();
        }
        //根据类型获取Tool数据
        public ToolModel getToolByType(string type)
        {
            Dictionary<string, ToolModel> tools = (data["tools"] as ArrayList)[0] as Dictionary<string, ToolModel>;

            return tools[type] as ToolModel;
        }
        //添加日志记录
        public void addLog(string url, string serial, string type, string content)
        {
            JObject log = new JObject();

            //添加数据
            log.Add("url", url);
            log.Add("serial", serial);
            log.Add("type", type);
            log.Add("content", content);

            //添加日志
            logs.Add(log);
        }
        //清空日志
        public void clearAllLog()
        {
            logs.Clear();
        }
        //添加JS注入脚本
        public void addJavaScriptToInvadeData(string content)
        {
            invadeData.Add(content);
        }
        //获取JS注入脚本
        public string getJavaScriptFormInvadeData()
        {
            //如果没有数据，直接返回""
            if (invadeData.Count <= 0)
            {
                return "";
            }

            //获取最新的数据
            string content = invadeData[0];

            invadeData.RemoveAt(0);

            return content;
        }
        #endregion
    }
}
