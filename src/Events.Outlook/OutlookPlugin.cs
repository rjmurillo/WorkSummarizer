using Extensibility;

namespace Events.Outlook
{
    public class OutlookSettingConstants
    {
        public const string IncludeOnlyMailAliases = "Outlook.Mail.IncludeOnlyAliases";
        public const string IncludeOnlyMeetingAliases = "Outlook.Meetings.IncludeOnlyAliases";
        public const string IncludeOnlyConversationHistoryAliases = "Outlook.ConversationHistory.IncludeOnlyAliases";
    }

    public class OutlookPlugin
    {
        public OutlookPlugin(IPluginContext pluginContext)
        {
            pluginContext.RegisterService<IEventQueryService>(new OutlookEmailEventQueryService(), new ServiceRegistration("Outlook.Mail", "Outlook", "Sent"));
            pluginContext.RegisterService<IEventQueryService>(new OutlookMeetingEventQueryService(), new ServiceRegistration("Outlook.Meetings", "Outlook", "Meetings"));
            pluginContext.RegisterService<IEventQueryService>(new OutlookConversationHistoryEventQueryService(), new ServiceRegistration("Outlook.ConversationHistory", "Outlook", "Conversation History"));
        }
    }
}
