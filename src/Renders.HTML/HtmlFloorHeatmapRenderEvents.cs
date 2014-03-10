using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Renders.HTML
{
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using Events;
    using Extensibility;
    using TagCloud;

    public class HtmlFloorHeatmapRenderEvents : IRenderEvents, IConfigurable
    {
        private readonly IDictionary<string, ConfigurationSetting> m_settings;

        public HtmlFloorHeatmapRenderEvents()
        {
            m_settings = new[]
                         {
                             new ConfigurationSetting(HtmlRenderSettingConstants.FloorHeatmapOutputDirectory,
                                 Environment.GetFolderPath(Environment.SpecialFolder.Desktop))
                             {
                                 Name = "Output Directory",
                                 Description = "Location where output should be generated."
                             }
                         }.ToDictionary(p => p.Key);
        }

        public IEnumerable<ConfigurationSetting> Settings { get { return m_settings.Values; } }

        public void Render(string eventType, DateTime startDateTime, DateTime endDateTime, IEnumerable<Event> events, IDictionary<string, int> weightedTags,
            IDictionary<string, int> weightedPeople, IEnumerable<string> importantSentences)
        {
            // weightedPeople = new Dictionary<string, int> { { "jocarrie", 5 }, { "edkotows", 8 }, { "benb", 1 }, { "rimuri", 500 }, { "alspiri", 5 }, { "mattwest", 5 }, };
            var sb = new StringBuilder();
            sb.AppendLine("<script>");
            var orderedWeightedPeople = weightedPeople.OrderByDescending(p => p.Value).GroupBy(p => p.Value).ToList();
            var partitionSize = (int)Math.Ceiling((double)orderedWeightedPeople.Count/7);
            for(int i = 0; i < orderedWeightedPeople.Count; i++)
            {
                var bucket = (int)Math.Ceiling((double)i/partitionSize);
                var people = orderedWeightedPeople.ElementAt(i);

                foreach (var person in people)
                {
                    sb.AppendLine(string.Format("scores[\"{0}\"] = {1};", person.Key.Replace("\"", "'"), bucket + 1));
                }
            }
            sb.AppendLine(string.Format("scores[\"{0}\"] = {1};", Environment.UserName.Split('\\').LastOrDefault(), -1));
            sb.AppendLine("render();</script>");

            var htmlBoilerplate = LoadResource("boilerplate-floorheatmap.html");

            string fileName = string.Format("WorkSummarizer_FloorHeatmap_{0}.html", eventType);
            fileName = Path.Combine(m_settings[HtmlRenderSettingConstants.FloorHeatmapOutputDirectory].Value, fileName);
            File.WriteAllText(fileName, htmlBoilerplate);
            File.AppendAllText(fileName, sb.ToString());
            Process.Start(fileName);
        }

        private static string LoadResource(string name)
        {
            var a = Assembly.GetExecutingAssembly();
            using (var s = a.GetManifestResourceStream("Renders.HTML." + name))
            {
                Debug.Assert(s != null);
                using (var r = new StreamReader(s))
                {
                    return r.ReadToEnd();
                }
            }
        }
    }
}
