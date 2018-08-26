using System.ComponentModel;
using System.Windows;

namespace FPlug.Models
{
    public class BaseModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region index and parentIndex
        private int _parentIndex;
        public int ParentIndex
        {
            get { return _parentIndex; }
            set
            {
                if (_parentIndex != value)
                {
                    _parentIndex = value;
                    NotifyPropertyChanged("IndexAndParentIndex");
                }
            }
        }

        private int _index;
        public int Index
        {
            get { return _index; }
            set
            {
                if (_index != value)
                {
                    _index = value;
                    NotifyPropertyChanged("Index");
                    if (_parentIndex >= 0)
                    {
                        NotifyPropertyChanged("IndexAndParentIndex");
                    }
                }
            }
        }

        public string IndexAndParentIndex
        {
            get {
                return _parentIndex + "_" + _index;
            }
        }
        #endregion

        #region enable
        private bool _enable;
        public bool Enable
        {
            get { return _enable; }
            set
            {
                if (_enable != value)
                {
                    _enable = value;
                    NotifyPropertyChanged("CheckShow");
                    NotifyPropertyChanged("CheckHide");
                    NotifyPropertyChanged("IconColor");
                }
            }
        }
        public Visibility CheckShow
        {
            get
            {
                return _enable ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public Visibility CheckHide
        {
            get
            {
                return _enable ? Visibility.Collapsed : Visibility.Visible;
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
        public BaseModel(int index, bool enable, int parentIndex = -1)
        {
            _parentIndex = parentIndex;
            _index = index;
            _enable = enable;
        }
        #endregion
    }
}
