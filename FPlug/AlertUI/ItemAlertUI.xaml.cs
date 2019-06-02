using FPlug.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FPlug.AlertUI
{
    /// <summary>
    /// ItemAlertUI.xaml 的交互逻辑
    /// </summary>
    public partial class ItemAlertUI : UserControl
    {
        private int _index;

        #region 构造函数
        public ItemAlertUI(int index)
        {
            _index = index;

            //初始化
            InitializeComponent();

            //设置输入框内容
            setInputText();
        }
        #endregion

        #region 设置输入框内容
        private void setInputText()
        {
            if(_index < 0)
            {
                return;
            }

            //获取数据
            ItemModel item = Main.mainData.getItemByIndex(_index);

            //设置数据
            this.name.Text = item.Name;
        }
        #endregion

        #region 鼠标点击事件
        private void addItem(object sender, MouseButtonEventArgs e)
        {
            string name = this.name.Text;

            if (name.Length == 0)
            {
                Fiddler.FiddlerApplication.DoNotifyUser("Please input the project name", "Error Tips");
                return;
            }

            if (_index >= 0)
            {
                //修改数据
                Main.mainData.modifyItemByIndex(_index, name);
            }
            else
            {
                //添加数据
                ItemModel item = Main.mainData.addItemToData(name);
                //添加UI
                Main.container.addItemToUI(item);
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
                addItem(null, null);
            }
        }
        #endregion
    }
}
