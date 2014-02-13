using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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
    }
}
