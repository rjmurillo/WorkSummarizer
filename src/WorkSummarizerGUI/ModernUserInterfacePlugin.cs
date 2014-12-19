using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkSummarizerGUI
{
    using Extensibility;

    using WorkSummarizerGUI.ViewModels;

    public class ModernUserInterfaceSettingConstants
    {
        public const string ThemeAccent = "MUI.Theme.Accent";
        public const string ThemeBackground = "MUI.Theme.Background";
    }

    internal class ModernUserInterfacePlugin 
    {
        public ModernUserInterfacePlugin(IPluginContext pluginContext)
        {
            var runtime = new ModernUserInterfaceRuntime();
            pluginContext.RegisterService<ModernUserInterfaceRuntime>(runtime, new ServiceRegistration("MUI.Runtime", "Modern User Interface", "Runtime"));

            ViewModelLocator.RegisterService(runtime);
        }
    }

    internal class ModernUserInterfaceRuntime : IConfigurable
    {
        private readonly IDictionary<string, ConfigurationSetting> m_settings;

        private readonly ConfigurationSetting m_themeAccentSetting;

        private readonly ConfigurationSetting m_themeBackgroundSetting;

        public ModernUserInterfaceRuntime()
        {
            m_themeAccentSetting = new ConfigurationSetting(ModernUserInterfaceSettingConstants.ThemeAccent, "Sienna")
                {
                    Name = "Theme Accent",
                    Description = "The default theme accent to use on startup."
                };

            m_themeBackgroundSetting = new ConfigurationSetting(
                ModernUserInterfaceSettingConstants.ThemeBackground, "Light") 
                { 
                    Name = "Theme Background", 
                    Description = "The default theme background to use on startup." 
                };

            m_settings = new[] { m_themeAccentSetting, m_themeBackgroundSetting }.ToDictionary(p => p.Key);
        }

        public IEnumerable<ConfigurationSetting> Settings
        {
            get
            {
                return m_settings.Values;
            }
        }

        public ConfigurationSetting ThemeAccentSetting
        {
            get
            {
                return m_themeAccentSetting;
            }
        }

        public ConfigurationSetting ThemeBackgroundSetting
        {
            get
            {
                return m_themeBackgroundSetting;
            }
        }
    }
}
