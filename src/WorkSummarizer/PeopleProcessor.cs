using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Events;

namespace WorkSummarizer
{
    internal class PeopleProcessor
    {
        public IDictionary<string, int> GetTeam(IEnumerable<Event> events)
        {
            var p = events
                    .SelectMany(s => s.Participants.Nodes)
                    .Select(s => s.Value.Alias)
                    .GroupBy(c=>c)
                    .ToDictionary(ks=>ks.Key, e=>e.Count());

            return p;
        }
    }
}
