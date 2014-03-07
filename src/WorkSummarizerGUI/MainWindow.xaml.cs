using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using Extensibility;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls.Dialogs;
using WorkSummarizerGUI.Models;
using WorkSummarizerGUI.ViewModels;

namespace WorkSummarizerGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
                        
            Messenger.Default.Register<ServiceConfigurationRequest>(this, msg => Dispatcher.Invoke(() => ShowServiceConfigurationFlyout(msg)));
            Messenger.Default.Register<ExceptionMessage>(this, msg => Dispatcher.Invoke(() => ShowExceptionWindow(msg)));
        }

        private void ShowExceptionWindow(ExceptionMessage exceptionMessage)
        {
            var source = String.IsNullOrWhiteSpace(exceptionMessage.Context) ? "not sure" : exceptionMessage.Context;
            this.ShowMessageAsync("Oh noes!", "Something went wrong while working on your request. Check your settings and try again. " + Environment.NewLine + Environment.NewLine + "From: " + source + Environment.NewLine + Environment.NewLine + "Hint: " + exceptionMessage.Exception.Message);
        }

        private void ShowServiceConfigurationFlyout(ServiceConfigurationRequest message)
        {
            // TODO move to own view
            var serviceConfigurationViewModels =
                message.Ids
                .Select(p =>
                {
                    var configService = SimpleIoc.Default.GetInstance<IPluginRuntime>().ConfigurationServices.SingleOrDefault(q => q.Key.Id.Equals(p));
                    return configService.Value != null ? new ServiceConfigurationViewModel(configService.Key.Name, configService.Value) : null;
                })
                .Where(p => p != null)
                .ToList();

            ConfigureView.ItemsSource = serviceConfigurationViewModels;
            ConfigureFlyout.Header = string.Format(CultureInfo.CurrentCulture, "{0} settings", message.Name);
            ConfigureFlyout.IsOpen = true;
        }

        private void OnLicenseClick(object sender, RoutedEventArgs e)
        {
            var licenseWindow = new LicenseWindow();
            licenseWindow.Owner = this;
            licenseWindow.ShowDialog();
        }
    }
}
