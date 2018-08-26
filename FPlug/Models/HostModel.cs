namespace FPlug.Models
{
    class HostModel:BaseModel
    {
        #region ip and port
        private string _ip;
        public string IP
        {
            get { return _ip; }
            set
            {
                if (_ip != value)
                {
                    _ip = value;
                    NotifyPropertyChanged("IpAndPort");
                }
            }
        }

        private string _port;
        public string Port
        {
            get { return _port; }
            set
            {
                if (_port != value)
                {
                    _port = value;
                    NotifyPropertyChanged("IpAndPort");
                }
            }
        }

        public string IpAndPort
        {
            get
            {
                if (_port.Length <= 0)
                {
                    return _ip;
                }
                else
                {
                    return _ip + ":" + _port;
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

        public HostModel(int parentIndex, int index, bool enable, string ip, string port, string url):base(index, enable, parentIndex)
        {
            _ip = ip;
            _port = port;
            _url = url;
        }
    }
}
