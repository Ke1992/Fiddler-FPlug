using System.ComponentModel;
using System.Windows;
using FPlug.Tools;

namespace FPlug.Models
{
    public class TabModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region version、download
        private string _version;
        public string Version
        {
            get
            {
                return _version;
            }
            set
            {
                if (_version != value)
                {
                    _version = value;
                    NotifyPropertyChanged("Version");
                    NotifyPropertyChanged("Download");
                }
            }
        }
        public Visibility Download
        {
            get
            {
                return string.Compare(_version, StaticResourcesTool.version) > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        #endregion

        #region enable、IconSource
        private bool _enable;
        public bool Enable
        {
            set
            {
                if (_enable != value)
                {
                    _enable = value;
                    NotifyPropertyChanged("IconSource");
                }
            }
        }

        public string IconSource
        {
            get
            {
                //变更插件Icon
                FiddlerTool.changeFPlugIcon(_enable);
                //返回数据
                return _enable ? "Resources/icon.png" : "Resources/icon_no.png";
            }
        }
        #endregion

        #region Type
        private string _type;
        public string Type
        {
            set
            {
                if (_type != value)
                {
                    _type = value;

                    NotifyPropertyChanged("HostWrap");
                    NotifyPropertyChanged("FileWrap");
                    NotifyPropertyChanged("HttpsWrap");
                    NotifyPropertyChanged("ToolsWrap");
                    NotifyPropertyChanged("ConsoleWrap");

                    NotifyPropertyChanged("HostTabColor");
                    NotifyPropertyChanged("FileTabColor");
                    NotifyPropertyChanged("HttpsTabColor");
                    NotifyPropertyChanged("ToolsTabColor");
                    NotifyPropertyChanged("ConsoleTabColor");
                }
            }
        }
        #endregion

        #region HostWrap、FileWrap、HttpsWrap、ToolsWrap、ConsoleWrap
        public Visibility HostWrap
        {
            get
            {
                return _type == "host" ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public Visibility FileWrap
        {
            get
            {
                return _type == "file" ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public Visibility HttpsWrap
        {
            get
            {
                return _type == "https" ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public Visibility ToolsWrap
        {
            get
            {
                return _type == "tools" ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public Visibility ConsoleWrap
        {
            get
            {
                return _type == "console" ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        #endregion

        #region HostTabColor、FileTabColor、HttpsTabColor、ToolTabColor、ConsoleTabColor
        public string HostTabColor
        {
            get
            {
                return _type == "host" ? "#FFEC8E72" : "#FFA8A8A8";
            }
        }
        public string FileTabColor
        {
            get
            {
                return _type == "file" ? "#FFEC8E72" : "#FFA8A8A8";
            }
        }
        public string HttpsTabColor
        {
            get
            {
                return _type == "https" ? "#FFEC8E72" : "#FFA8A8A8";
            }
        }
        public string ToolsTabColor
        {
            get
            {
                return _type == "tools" ? "#FFEC8E72" : "#FFA8A8A8";
            }
        }
        public string ConsoleTabColor
        {
            get
            {
                return _type == "console" ? "#FFEC8E72" : "#FFA8A8A8";
            }
        }
        #endregion

        #region ConsoleShow、ConsoleTab
        private bool _consoleShow;
        public bool ConsoleShow
        {
            get
            {
                return _consoleShow;
            }
            set
            {
                if (_consoleShow != value)
                {
                    _consoleShow = value;
                    NotifyPropertyChanged("ConsoleTab");
                }
            }
        }
        public Visibility ConsoleTab
        {
            get
            {
                return _consoleShow ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        #endregion

        #region ConsoleType
        private string _consoleType;
        public string ConsoleType
        {
            get
            {
                return _consoleType;
            }
            set
            {
                if (_consoleType != value)
                {
                    _consoleType = value;

                    NotifyPropertyChanged("ConsoleAllCheckShow");
                    NotifyPropertyChanged("ConsoleAllCheckHide");

                    NotifyPropertyChanged("ConsoleLogCheckShow");
                    NotifyPropertyChanged("ConsoleLogCheckHide");

                    NotifyPropertyChanged("ConsoleWarnCheckShow");
                    NotifyPropertyChanged("ConsoleWarnCheckHide");

                    NotifyPropertyChanged("ConsoleErrorCheckShow");
                    NotifyPropertyChanged("ConsoleErrorCheckHide");
                }
            }
        }
        #endregion

        #region ConsoleAllCheck、ConsoleLogCheck、ConsoleWarnCheck、ConsoleErrorCheck
        public Visibility ConsoleAllCheckShow
        {
            get
            {
                return _consoleType == "all" ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public Visibility ConsoleAllCheckHide
        {
            get
            {
                return _consoleType == "all" ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        public Visibility ConsoleLogCheckShow
        {
            get
            {
                return _consoleType == "log" ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public Visibility ConsoleLogCheckHide
        {
            get
            {
                return _consoleType == "log" ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        public Visibility ConsoleWarnCheckShow
        {
            get
            {
                return _consoleType == "warn" ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public Visibility ConsoleWarnCheckHide
        {
            get
            {
                return _consoleType == "warn" ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        public Visibility ConsoleErrorCheckShow
        {
            get
            {
                return _consoleType == "error" ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public Visibility ConsoleErrorCheckHide
        {
            get
            {
                return _consoleType == "error" ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        #endregion

        #region 通知UI更新数据
        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region 构造函数
        public TabModel()
        {
            _enable = Main.mainData.getEnable();
            _consoleShow = (Main.mainData.getToolByType("console") as ToolModel).Enable;
            _version = "";
            _type = "host";
            _consoleType = "all";
        }
        #endregion
    }
}

