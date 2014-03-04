using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Extensibility;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro;
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
        private readonly MainViewModel m_mainViewModel;
        private readonly IPluginRuntime m_pluginRuntime;

        private IEnumerator<string> m_themeAccentChooser = GetThemeAccentChooser();
        private IEnumerator<Theme> m_themeBackgroundChooser = GetThemeBackgroundChooser();

        public MainWindow()
        {
            m_pluginRuntime = SimpleIoc.Default.GetInstance<IPluginRuntime>();

            m_mainViewModel = new MainViewModel(m_pluginRuntime.EventQueryServices, m_pluginRuntime.RenderEventServices);
            InitializeComponent();

            UserAccountButton.Content = Environment.UserName;
            m_themeAccentChooser.MoveNext();
            m_themeBackgroundChooser.MoveNext();

            Messenger.Default.Register<ServiceConfigurationRequest>(this, msg => Dispatcher.Invoke(() => ShowServiceConfigurationFlyout(msg)));
            Messenger.Default.Register<Exception>(this, msg => Dispatcher.Invoke(() => ShowExceptionWindow(msg)));
        }

        private void ShowExceptionWindow(Exception exception)
        {
            this.ShowMessageAsync("Oh noes!", "Something went wrong while working on your request. Check your settings and try again. " + Environment.NewLine + Environment.NewLine + "Hint: " + exception.Message);
        }

        private void ShowServiceConfigurationFlyout(ServiceConfigurationRequest message)
        {
            // TODO move to own view
            var serviceConfigurationViewModels =
                message.Ids
                .Select(p =>
                {
                    var configService = m_pluginRuntime.ConfigurationServices.SingleOrDefault(q => q.Key.Id.Equals(p));
                    return configService.Value != null ? new ServiceConfigurationViewModel(configService.Key.Name, configService.Value) : null;
                })
                .Where(p => p != null)
                .ToList();

            ConfigureView.ItemsSource = serviceConfigurationViewModels;
            ConfigureFlyout.Header = string.Format(CultureInfo.CurrentCulture, "{0} settings", message.Name);
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

        private void OnPreviewContentStageMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;
            if (scrollViewer != null)
            {
                scrollViewer.ScrollToHorizontalOffset(scrollViewer.ContentHorizontalOffset - e.Delta);
                e.Handled = true;
            }
        }

        private void OnChangeThemeAccentClick(object sender, RoutedEventArgs e)
        {
            m_themeAccentChooser.MoveNext();
            ThemeManager.ChangeTheme(Application.Current, ThemeManager.DefaultAccents.First(p => p.Name.Equals(m_themeAccentChooser.Current)), m_themeBackgroundChooser.Current);
            ThemeAccentButton.ToolTip = string.Format(CultureInfo.CurrentCulture, "Theme accent: {0}", m_themeAccentChooser.Current);
        }

        private void OnChangeThemeBackgroundClick(object sender, RoutedEventArgs e)
        {
            m_themeBackgroundChooser.MoveNext();
            ThemeManager.ChangeTheme(Application.Current, ThemeManager.DefaultAccents.First(p => p.Name.Equals(m_themeAccentChooser.Current)), m_themeBackgroundChooser.Current);
            ThemeBackgroundButton.ToolTip = string.Format(CultureInfo.CurrentCulture, "Theme background: {0}", m_themeBackgroundChooser.Current);
        }

        private static IEnumerator<Theme> GetThemeBackgroundChooser()
        {
            while (true)
            {
                yield return Theme.Light;
                yield return Theme.Dark;
            }
        }

        private static IEnumerator<string> GetThemeAccentChooser()
        {
            while (true)
            {
                yield return "Sienna";
                yield return "Amber";
                yield return "Blue";
                yield return "Brown";
                yield return "Cobalt";
                yield return "Crimson";
                yield return "Cyan";
                yield return "Emerald";
                yield return "Green";
                yield return "Indigo";
                yield return "Lime";
                yield return "Magenta";
                yield return "Mauve";
                yield return "Olive";
                yield return "Orange";
                yield return "Pink";
                yield return "Purple";
                yield return "Red";
                yield return "Steel";
                yield return "Teal";
                yield return "Violet";
                yield return "Yellow";
            }
        }

        private void OnLicenseClick(object sender, RoutedEventArgs e)
        {
            var licenseWindow = new LicenseWindow();
            licenseWindow.Owner = this;
            licenseWindow.ShowDialog();
        }
    }
}
