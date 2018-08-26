namespace FPlug.Models
{
    public class ToolModel : BaseModel
    {
        #region content
        private string _content;
        public string Content
        {
            get { return _content; }
            set
            {
                if (_content != value)
                {
                    _content = value;
                    NotifyPropertyChanged("Content");
                }
            }
        }
        #endregion

        #region IconColor
        public string IconColor
        {
            get
            {
                return Enable ? "#FFFFFFFF" : "#FF919191";
            }
        }
        #endregion

        public ToolModel(bool enable, string content) : base(-1, enable)
        {
            _content = content;
        }
    }
}
