using System.Collections.Generic;

namespace Extensibility
{
    public interface IConfigurable
    {
        IEnumerable<ConfigurationSetting> Settings { get; }
    }

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
}
