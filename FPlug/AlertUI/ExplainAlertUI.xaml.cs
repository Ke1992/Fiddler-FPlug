using System.Windows;
using System.Windows.Controls;

namespace FPlug.AlertUI
{
    /// <summary>
    /// ExplainAlertUI.xaml 的交互逻辑
    /// </summary>
    public partial class ExplainAlertUI : UserControl
    {
        public ExplainAlertUI(string type)
        {
            InitializeComponent();

            if (this.FindName(type) != null)
            {
                (this.FindName(type) as StackPanel).Visibility = Visibility.Visible;
            }
        }
    }
}
