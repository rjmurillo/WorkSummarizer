using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace WorkSummarizerGUI.ViewModels
{
    public class ServiceViewModel : ViewModelBase
    {
        private readonly bool m_isConfigurable;
        private readonly IEnumerable<string> m_serviceIds;
        private readonly string m_name;
        private readonly ICommand m_configureCommand;
        private string m_helpText;
        private bool m_isSelected;
        
        public ServiceViewModel(string name, IEnumerable<string> serviceIds, ICommand configureCommand)
        {
            m_serviceIds = serviceIds;
            m_name = name;
            m_helpText = String.Empty;
            m_isConfigurable = configureCommand.CanExecute(null);
            m_configureCommand = configureCommand;
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
        
        public ICommand ConfigureCommand
        {
            get { return m_configureCommand; }
        }

        public bool IsConfigurable
        {
            get { return m_isConfigurable; }
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
