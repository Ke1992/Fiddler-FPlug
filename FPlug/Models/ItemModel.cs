using System.Collections;
using System.Windows;

namespace FPlug.Models
{
    public class ItemModel: BaseModel
    {
        #region name
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }
        #endregion

        #region show
        private bool _show;
        public bool Show
        {
            get { return _show; }
            set
            {
                if (_show != value)
                {
                    _show = value;
                    NotifyPropertyChanged("ContentShow");
                    NotifyPropertyChanged("ContentHide");
                }
            }
        }
        public Visibility ContentShow
        {
            get
            {
                return _show ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public Visibility ContentHide
        {
            get
            {
                return _show ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        #endregion

        //所有的规则数组
        public ArrayList Rules;

        public ItemModel(int index, bool enable, string name, bool show):base(index, enable)
        {
            _name = name;
            _show = show;

            //初始化规则数组
            Rules = new ArrayList();
        }
    }
}
