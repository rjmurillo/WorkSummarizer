using System.Windows;
using WorkSummarizerGUI.ViewModels;

namespace WorkSummarizerGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel m_mainViewModel;

        public MainWindow()
        {
            m_mainViewModel = new MainViewModel();
            InitializeComponent();
        }

        public MainViewModel ViewModel
        {
            get { return m_mainViewModel; }
        }

        private async void OnGenerateClickAsync(object sender, RoutedEventArgs e)
        {
            await m_mainViewModel.GenerateAsync();
        }
    }
}
