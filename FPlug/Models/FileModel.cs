namespace FPlug.Models
{
    class FileModel: BaseModel
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

        #region path
        private string _path;
        public string Path
        {
            get { return _path; }
            set
            {
                if (_path != value)
                {
                    _path = value;
                    NotifyPropertyChanged("Path");
                }
            }
        }
        #endregion

        public FileModel(int parentIndex, int index, bool enable, string url, string path):base(index, enable, parentIndex)
        {
            _url = url;
            _path = path;
        }
    }
}
