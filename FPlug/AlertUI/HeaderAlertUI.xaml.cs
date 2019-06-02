using FPlug.Models;
using FPlug.Tools;
using Newtonsoft.Json.Linq;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace FPlug.AlertUI
{
    /// <summary>
    /// HeaderAlertUI.xaml 的交互逻辑
    /// </summary>
    public partial class HeaderAlertUI : UserControl
    {
        private int _parentIndex;
        private int _index;
        private string _type;

        //生产相关数据
        private string _key = StaticResourcesTool.headerKeys[1];
        private bool _isReq = true;

        public HeaderAlertUI(int parentIndex, int index, string type)
        {
            _parentIndex = parentIndex;
            _index = index;
            _type = type;

            //初始化
            InitializeComponent();

            //设置输入框内容
            setInputText();
            //设置ComboBoxItem
            setComboBoxItems();
            //更新UI
            this.updateUI();
        }

        #region 初始化相关函数: setInputText、setComboBoxItems
        //设置输入框内容
        private void setInputText()
        {
            if (_parentIndex < 0 || _index < 0)
            {
                return;
            }

            //获取数据
            HeaderModel rule = Main.mainData.getRuleByIndex<HeaderModel>(_parentIndex, _index);

            //设置数据
            this._key = rule.Key;
            this.url.Text = rule.Url;
            this.value.Text = rule.Value;
            this._isReq = rule.Type == "req";
        }

        //设置ComboBoxItem
        private void setComboBoxItems()
        {
            //获取所有的key
            string[] keys = StaticResourcesTool.headerKeys;

            for (int i = 0, len = keys.Length; i < len; i++)
            {
                //新生成选择项
                ComboBoxItem item = new ComboBoxItem();

                //设置相关数据
                item.Content = keys[i];
                item.Tag = keys[i];

                //添加进选择框
                this.comboxBox.Items.Add(item);
            }

            //查找是否是预设key值
            int index = Array.IndexOf(keys, this._key);

            //设置默认选项
            this.customKey.Text = index == -1 ? this._key : "";
            this.comboxBox.SelectedIndex = index == -1 ? 0 : index;
        }
        #endregion

        #region 鼠标点击事件
        //type切换点击事件
        private void handleTypeClickEvent(object sender, MouseButtonEventArgs e)
        {
            string tag = (sender as Ellipse).Tag.ToString();
            //更新数据
            this._isReq = tag == "req";
            //更新UI
            this.updateUI();
        }
        
        //保存点击事件
        private void addHeaderRule(object sender, MouseButtonEventArgs e)
        {
            string url = this.url.Text;
            string value = this.value.Text;
            string type = this._isReq ? "req" : "res";
            string key = this._key == StaticResourcesTool.headerKeys[0] ? this.customKey.Text : this._key;

            if (type.Length == 0)
            {
                Fiddler.FiddlerApplication.DoNotifyUser("Please input the type", "Error tips");
                return;
            }

            if (url.Length == 0)
            {
                Fiddler.FiddlerApplication.DoNotifyUser("Please input the url", "Error tips");
                return;
            }

            if (key.Length == 0)
            {
                Fiddler.FiddlerApplication.DoNotifyUser("Please input the key", "Error tips");
                return;
            }

            if (value.Length == 0)
            {
                Fiddler.FiddlerApplication.DoNotifyUser("Please input the value", "Error tips");
                return;
            }

            //生产参数
            JObject param = new JObject();
            param["type"] = type;
            param["url"] = url;
            param["key"] = key;
            param["value"] = value;

            if (_index >= 0 && _type != "copy")
            {
                //修改数据
                Main.mainData.modifyRuleByIndex(_parentIndex, _index, param);
            }
            else
            {
                //添加Rule数据
                HeaderModel rule = Main.mainData.addRuleToItem<HeaderModel>(_parentIndex, param);
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
        
        //comboBox选择事件
        private void comboBoxSelectEvent(object sender, SelectionChangedEventArgs e)
        {
            string tag = ((sender as ComboBox).SelectedItem as ComboBoxItem).Tag.ToString();
            //更新数据
            this._key = tag;
            //如果不是自定义则清空
            if (tag != StaticResourcesTool.headerKeys[0])
            {
                this.customKey.Text = "";
                this.customKey.IsEnabled = false;
            }
            else
            {
                this.customKey.IsEnabled = true;
            }
            //设置前置Label的内容
            this.comboxBoxFront.Content = tag;
        }
        #endregion

        #region 输入框按键监听事件
        private void inputKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                addHeaderRule(null, null);
            }
        }
        #endregion

        #region 更新UI
        private void updateUI()
        {
            this.req.Visibility = this._isReq ? Visibility.Visible : Visibility.Collapsed;
            this.res.Visibility = this._isReq ? Visibility.Collapsed : Visibility.Visible;
        }
        #endregion
    }
}
