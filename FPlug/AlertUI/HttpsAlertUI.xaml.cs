using FPlug.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Newtonsoft.Json.Linq;

namespace FPlug.AlertUI
{
    /// <summary>
    /// HttpsAlertUI.xaml 的交互逻辑
    /// </summary>
    public partial class HttpsAlertUI : UserControl
    {
        private int _parentIndex;
        private int _index;
        private string _type;

        public HttpsAlertUI(int parentIndex, int index, string type)
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
            HttpsModel rule = Main.mainData.getRuleByIndex<HttpsModel>(_parentIndex, _index);

            //设置数据
            this.url.Text = rule.Url;
        }
        #endregion

        #region 鼠标点击事件
        private void addHttpsRule(object sender, MouseButtonEventArgs e)
        {
            string url = this.url.Text;

            if (url.Length == 0)
            {
                Fiddler.FiddlerApplication.DoNotifyUser("请填写URL", "输入提示");
                return;
            }

            //生产参数
            JObject param = new JObject();
            param["url"] = url;

            if (_index >= 0 && _type != "copy")
            {
                //修改数据
                Main.mainData.modifyRuleByIndex(_parentIndex, _index, param);
            }
            else
            {
                //添加Rule数据
                HttpsModel rule = Main.mainData.addRuleToItem<HttpsModel>(_parentIndex, param);
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
                addHttpsRule(null, null);
            }
        }
        #endregion
    }
}
