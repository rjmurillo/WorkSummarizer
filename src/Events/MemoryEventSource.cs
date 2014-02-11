using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events
{
    public class MemoryEventSource
    {
        public IEnumerable<Event> GetEvents()
        {
            return Enumerable.Empty<Event>();
        }
    }
}
