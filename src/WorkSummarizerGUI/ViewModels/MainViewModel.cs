using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Events;
using Events.CodeFlow;
using Events.Connect;
using Events.Kudos;
using Events.ManicTime;
using Events.Outlook;
using Events.TeamFoundationServer;
using Events.Yammer;
using Extensibility;
using Renders;
using Renders.Console;
using Renders.Excel;
using Renders.HTML;
using WorkSummarizer;

namespace WorkSummarizerGUI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IEnumerable<ServiceViewModel> m_eventSources;
        private readonly IPluginRuntime m_pluginRuntime;
        private readonly IEnumerable<ServiceViewModel> m_reportingSinks;

        private DateTime m_endLocalTime;
        private bool m_isBusy;
        private DateTime m_startLocalTime;

        public MainViewModel()
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

                typeof(ConsoleRenderPlugin),
                typeof(ExcelRenderPlugin),
                typeof(HtmlRenderPlugin),
            });

            m_pluginRuntime = pluginRuntime;

            m_eventSources =
                pluginRuntime.EventQueryServices
                             .GroupBy(p => p.Key.Family)
                             .Select(p => new ServiceViewModel(p.Key, p.Select(q => q.Key.Id).ToList()))
                             .ToList();

            m_endLocalTime = DateTime.Now;
            
            m_reportingSinks = pluginRuntime.RenderEventServices
                             .GroupBy(p => p.Key.Family)
                             .Select(p => new ServiceViewModel(p.Key, p.Select(q => q.Key.Id).ToList()))
                             .ToList();

            m_startLocalTime = DateTime.Now.AddMonths(-1);
        }
        
        public DateTime EndLocalTime
        {
            get { return m_endLocalTime; }
            set 
            {
                m_endLocalTime = value;
                OnPropertyChanged();
            }
        }
        
        public IEnumerable<ServiceViewModel> EventSources
        {
            get { return m_eventSources; }
        }

        public bool IsBusy
        {
            get { return m_isBusy; }
            private set
            {
                m_isBusy = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<ServiceViewModel> ReportingSinks
        {
            get { return m_reportingSinks; }
        }

        public DateTime StartLocalTime
        {
            get { return m_startLocalTime; }
            set 
            {
                m_startLocalTime = value;
                OnPropertyChanged();
            }
        }

        public string Version
        {
            get { return "v" + Assembly.GetExecutingAssembly().GetName().Version; }
        }

        public void Generate()
        {
            IsBusy = true;

            var selectedEventSourceIds = EventSources.Where(p => p.IsSelected)
                                                     .SelectMany(p => p.ServiceIds)
                                                     .ToList();

            var selectedReportingSinkTypes = ReportingSinks.Where(p => p.IsSelected)
                                                           .SelectMany(p => p.ServiceIds)
                                                           .ToList();

            var selectedStartLocalTime = m_startLocalTime;
            var selectedEndLocalTime = m_endLocalTime;

            if (selectedEventSourceIds.Any() && selectedReportingSinkTypes.Any())
            {
                DispatcherFrame frame = new DispatcherFrame();
                Dispatcher.CurrentDispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    new DispatcherOperationCallback(o =>
                    {
                        frame.Continue = false;

                        try
                        {
                            var eventQueryServiceRegistrations =
                                m_pluginRuntime
                                    .EventQueryServices
                                    .Where(p => selectedEventSourceIds.Contains(p.Key.Id));

                            var renderServiceRegistrations =
                                m_pluginRuntime
                                    .RenderEventServices
                                    .Where(p => selectedReportingSinkTypes.Contains(p.Key.Id));

                            foreach (var eventQueryServiceRegistration in eventQueryServiceRegistrations)
                            {
                                Console.WriteLine("Querying from event query service: " + eventQueryServiceRegistration.Key);
                                var evts = eventQueryServiceRegistration.Value.PullEvents(selectedStartLocalTime, selectedEndLocalTime);

                                IDictionary<string, int> weightedTags = null; // TODO - pass real tags
                                IDictionary<string, int> weightedPeople = null; // TODO
                                IEnumerable<string> importantSentences = null; // TODO

                                foreach (var render in renderServiceRegistrations)
                                {
                                    render.Value.Render(eventQueryServiceRegistration.Key.Id, evts, weightedTags, weightedPeople, importantSentences);
                                }

                                Console.WriteLine();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Oh noes!" + Environment.NewLine + Environment.NewLine + ex);
                        }

                        return null;
                    }), 
                    frame);

                Dispatcher.PushFrame(frame);
            }

            Console.WriteLine("Done.");
            IsBusy = false;
        }
    }
}
