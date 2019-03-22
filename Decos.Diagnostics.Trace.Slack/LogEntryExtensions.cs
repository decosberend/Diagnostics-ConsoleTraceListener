using System;
using System.Collections.Generic;
using System.Text;
using Slack.Webhooks;

namespace Decos.Diagnostics.Trace.Slack
{
    internal static class LogEntryExtensions
    {
        public static SlackMessage ToSlackMessage(this LogEntry logEntry)
        {
            return new SlackMessage
            {
                Text = logEntry.Message
            };
        }
    }
}
