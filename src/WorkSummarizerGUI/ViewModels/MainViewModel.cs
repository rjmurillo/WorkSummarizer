using System;
using System.Collections.Generic;
using System.Linq;
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
using Renders.Excel;
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
            m_reportingSinks = new[] { new ReportingSinkViewModel(), new ReportingSinkViewModel() };
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

        public void Generate()
        {
            IsBusy = true;

            var selectedEventSourceTypes = EventSources.Where(p => p.IsSelected)
                                                       .Select(p => p.EventSourceType)
                                                       .ToList();

            if (selectedEventSourceTypes.Any())
            {
                DispatcherFrame frame = new DispatcherFrame();
                Dispatcher.CurrentDispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    new DispatcherOperationCallback(o =>
                    {
                        frame.Continue = false;

                        var pluginRuntime = new PluginRuntime();

                        var renders = new List<IRenderEvents>();
                        renders.Add(new ExcelWriteEvents());

                        pluginRuntime.Start(selectedEventSourceTypes);

                        foreach (var eventQueryServiceRegistration in pluginRuntime.EventQueryServices)
                        {
                            Console.WriteLine("Querying from event query service: " + eventQueryServiceRegistration.Key);
                            var evts = eventQueryServiceRegistration.Value.PullEvents(new DateTime(2014, 1, 1),
                                new DateTime(2014, 2, 14));

                            foreach (IRenderEvents render in renders)
                            {
                                render.WriteOut(eventQueryServiceRegistration.Key, evts);
                            }

                            Console.WriteLine();
                        }

                        if (!pluginRuntime.EventQueryServices.Any())
                        {
                            Console.WriteLine("No event query services registered!");
                        }

                        return null;
                    }), frame);
                Dispatcher.PushFrame(frame);
            }

            Console.WriteLine("Done.");
            IsBusy = false;
        }
    }
}
