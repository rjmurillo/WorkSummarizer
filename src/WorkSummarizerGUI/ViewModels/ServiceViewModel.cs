
using System;
using System.Collections;
using System.Collections.Generic;

namespace WorkSummarizerGUI.ViewModels
{
    public class ServiceViewModel : ViewModelBase
    {
        private readonly IEnumerable<string> m_serviceIds;
        private readonly string m_name;
        private string m_helpText;
        private bool m_isEnabled;
        private bool m_isSelected;

        public ServiceViewModel(string name, IEnumerable<string> serviceIds)
        {
            m_serviceIds = serviceIds;
            m_name = name;
            m_helpText = String.Empty;
            m_isEnabled = true;
        }

        public string HelpText
        {
            get { return m_helpText; }
            set
            {
                m_helpText = value;
                OnPropertyChanged();
            }
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

        public IEnumerable<string> ServiceIds
        {
            get { return m_serviceIds; }
        }
    }
}
