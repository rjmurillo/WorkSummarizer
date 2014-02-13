﻿using System;
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
            pluginRuntime.EventQueryServices["Outlook.Mail"] = new OutlookEmailEventQueryService();
            pluginRuntime.EventQueryServices["Outlook.Meetings"] = new OutlookMeetingEventQueryService();
        }
    }
}
