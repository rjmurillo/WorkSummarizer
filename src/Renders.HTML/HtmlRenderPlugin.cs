using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensibility;

namespace Renders.HTML
{
    public class HtmlRenderSettingConstants
    {
        public const string OutputDirectory = "HTML.OutputDirectory";
    }

    public class HtmlRenderPlugin
    {
        public HtmlRenderPlugin(IPluginRuntime pluginRuntime)
        {
            var htmlConfigurationService = new DefaultConfigurationService(new[]
            {
                new ConfigurationSetting(HtmlRenderSettingConstants.OutputDirectory, Directory.GetCurrentDirectory())
                { 
                    Name = "Output Directory", 
                    Description = "Location where output should be generated." 
                }
            });

            pluginRuntime.ConfigurationServices[new ServiceRegistration("HTML", "HTML", "File")] = htmlConfigurationService; // REVIEW not depend on configuration service id to line up with another service

            pluginRuntime.RenderEventServices[new ServiceRegistration("HTML", "HTML", "File") { IsConfigurable = true }] = new HtmlWriteEvents(htmlConfigurationService);
        }
    }
}
