using System.Windows.Controls;
using System.Windows.Input;

namespace WorkSummarizerGUI.Views
{
    using System.Windows;
    using System.Windows.Media;

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
            var obj = e.OriginalSource as DependencyObject;

            do
            {
                if (obj == null)
                {
                    break;
                }

                var scrollViewer = obj as ScrollViewer;
                if (scrollViewer != null)
                {
                    if (scrollViewer.Equals(ContentStage))
                    {
                        ContentStage.ScrollToHorizontalOffset(ContentStage.ContentHorizontalOffset - e.Delta);
                        e.Handled = true;
                    }

                    break;
                }

                obj = VisualTreeHelper.GetParent(obj);
            }
            while (obj != null);
        }
    }
}
