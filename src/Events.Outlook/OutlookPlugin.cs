using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensibility;

namespace Events.Outlook
{
    public class OutlookPlugin
    {
        public OutlookPlugin(IPluginRuntime pluginRuntime)
        {
            pluginRuntime.EventQueryServices[new ServiceRegistration("Outlook.Mail", "Outlook", "Sent")] = new OutlookEmailEventQueryService();
            pluginRuntime.EventQueryServices[new ServiceRegistration("Outlook.Meetings", "Outlook", "Meetings")] = new OutlookMeetingEventQueryService();
            pluginRuntime.EventQueryServices[new ServiceRegistration("Outlook.ConversationHistory", "Outlook", "Conversation History")] = new OutlookConversationHistoryEventQueryService();
        }
    }
}
