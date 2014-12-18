
using System.Collections.Generic;
namespace WorkSummarizerGUI.Models
{
    public class ServiceConfigurationRequest
    {
        public string Name { get; set; }

        public IEnumerable<string> Ids { get; set; }
    }
}
