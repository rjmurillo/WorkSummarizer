using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Events;

namespace Renders
{
    public interface IRenderEvents
    {
        void Render(string eventType, DateTime startDateTime, DateTime endDateTime, IEnumerable<Event> events, IDictionary<string, int> weightedTags, IDictionary<string, int> weightedPeople, IEnumerable<string> importantSentences);
    }
}
