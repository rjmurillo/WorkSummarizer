﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensibility
{
    public class ConfigurationSetting
    {
        public ConfigurationSetting(string key, string defaultValue)
        {
            Key = key;
            Value = DefaultValue = defaultValue;
        }

        public string DefaultValue { get; private set; }
        public string Description { get; set; }
        public string Key { get; private set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public interface IConfigurationService
    {
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
    }
}
