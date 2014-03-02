using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Events;

namespace WorkSummarizer
{
    public class PeopleProcessor
    {
        public IDictionary<string, int> GetTeam(IEnumerable<Event> events)
        {
            var p = events
                    .SelectMany(s => s.Participants.Nodes)
                    .Select(s => s.Value.Alias)
                    .Where(predicate=>!predicate.Equals(Environment.UserName))
                    .GroupBy(c=>c)
                    .ToDictionary(ks=>ks.Key, e=>e.Count());

            return p;
        }
    }
}
