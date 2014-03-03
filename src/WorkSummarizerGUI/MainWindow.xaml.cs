using System.Globalization;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls.Dialogs;
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
using Extensibility;
using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.Generic;

namespace WorkSummarizerGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly MainViewModel m_mainViewModel;
        private readonly IPluginRuntime m_pluginRuntime;

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
            m_pluginRuntime = pluginRuntime;

            m_mainViewModel = new MainViewModel(pluginRuntime.EventQueryServices, pluginRuntime.RenderEventServices);
            InitializeComponent();

            UserAccountButton.Content = Environment.UserName;

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

        private IEnumerator<string> m_themeAccentChooser = GetThemeAccentChooser();

        private void OnChangeThemeAccentClick(object sender, RoutedEventArgs e)
        {
            m_themeAccentChooser.MoveNext();
            var nextThemeAccent = m_themeAccentChooser.Current;
            Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml", UriKind.RelativeOrAbsolute) });
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml", UriKind.RelativeOrAbsolute) });
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Colors.xaml", UriKind.RelativeOrAbsolute) });
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/" + nextThemeAccent + ".xaml", UriKind.RelativeOrAbsolute) });
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Accents/BaseLight.xaml", UriKind.RelativeOrAbsolute) });
            ThemeAccentButton.ToolTip = string.Format(CultureInfo.CurrentCulture, "Theme accent: {0}. Click to change", nextThemeAccent);
        }

        private static IEnumerator<string> GetThemeAccentChooser()
        {
            while (true)
            {
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
                yield return "Sienna";
            }
        }
    }
}
