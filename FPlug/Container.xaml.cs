using FPlug.Models;
using FPlug.Tools;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace FPlug
{
    /// <summary>
    /// Container.xaml 的交互逻辑
    /// </summary>
    public partial class Container : UserControl
    {
        private JObject tabShowData;//tab第一次显示标记数据
        public static TabModel tabModel = new TabModel();//容器对应的Tab数据

        public Container()
        {
            InitializeComponent();
            //初始化tab数据
            tabShowData = initTabShowData();
            //绑定TabModel到视图容器
            this.main.DataContext = tabModel;
            //初始化HOST面板
            initItemToUI();
        }

        #region Explain--事件
        //显示说明弹框
        private void showExplainAlertUI(object sender, MouseButtonEventArgs e)
        {
            string type = (sender as Label).Tag.ToString();

            AlertTool.showExplainAlertUI(type);
        }
        #endregion

        #region Tab--事件
        //启用/禁用插件
        private void changeFPlugEnable(object sender, MouseButtonEventArgs e)
        {
            tabModel.Enable = Main.mainData.changeEnable();
        }
        //切换Tab
        private void changeTabByTag(object sender, MouseButtonEventArgs e)
        {
            string tag = (sender as StackPanel).Tag.ToString();

            //更新绑定的数据
            tabModel.Type = tag;
            //更新全局标识
            Main.mainData.type = tag;
            //是否是第一次渲染
            if (updateTabShowData(tag))
            {
                //第一次渲染
                if (tag == "tools")
                {
                    bindToolDataToUI();
                }
                else if (tag != "console")
                {
                    initItemToUI();
                }
            }

            if (tag == "tools")
            {
                this.tool_useragent_content.Focus();
            }
        }
        #endregion

        #region Item--事件
        //增加Item
        private void addItem(object sender, MouseButtonEventArgs e)
        {
            AlertTool.showItemAlertUI();
        }
        //禁止所有Item
        private void disabledAllItem(object sender, MouseButtonEventArgs e)
        {
            Main.mainData.disabledAllItemFromData();
        }
        #endregion

        #region Tool--事件
        //Tool生效事件
        private void changeToolEnable(object sender, MouseButtonEventArgs e)
        {
            string type = (sender as Label).Tag.ToString();

            //变更Enable
            bool enable = Main.mainData.changeItemEnableByType(type);

            //serverip切换显示隐藏
            if (type == "serverip")
            {
                FiddlerTool.showHideServerIP(enable);
            }
            else if (type == "console")
            {
                //切换属性值
                tabModel.ConsoleShow = !tabModel.ConsoleShow;
            }
        }
        //useragent输入监听事件
        private void useragentInput(object sender, KeyEventArgs e)
        {
            Main.mainData.changeItemContentByType("useragent", this.tool_useragent_content.Text.ToString());
        }
        //日志筛选事件
        private void changeConsoleType(object sender, MouseButtonEventArgs e)
        {
            //获取类型
            string type = (sender as FrameworkElement).Tag.ToString();

            //选择和当前相同，直接返回
            if (tabModel.ConsoleType == type)
            {
                return;
            }

            //更新数据
            tabModel.ConsoleType = type;
            //更新日志
            filterLogByType(type);
        }
        //清空所有日志
        private void clearAllLog(object sender, MouseButtonEventArgs e)
        {
            //清空数据
            Main.mainData.clearAllLog();
            //清空UI
            this.tool_console_content.Blocks.Clear();
        }
        //发送日志
        private void sendJavaScriptToWeb(object sender, MouseButtonEventArgs e)
        {
            //获取数据
            string content = this.tool_invade_content.Text.ToString();

            //填充进List
            Main.mainData.addJavaScriptToInvadeData(content);

            //清空
            this.tool_invade_content.Text = "";
        }
        //打开下载页面
        private void goToDownloadPage(object sender, MouseButtonEventArgs e)
        {
            //使用默认浏览器，打开下载页面
            System.Diagnostics.Process.Start("https://github.com/Ke1992/FPlug");
        }
        #endregion

        #region 私有方法(内部工具方法)
        //初始化tabShowData
        private JObject initTabShowData()
        {
            JObject result = new JObject();

            result.Add("host", true);
            result.Add("file", false);
            result.Add("https", false);
            result.Add("tools", false);
            result.Add("console", false);

            return result;
        }
        //更新tabShowData
        private bool updateTabShowData(string type)
        {
            bool result = (bool)tabShowData[type];

            tabShowData[type] = true;

            return !result;
        }
        //初始化Item面板
        private void initItemToUI()
        {
            ArrayList items = Main.mainData.getItemAll();

            //遍历添加Item到UI
            for (int i = 0, len = items.Count; i < len; i++)
            {
                addItemToUI(items[i] as ItemModel);
                //添加Item下的Rule
                initRuleToUI(i);
            }
        }
        //初始化Item下的Rule
        private void initRuleToUI(int parentIndex)
        {
            ArrayList rules = Main.mainData.getRuleAll(parentIndex);

            //遍历添加Rule到UI
            for (int i = 0, len = rules.Count; i < len; i++)
            {
                addRuleToUI(rules[i] as BaseModel);
            }
        }
        //初始化Tool面板(绑定数据)
        private void bindToolDataToUI()
        {
            ArrayList toolsList = Main.mainData.getItemAll("tools");
            Dictionary<string, ToolModel> tools = toolsList[0] as Dictionary<string, ToolModel>;

            //获取key
            string[] keys = StaticResourcesTool.keys;

            //遍历绑定数据
            for (int i = 0, len = keys.Length; i < len; i++)
            {
                (this.FindName("tool_" + keys[i]) as Panel).DataContext = tools[keys[i]] as ToolModel;

                //UA塞入内容
                if (keys[i] == "useragent")
                {
                    this.tool_useragent_content.Text = (tools[keys[i]] as ToolModel).Content;
                }
            }
        }
        //根据类型筛选日志
        private void filterLogByType(string fillterType)
        {
            //清空面板上的数据
            this.tool_console_content.Blocks.Clear();

            //获取所有的日志
            JArray logs = Main.mainData.Logs;

            //重新渲染面板数据
            for (int i = 0, len = logs.Count; i < len; i++)
            {
                JObject log = logs[i] as JObject;

                string type = log["type"].ToString();

                //all || 当前类型
                if (fillterType == "all" || fillterType == type)
                {
                    string url = log["url"].ToString();
                    string serial = log["serial"].ToString();
                    string content = log["content"].ToString();

                    //添加一条记录到面板上
                    addLogToPanel(url, serial, type, content);
                }
            }

            //遍历完成以后，自动滚动到底部
            tool_console_scroll.ScrollToEnd();
        }
        #endregion

        #region 暴露出去的接口--Item
        //添加Item控件
        public void addItemToUI(ItemModel item)
        {
            string type = Main.mainData.type;
            //创建UI对象
            Label label = new Label();

            //根据类型初始化属性
            if (type == "host")
            {
                //设置UI对象属性
                label.Template = Resources["main_wrap_host"] as ControlTemplate;
                label.DataContext = item;

                //添加UI
                this.host.Children.Add(label);
            }
            else if (type == "file")
            {
                //设置UI对象属性
                label.Template = Resources["main_wrap_file"] as ControlTemplate;
                label.DataContext = item;

                //添加UI
                this.file.Children.Add(label);
            }
            else if (type == "https")
            {
                //设置UI对象属性
                label.Template = Resources["main_wrap_https"] as ControlTemplate;
                label.DataContext = item;

                //添加UI
                this.https.Children.Add(label);
            }
        }

        //删除Item控件
        public void deleteItemFromUI(int index)
        {
            string type = Main.mainData.type;

            //根据类型删除
            if (type == "host")
            {
                this.host.Children.RemoveAt(index);
            }
            else if (type == "file")
            {
                this.file.Children.RemoveAt(index);
            }
            else if (type == "https")
            {
                this.https.Children.RemoveAt(index);
            }
        }

        //移动Item控件
        public void moveItemFromUI(int index, string moveType)
        {
            string type = Main.mainData.type;

            if (index <= 0 && moveType == "up")
            {
                Fiddler.FiddlerApplication.DoNotifyUser("已在最顶部", "无法上移");
                return;
            }

            StackPanel panel = null;

            if (type == "host")
            {
                panel = this.host;
            }
            else if (type == "file")
            {
                panel = this.file;
            }
            else if (type == "https")
            {
                panel = this.https;
            }

            if (index == panel.Children.Count - 1 && moveType == "down")
            {
                Fiddler.FiddlerApplication.DoNotifyUser("已在最底部", "无法下移");
                return;
            }

            //移除所有的Item
            panel.Children.Clear();
            //重新渲染所有的Item
            initItemToUI();
        }
        #endregion

        #region 暴露出去的接口--Rule
        //添加Rule控件
        public void addRuleToUI(BaseModel model)
        {
            string type = Main.mainData.type;
            //创建UI对象
            Label label = new Label();
            //Item控件
            Label item = null;

            if (type == "host")
            {
                //设置UI对象属性
                HostModel rule = model as HostModel;
                label.Template = Resources["main_content_host"] as ControlTemplate;
                label.DataContext = rule;

                //获取对应Item控件
                item = this.host.Children[rule.ParentIndex] as Label;
            }
            else if (type == "file")
            {
                //设置UI对象属性
                FileModel rule = model as FileModel;
                label.Template = Resources["main_content_file"] as ControlTemplate;
                label.DataContext = rule;

                //获取对应Item控件
                item = this.file.Children[rule.ParentIndex] as Label;
            }
            else if (type == "https")
            {
                //设置UI对象属性
                HttpsModel rule = model as HttpsModel;
                label.Template = Resources["main_content_https"] as ControlTemplate;
                label.DataContext = rule;

                //获取对应Item控件
                item = this.https.Children[rule.ParentIndex] as Label;
            }

            //首先执行一次ApplyTemplate，确保模板应用到了渲染树中
            item.ApplyTemplate();
            //获取对应的内容
            StackPanel content = item.Template.FindName("content", item) as StackPanel;
            //添加Rule控件
            content.Children.Add(label);
        }

        //删除Rule控件
        public void deleteRuleFromUI(int parentIndex, int index)
        {
            string type = Main.mainData.type;
            //Item控件
            Label item = null;

            if (type == "host")
            {
                //获取对应Item控件
                item = this.host.Children[parentIndex] as Label;
            }
            else if (type == "file")
            {
                //获取对应Item控件
                item = this.file.Children[parentIndex] as Label;
            }
            else if (type == "https")
            {
                //获取对应Item控件
                item = this.https.Children[parentIndex] as Label;
            }

            //获取对应的内容
            StackPanel content = item.Template.FindName("content", item) as StackPanel;
            //删除Rule控件
            content.Children.RemoveAt(index);
        }

        //移动Rule控件
        public void moveRuleFromUI(int parentIndex, int index, string moveType)
        {
            string type = Main.mainData.type;

            if (index <= 0 && moveType == "up")
            {
                Fiddler.FiddlerApplication.DoNotifyUser("已在最顶部", "无法上移");
                return;
            }

            //Item控件
            Label item = null;

            if (type == "host")
            {
                //获取对应Item控件
                item = this.host.Children[parentIndex] as Label;
            }
            else if (type == "file")
            {
                //获取对应Item控件
                item = this.file.Children[parentIndex] as Label;
            }
            else if (type == "https")
            {
                //获取对应Item控件
                item = this.https.Children[parentIndex] as Label;
            }

            //获取对应的内容
            StackPanel content = item.Template.FindName("content", item) as StackPanel;

            if (index == content.Children.Count - 1 && moveType == "down")
            {
                Fiddler.FiddlerApplication.DoNotifyUser("已在最底部", "无法下移");
                return;
            }

            //移除所有的Item
            content.Children.Clear();
            //重新渲染所有的Item
            initRuleToUI(parentIndex);
        }
        #endregion

        #region 暴露出去的接口--日志
        //添加日志到
        public void addLogToPanel(string url, string serial, string type, string content)
        {
            //生成段落
            Paragraph paragraph = new Paragraph();
            //添加样式
            if (type == "error")
            {
                paragraph.Style = Resources["tool_style_console_error"] as Style;
            }
            else if (type == "warn")
            {
                paragraph.Style = Resources["tool_style_console_warn"] as Style;
            }
            else
            {
                paragraph.Style = Resources["tool_style_console_log"] as Style;
            }

            //添加内容
            paragraph.Inlines.Add("来源：" + url);
            paragraph.Inlines.Add(new LineBreak());
            paragraph.Inlines.Add("序号：" + serial);
            paragraph.Inlines.Add(new LineBreak());
            paragraph.Inlines.Add("内容：" + content);

            //向流文档添加内容
            this.tool_console_content.Blocks.Add(paragraph);

            //滚动到底部
            this.tool_console_scroll.ScrollToEnd();
        }
        #endregion
    }
}
