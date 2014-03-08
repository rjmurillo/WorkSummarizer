namespace WorkSummarizer
{
    using System.Collections.Generic;
    using System.Linq;
    using Extensibility;

    public interface IConfigurationService
    {
        string GetValueOrDefault(string key);

        IEnumerable<ConfigurationSetting> Settings { get; }
    }

    public class DefaultConfigurationService : IConfigurationService
    {
        private readonly IDictionary<string, ConfigurationSetting> m_configuration;

        public DefaultConfigurationService(IEnumerable<ConfigurationSetting> defaultConfiguration)
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
}
