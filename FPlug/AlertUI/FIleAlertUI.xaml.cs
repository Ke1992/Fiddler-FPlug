using FPlug.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Newtonsoft.Json.Linq;

namespace FPlug.AlertUI
{
    /// <summary>
    /// FIleAlertUI.xaml 的交互逻辑
    /// </summary>
    public partial class FIleAlertUI : UserControl
    {
        private int _parentIndex;
        private int _index;
        private string _type;

        public FIleAlertUI(int parentIndex, int index, string type)
        {
            _parentIndex = parentIndex;
            _index = index;
            _type = type;

            //初始化
            InitializeComponent();

            //设置输入框内容
            setInputText();
        }

        #region 设置输入框内容
        private void setInputText()
        {
            if (_parentIndex < 0 || _index < 0)
            {
                return;
            }

            //获取数据
            FileModel rule = Main.mainData.getRuleByIndex<FileModel>(_parentIndex, _index);

            //设置数据
            this.url.Text = rule.Url;
            this.path.Text = rule.Path;
        }
        #endregion

        #region 鼠标点击事件
        private void addFileRule(object sender, MouseButtonEventArgs e)
        {
            string url = this.url.Text;
            string path = this.path.Text;

            if (url.Length == 0)
            {
                Fiddler.FiddlerApplication.DoNotifyUser("Please input the url", "Error tips");
                return;
            }

            if (path.Length == 0)
            {
                Fiddler.FiddlerApplication.DoNotifyUser("Please input the file path", "Error tips");
                return;
            }

            //生产参数
            JObject param = new JObject();
            param["url"] = url;
            param["path"] = path;

            if (_index >= 0 && _type != "copy")
            {
                //修改数据
                Main.mainData.modifyRuleByIndex(_parentIndex, _index, param);
            }
            else
            {
                //添加Rule数据
                FileModel rule = Main.mainData.addRuleToItem<FileModel>(_parentIndex, param);
                //添加UI
                Main.container.addRuleToUI(rule);
                //获取Item数据
                ItemModel item = Main.mainData.getItemByIndex(_parentIndex);
                //显示对应区域
                item.Show = true;
            }

            //关闭弹框
            (this.Parent as Window).Close();
        }
        #endregion

        #region 输入框按键监听事件
        private void inputKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                addFileRule(null, null);
            }
        }
        #endregion
    }
}
