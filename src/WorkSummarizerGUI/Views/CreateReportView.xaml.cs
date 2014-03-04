using System.Windows.Controls;
using System.Windows.Input;

namespace WorkSummarizerGUI.Views
{
    /// <summary>
    /// Interaction logic for CreateReportView.xaml
    /// </summary>
    public partial class CreateReportView : UserControl
    {
        public CreateReportView()
        {
            InitializeComponent();
        }
        
        private void OnPreviewContentStageMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;
            if (scrollViewer != null)
            {
                scrollViewer.ScrollToHorizontalOffset(scrollViewer.ContentHorizontalOffset - e.Delta);
                e.Handled = true;
            }
        }
    }
}
