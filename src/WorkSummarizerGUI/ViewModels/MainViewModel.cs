using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WorkSummarizerGUI.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
    public class MainViewModel : ViewModelBase
    {
        private DateTime m_endLocalTime;
        private readonly IEnumerable<EventSourceViewModel> m_eventSources;
        private bool m_isBusy;
        private DateTime m_startLocalTime;

        public MainViewModel()
        {
            m_eventSources = new[] {new EventSourceViewModel(), new EventSourceViewModel()};
            m_isBusy = true;
            m_endLocalTime = DateTime.Now;
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

    public class EventSourceViewModel : ViewModelBase
    {
        private readonly string m_name;

        public EventSourceViewModel()
        {
            m_name = "Jocarrie";
        }

        public string Name
        {
            get { return m_name; }
        }
    }
}
