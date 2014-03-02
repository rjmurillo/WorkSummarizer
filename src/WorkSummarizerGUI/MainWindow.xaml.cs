using System.Globalization;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using Events.CodeFlow;
using Events.Connect;
using Events.Kudos;
using Events.ManicTime;
using Events.Outlook;
using Events.TeamFoundationServer;
using Events.Yammer;
using Renders.Console;
using Renders.Excel;
using Renders.HTML;
using WorkSummarizer;
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
            var pluginRuntime = new PluginRuntime();
            pluginRuntime.Start(new[]
            {
                typeof(CodeFlowPlugin),
                typeof(ConnectPlugin),
                typeof(KudosPlugin),
                typeof(ManicTimePlugin),
                typeof(OutlookPlugin),
                typeof(TeamFoundationServerPlugin),
                typeof(YammerPlugin),

                typeof(ExcelRenderPlugin),
                typeof(HtmlRenderPlugin),
            });

            m_mainViewModel = new MainViewModel(pluginRuntime.EventQueryServices, pluginRuntime.RenderEventServices);
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
