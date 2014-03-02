using System;
using System.IO;
using Extensibility;

namespace Events.ManicTime
{
    public class ManicTimeSettingConstants
    {
        public const string ActivitiesDatabaseFile = "ManicTime.Activities.DatabaseFile";
    }

    public class ManicTimePlugin
    {
        public ManicTimePlugin(IPluginRuntime pluginRuntime)
        {
            var manicTimeDbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "..\\Local\\Finkit\\ManicTime");
            if (!Directory.Exists(manicTimeDbPath))
            {
                manicTimeDbPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }

            var manicTimeDbFile = Path.Combine(manicTimeDbPath, "ManicTime.sdf");
            var activitiesConfigurationService = new DefaultConfigurationService(new[]
            {
                new ConfigurationSetting(ManicTimeSettingConstants.ActivitiesDatabaseFile, manicTimeDbFile) 
                { 
                    Name = "Database File Location", 
                    Description = "Path to the *.sdf database." 
                }
            });

            pluginRuntime.ConfigurationServices[new ServiceRegistration("ManicTime.Activities", "ManicTime", "Activities")] = activitiesConfigurationService;

            pluginRuntime.EventQueryServices[new ServiceRegistration("ManicTime.Activities", "ManicTime", "Activities") { IsConfigurable = true }] = new ManicTimeActivityEventQueryService(activitiesConfigurationService);
        }
    }
}
