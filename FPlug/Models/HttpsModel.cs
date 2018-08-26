namespace FPlug.Models
{
    class HttpsModel: BaseModel
    {
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

        public HttpsModel(int parentIndex, int index, bool enable, string url):base(index, enable, parentIndex)
        {
            _url = url;
        }
    }
}
