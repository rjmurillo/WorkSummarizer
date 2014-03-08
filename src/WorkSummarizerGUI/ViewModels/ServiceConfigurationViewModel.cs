using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Extensibility;
using WorkSummarizerGUI.Commands;

namespace WorkSummarizerGUI.ViewModels
{
    using WorkSummarizer;

    public class ServiceConfigurationViewModel : ViewModelBase
    {
        public ServiceConfigurationViewModel(string name, IConfigurationService configurationService)
        {
            Name = name;
            Settings = configurationService.Settings.Select(p => new ConfigurationSettingViewModel(p)).ToList();
            RevertToDefaultCommand = new RelayCommand(() => { foreach (var setting in Settings) { setting.RevertToDefaultCommand.Execute(null); } });
        }

        public string Name { get; private set; }

        public IEnumerable<ConfigurationSettingViewModel> Settings { get; private set; }

        public ICommand RevertToDefaultCommand
        {
            get;
            private set;
        }
    }

    public class ConfigurationSettingViewModel : ViewModelBase
    {        
        private readonly ConfigurationSetting m_setting;

        public ConfigurationSettingViewModel(ConfigurationSetting setting)
        {
            m_setting = setting;
            RevertToDefaultCommand = new RelayCommand(() => Value = DefaultValue);
        }

        public string DefaultValue
        {
            get { return m_setting.DefaultValue; }
        }
        
        public string Description
        {
            get { return m_setting.Description; }
        }

        public string Name
        {
            get { return m_setting.Name; }
        }
        
        public string Value
        {
            get { return m_setting.Value; }
            set { m_setting.Value = value; OnPropertyChanged(); }
        }

        public ICommand RevertToDefaultCommand
        {
            get; private set;
        }
    }
}
