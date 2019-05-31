using FPlug.Models;
using FPlug.Tools;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace FPlug
{
    partial class GlobalEvent
    {
        #region Item--事件
        //修改项目
        private void modifyItem(object sender, MouseButtonEventArgs e)
        {
            Label content = sender as Label;
            //获取下标
            int index = (int)content.Tag;
            //显示弹框
            AlertTool.showItemAlertUI(index);
        }

        //是否生效事件
        private void changeItemEnable(object sender, MouseButtonEventArgs e)
        {
            Rectangle content = sender as Rectangle;
            //获取下标
            int index = (int)content.Tag;
            //变更生效状态
            Main.mainData.changeItemEnableByIndex(index);
        }

        //是否展开事件
        private void changeItemShow(object sender, MouseButtonEventArgs e)
        {
            Canvas content = sender as Canvas;
            //获取下标
            int index = (int)content.Tag;
            //获取数据
            ItemModel item = Main.mainData.getItemByIndex(index);
            //变更属性
            item.Show = !item.Show;
        }

        //菜单点击事件
        private void handleItemMenuClick(object sender, RoutedEventArgs e)
        {
            string type = (sender as MenuItem).Tag.ToString();
            object target = ((sender as MenuItem).Parent as ContextMenu).PlacementTarget as object;//获取点击源控件
            int index = (int)(target as Label).Tag;

            if (type == "add")
            {
                AlertTool.showRuleAlertUI(index);
            }
            else if (type == "modify")
            {
                modifyItem(target, null);
            }
            else if (type == "delete")
            {
                //删除数据
                Main.mainData.deleteItemByIndex(index);
                //删除对应UI
                Main.container.deleteItemFromUI(index);
            }
            else if (type == "up" || type == "down" || type == "top")
            {
                //移动对应的数据 
                Main.mainData.moveItemByType(index, type);
                //移动对应的UI
                Main.container.moveItemFromUI(index, type);
            }
        }
        #endregion

        #region Rule--事件
        //修改项目
        private void modifyRule(object sender, MouseButtonEventArgs e)
        {
            string[] indexs = (sender as Label).Tag.ToString().Split('_');
            //显示弹框
            AlertTool.showRuleAlertUI(Convert.ToInt32(indexs[0]), Convert.ToInt32(indexs[1]));
        }

        //是否生效事件
        private void changeRuleEnable(object sender, MouseButtonEventArgs e)
        {
            string[] indexs = (sender as Rectangle).Tag.ToString().Split('_');

            //变更状态
            Main.mainData.changeRuleEnableByIndex(Convert.ToInt32(indexs[0]), Convert.ToInt32(indexs[1]));
        }

        //菜单点击事件
        private void handleRuleMenuClick(object sender, RoutedEventArgs e)
        {
            string type = (sender as MenuItem).Tag.ToString();
            object target = ((sender as MenuItem).Parent as ContextMenu).PlacementTarget as object;//获取点击源控件

            //获取对应下标
            string[] indexs = (target as Label).Tag.ToString().Split('_');
            int parentIndex = Convert.ToInt32(indexs[0]);
            int index = Convert.ToInt32(indexs[1]);

            if (type == "modify")
            {
                modifyRule(target, null);
            }
            else if (type == "delete")
            {
                //删除数据
                Main.mainData.deleteRuleByIndex(parentIndex, index);
                //删除对应UI
                Main.container.deleteRuleFromUI(parentIndex, index);
            }
            else if (type == "copy")
            {
                //显示弹框
                AlertTool.showRuleAlertUI(parentIndex, index, "copy");
            }
            else if (type == "up" || type == "down" || type == "top")
            {
                //移动对应的数据 
                Main.mainData.moveRuleByType(parentIndex, index, type);
                //移动对应的UI
                Main.container.moveRuleFromUI(parentIndex, index, type);
            }
            else if (type == "open")
            {
                //打开文件所在位置
                FileModel rule = Main.mainData.getRuleByIndex<FileModel>(parentIndex, index);
                try
                {
                    Process.Start("explorer.exe", "/select," + rule.Path);
                }
                catch (Exception error)
                {
                    Fiddler.FiddlerApplication.DoNotifyUser("打开文件位置失败，请确认路径是否正确", "打开文件位置失败");
                    Fiddler.FiddlerApplication.Log.LogString("FPlug出现错误(handleRuleMenuClick函数)：" + error.ToString());
                }
            }
        }
        #endregion
    }
}
