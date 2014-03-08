namespace WorkSummarizer
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Linq;
    using Extensibility;

    public interface IConfigurationService
    {
        string GetValueOrDefault(string key);

        IEnumerable<ConfigurationSetting> Settings { get; }
    }

    internal class MemoryConfigurationService : IConfigurationService
    {
        private readonly IDictionary<string, ConfigurationSetting> m_configuration;

        public MemoryConfigurationService(IEnumerable<ConfigurationSetting> defaultConfiguration)
        {
            m_configuration = defaultConfiguration.ToDictionary(p => p.Key);
        }

        public IEnumerable<ConfigurationSetting> Settings
        {
            get
            {
                return m_configuration.Values;
            }
        }

        public string GetValueOrDefault(string key)
        {
            ConfigurationSetting returnValue;
            if (m_configuration.TryGetValue(key, out returnValue))
            {
                return returnValue.Value;
            }

            return default(string);
        }
    }

    internal class FileConfigurationService : MemoryConfigurationService
    {
        public FileConfigurationService(IEnumerable<ConfigurationSetting> defaultConfiguration) 
            : base(defaultConfiguration)
        {
            var configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None); 
            
            foreach (var defaultSetting in defaultConfiguration)
            {
                var fileConfig = configuration.AppSettings.Settings[defaultSetting.Key];
                if (fileConfig != null)
                {
                    defaultSetting.Value = fileConfig.Value;
                }

                defaultSetting.ValueChanged += (sender, args) =>
                                               {
                                                   try
                                                   {
                                                       configuration.AppSettings.Settings.Remove(defaultSetting.Key);
                                                       configuration.AppSettings.Settings.Add(defaultSetting.Key, defaultSetting.Value);
                                                       configuration.Save(ConfigurationSaveMode.Modified);
                                                   }
                                                   catch (ConfigurationErrorsException ex)
                                                   {
                                                       Trace.WriteLine(ex);
                                                   }
                                               };
            }
        }
    }
}
