using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Events.Yammer;
using Extensibility;
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
            m_eventSources = new[] {new EventSourceViewModel(), new EventSourceViewModel()};
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
            var plugins = new List<Type>();

            //plugins.Add(typeof(FakeEventsPlugin));
            // plugins.Add(typeof(CodeFlowPlugin));
            // plugins.Add(typeof(ConnectPlugin));
            // plugins.Add(typeof(KudosPlugin));
            // plugins.Add(typeof(ManicTimePlugin));
            // plugins.Add(typeof(OutlookPlugin));
            // plugins.Add(typeof(TeamFoundationServerPlugin));
            plugins.Add(typeof(YammerPlugin));

            var pluginRuntime = new PluginRuntime();

            var renders = new List<IRenderEvents>();
            renders.Add(new ExcelWriteEvents());

            pluginRuntime.Start(plugins);

            foreach (var eventQueryServiceRegistration in pluginRuntime.EventQueryServices)
            {
                Console.WriteLine("Querying from event query service: " + eventQueryServiceRegistration.Key);
                var evts = eventQueryServiceRegistration.Value.PullEvents(new DateTime(2014, 1, 1), new DateTime(2014, 2, 14));

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

            Console.WriteLine("Done.");
        }
    }
}
