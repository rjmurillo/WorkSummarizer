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
using Renders;
using Renders.Console;
using Renders.Excel;
using Renders.HTML;
using WorkSummarizer;

namespace WorkSummarizerGUI.ViewModels
{
    
    public class MainViewModel : ViewModelBase
    {
        private DateTime m_endLocalTime;
        private readonly IEnumerable<EventSourceViewModel> m_eventSources;
        private bool m_isBusy;
        private readonly IEnumerable<ReportingSinkViewModel> m_reportingSinks;
        private DateTime m_startLocalTime;

        public MainViewModel()
        {
            m_eventSources = new[] 
            {
                new EventSourceViewModel("Fake", typeof(FakeEventsPlugin)) { IsSelected = true }, 
                new EventSourceViewModel("CodeFlow", typeof(CodeFlowPlugin)), 
                new EventSourceViewModel("Connect", typeof(ConnectPlugin)), 
                new EventSourceViewModel("Kudos", typeof(KudosPlugin)), 
                new EventSourceViewModel("ManicTime", typeof(ManicTimePlugin)), 
                new EventSourceViewModel("Outlook", typeof(OutlookPlugin)), 
                new EventSourceViewModel("Team Foundation Server", typeof(TeamFoundationServerPlugin)), 
                new EventSourceViewModel("Yammer", typeof(YammerPlugin)), 
            };
            m_endLocalTime = DateTime.Now;
            m_reportingSinks = new[] 
            { 
                new ReportingSinkViewModel("Console", typeof(ConsoleWriteEvents)) { IsSelected = true },
                new ReportingSinkViewModel("Excel", typeof(ExcelWriteEvents)), 
                new ReportingSinkViewModel("Web page", typeof(HtmlWriteEvents)), 
            };
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
        
        public IEnumerable<EventSourceViewModel> EventSources
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

        public IEnumerable<ReportingSinkViewModel> ReportingSinks
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

            var selectedEventSourceTypes = EventSources.Where(p => p.IsSelected)
                                                       .Select(p => p.EventSourceType)
                                                       .ToList();

            var selectedReportingSinkTypes = ReportingSinks.Where(p => p.IsSelected)
                                                           .Select(p => p.ReportingSinkType)
                                                           .ToList();

            var selectedStartLocalTime = m_startLocalTime;
            var selectedEndLocalTime = m_endLocalTime;

            if (selectedEventSourceTypes.Any() && selectedReportingSinkTypes.Any())
            {
                DispatcherFrame frame = new DispatcherFrame();
                Dispatcher.CurrentDispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    new DispatcherOperationCallback(o =>
                    {
                        frame.Continue = false;

                        try
                        {
                            var pluginRuntime = new PluginRuntime();
                            pluginRuntime.Start(selectedEventSourceTypes);

                            foreach (var eventQueryServiceRegistration in pluginRuntime.EventQueryServices)
                            {
                                Console.WriteLine("Querying from event query service: " + eventQueryServiceRegistration.Key);
                                var evts = eventQueryServiceRegistration.Value.PullEvents(selectedStartLocalTime, selectedEndLocalTime);

                                // TODO renderers as plugins
                                foreach (IRenderEvents render in selectedReportingSinkTypes.Select(Activator.CreateInstance))
                                {
                                    render.Render(eventQueryServiceRegistration.Key, evts);
                                }

                                Console.WriteLine();
                            }

                            if (!pluginRuntime.EventQueryServices.Any())
                            {
                                Console.WriteLine("No event query services registered!");
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
