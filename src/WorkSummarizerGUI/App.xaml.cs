using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Ioc;
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
using Extensibility;
using WorkSummarizerGUI.Services;

namespace WorkSummarizerGUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
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

            SimpleIoc.Default.Register<IPluginRuntime>(() => pluginRuntime);
            SimpleIoc.Default.Register<IThemeSwitchService, MahAppsModernThemeSwitchService>();
        }
    }
}
