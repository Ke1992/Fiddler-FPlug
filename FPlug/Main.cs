using Fiddler;
using FPlug.Models;
using FPlug.Tools;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace FPlug
{
    public class Main : UserControl, IFiddlerExtension
    {
        public static TabPage page;
        public static Container container;
        public static MainModel mainData;
        public static float dpiXScale;
        public static float dpiYScale;

        #region 构造函数、Init函数(初始化UI界面)
        public Main()
        {
            //添加tab的icon图片进入列表
            FiddlerApplication.UI.tabsViews.ImageList.Images.Add("FPlug_Icon", Properties.Resources.icon);
            FiddlerApplication.UI.tabsViews.ImageList.Images.Add("FPlug_Icon_No", Properties.Resources.icon_no);

            //初始化page
            page = new TabPage("FPlug");
            //将page加入fiddler的tab选项卡中
            FiddlerApplication.UI.tabsViews.TabPages.Add(page);
            //初始化icon
            page.ImageIndex = FiddlerApplication.UI.tabsViews.ImageList.Images.IndexOfKey("FPlug_Icon");
        }

        private void Init()
        {
            //将WinForm和WPF联系起来(在WinForm中调用WPF)
            ElementHost element = new ElementHost();
            element.Child = container;
            element.Dock = DockStyle.Fill;

            //获取系统的DPI
            var dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
            var dpiYProperty = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);
            int dpiX = (int)dpiXProperty.GetValue(null, null);
            int dpiY = (int)dpiYProperty.GetValue(null, null);

            //设置DPI的缩放比例
            dpiXScale = (float)96.0 / dpiX;
            dpiYScale = (float)96.0 / dpiY;

            //TODO:实在用不来SizeF，先使用不推荐的方法，后面再进行替换
            element.Scale(dpiXScale, dpiYScale);

            //将wpf挂载对象添加到page中
            page.Controls.Add(element);
        }
        #endregion

        public void OnBeforeUnload()
        {

        }

        public void OnLoad()
        {
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
                    //主动发起一个请求，用来查询最新的版本号
                    FiddlerApplication.oProxy.SendRequest(StaticResourcesTool.versionRequestStr, null);
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
    }
}
