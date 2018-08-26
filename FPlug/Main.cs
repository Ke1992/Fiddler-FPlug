using Fiddler;
using FPlug.Models;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using FPlug.Tools;

namespace FPlug
{
    public class Main : UserControl, IFiddlerExtension
    {
        public static TabPage page;
        public static int iconCount;
        public static Container container;
        public static MainModel mainData;

        #region 构造函数、Init函数(初始化UI界面)
        public Main()
        {
            //添加tab的icon图片进入列表
            FiddlerApplication.UI.tabsViews.ImageList.Images.Add(Properties.Resources.icon);
            FiddlerApplication.UI.tabsViews.ImageList.Images.Add(Properties.Resources.icon_no);

            //首先保存所有icon的总数、当前tab的page的数量
            iconCount = FiddlerApplication.UI.tabsViews.ImageList.Images.Count;

            //初始化page
            page = new TabPage("FPlug");
            //将page加入fiddler的tab选项卡中
            FiddlerApplication.UI.tabsViews.TabPages.Add(page);
            //初始化icon
            page.ImageIndex = iconCount - 2;
        }
        private void Init()
        {
            //将WinForm和WPF联系起来(在WinForm中调用WPF)
            ElementHost element = new ElementHost();
            element.Child = container;
            element.Dock = DockStyle.Fill;

            page.Controls.Add(element);
        }
        #endregion

        public void OnBeforeUnload()
        {

        }

        public void OnLoad()
        {
            #region 用户点击插件的tab才渲染页面
            //初始化数据
            mainData = new MainModel();
            //初始化UI
            container = new Container();

            //创建委托对象
            TabControlEventHandler tabSelectedEvent = null;
            tabSelectedEvent = delegate (object obj, TabControlEventArgs e)
            {
                if (e.TabPage == page)
                {
                    //初始化UI
                    Init();
                    //移除委托监听
                    FiddlerApplication.UI.tabsViews.Selected -= tabSelectedEvent;
                    FiddlerApplication.Log.LogString("FPlug初始化完成！");
                }
            };

            //添加委托监听
            FiddlerApplication.UI.tabsViews.Selected += tabSelectedEvent;

            //监听请求响应之前
            FiddlerApplication.BeforeRequest += delegate (Session session)
            {
                FiddlerTool.handleRequest(session);
            };

            //监听请求响应之后
            FiddlerApplication.BeforeResponse += delegate (Session session)
            {
                FiddlerTool.handleResponse(session);
            };

            //监听websocket的请求
            FiddlerApplication.OnWebSocketMessage += delegate (object session, WebSocketMessageEventArgs msg)
            {
                FiddlerTool.handleWebSocket(session, msg);
            };
        }
        #endregion
    }
}
