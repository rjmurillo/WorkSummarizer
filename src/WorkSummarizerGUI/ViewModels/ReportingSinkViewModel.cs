
using System;

namespace WorkSummarizerGUI.ViewModels
{
    public class ReportingSinkViewModel : ViewModelBase
    {
        private readonly string m_name;
        private readonly Type m_reportingSinkType;
        private bool m_isEnabled;
        private bool m_isSelected;
        
        public ReportingSinkViewModel(string name, Type reportingSinkType)
        {
            m_name = name;
            m_reportingSinkType = reportingSinkType;
            m_isEnabled = true;
        }
        
        public bool IsEnabled
        {
            get
            {
                return m_isEnabled;
            }
            set 
            { 
                m_isEnabled = value;
                OnPropertyChanged();
            }
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

        public Type ReportingSinkType
        {
            get { return m_reportingSinkType; }
        }
    }
}
