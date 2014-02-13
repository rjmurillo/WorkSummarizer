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
        void WriteOut(IEnumerable<Event> events);
    }
}
