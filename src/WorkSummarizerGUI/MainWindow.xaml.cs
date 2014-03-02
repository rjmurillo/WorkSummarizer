using System.Globalization;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using WorkSummarizerGUI.Models;
using WorkSummarizerGUI.ViewModels;

namespace WorkSummarizerGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly MainViewModel m_mainViewModel;

        public MainWindow()
        {
            m_mainViewModel = new MainViewModel();
            InitializeComponent();

            Messenger.Default.Register<ServiceConfigurationRequest>(this, ShowWindow);
        }

        private void ShowWindow(ServiceConfigurationRequest message)
        {
            // TODO move to own view
            ConfigureView.ItemsSource = message.Ids;
            ConfigureViewName.Text = message.Name;
            ConfigureFlyout.IsOpen = true;
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
