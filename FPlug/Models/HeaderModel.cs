namespace FPlug.Models
{
    class HeaderModel : BaseModel
    {
        #region type
        private string _type;
        public string Type
        {
            get { return _type; }
            set
            {
                if (_type != value)
                {
                    _type = value;
                    NotifyPropertyChanged("Type");
                }
            }
        }
        #endregion

        #region url
        private string _url;
        public string Url
        {
            get { return _url; }
            set
            {
                if (_url != value)
                {
                    _url = value;
                    NotifyPropertyChanged("Url");
                }
            }
        }
        #endregion

        #region key
        private string _key;
        public string Key
        {
            get { return _key; }
            set
            {
                if (_key != value)
                {
                    _key = value;
                    NotifyPropertyChanged("Key");
                }
            }
        }
        #endregion

        #region value
        private string _value;
        public string Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    NotifyPropertyChanged("Value");
                }
            }
        }
        #endregion

        public HeaderModel(int parentIndex, int index, bool enable, string type, string url, string key, string value) : base(index, enable, parentIndex)
        {
            _type = type;
            _url = url;
            _key = key;
            _value = value;
        }
    }
}
