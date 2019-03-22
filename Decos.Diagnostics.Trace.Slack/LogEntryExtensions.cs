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
                Attachments = new List<SlackAttachment>
                {
                    new SlackAttachment
                    {
                        Text = logEntry.Message,
                        AuthorName = logEntry.Source,
                        Color = logEntry.Level.GetLogLevelColor(),
                        Fields = new List<SlackField>
                        {
                        },
                        Footer = $"{logEntry.Level} - {logEntry.HostName} - PID {logEntry.ProcessId} - thread {logEntry.ThreadId}"
                    }
                }
            };
        }
    }
}
