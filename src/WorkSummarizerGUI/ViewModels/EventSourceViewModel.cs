
using System;

namespace WorkSummarizerGUI.ViewModels
{
    public class EventSourceViewModel : ViewModelBase
    {
        private readonly Type m_eventSourceType;
        private readonly string m_name;
        private bool m_isSelected;

        public EventSourceViewModel(string name, Type eventSourceType)
        {
            m_eventSourceType = eventSourceType;
            m_name = name;
        }

        public Type EventSourceType
        {
            get { return m_eventSourceType; }
        }

        public bool IsSelected
        {
            get 
            { 
                return m_isSelected; 
            }
            set 
            { 
                m_isSelected = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get { return m_name; }
        }
    }
}
