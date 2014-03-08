using System.Collections.Generic;

namespace Extensibility
{
    using System;

    public interface IConfigurable
    {
        IEnumerable<ConfigurationSetting> Settings { get; }
    }

    public class ConfigurationSetting
    {
        private string m_value;

        public ConfigurationSetting(string key, string defaultValue)
        {
            Key = key;
            Value = DefaultValue = defaultValue;
        }

        public event EventHandler ValueChanged;

        public string DefaultValue { get; private set; }
        public string Description { get; set; }
        public string Key { get; private set; }
        public string Name { get; set; }

        public string Value
        {
            get { return m_value; }
            set
            {
                if (Equals(m_value, value))
                {
                    return;
                }

                m_value = value;
                if (ValueChanged != null)
                {
                    ValueChanged(this, EventArgs.Empty);
                }
            }
        }
    }
}
