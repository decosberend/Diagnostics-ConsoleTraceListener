using System;
using System.Collections.Generic;
using System.Linq;

using Slack.Webhooks;

namespace Decos.Diagnostics.Trace.Slack
{
    internal static class LogEntryExtensions
    {
        public static SlackMessage ToSlackMessage(this LogEntry logEntry)
        {
            var logAttachment = new SlackAttachment
            {
                AuthorName = logEntry.Source,
                Color = logEntry.Level.GetLogLevelColor(),
                Text = logEntry.Message,
                Fields = new List<SlackField>(logEntry.GetDataFields()),
                Footer = $"{logEntry.Level} - {logEntry.HostName} - PID {logEntry.ProcessId} - thread {logEntry.ThreadId}"
            };

            if (string.IsNullOrEmpty(logEntry.Message) && logEntry.Data != null)
            {
                logAttachment.Fallback = $"{logEntry.Data}";
            }

            return new SlackMessage
            {
                Attachments = new List<SlackAttachment> { logAttachment },
                Markdown = true
            };
        }

        private static IEnumerable<SlackField> GetDataFields(this LogEntry logEntry)
        {
            var data = logEntry.Data;
            if (data == null)
                yield break;

            if (data is Exception ex)
            {
                yield return new SlackField { Title = $"{ex.GetType()}", Value = ex.Message, Short = false };
                yield return new SlackField { Title = "Stack trace", Value = $"{ex.StackTrace}", Short = false };
            }
            else
            {
                var type = data.GetType();
                foreach (var property in type.GetProperties().Where(x => x.CanRead))
                {
                    var value = property.GetValue(data);
                    yield return new SlackField { Title = property.Name, Value = FormatData(property.PropertyType, value), Short = true };
                }
            }
        }

        private static string FormatData(Type propertyType, object value)
        {
            if (propertyType == typeof(DateTime))
            {
                var date = new DateTimeOffset((DateTime)value);
                return FormatSlackDate(date);
            }
            else if (propertyType == typeof(DateTimeOffset))
            {
                var date = (DateTimeOffset)value;
                return FormatSlackDate(date);
            }

            return $"{value}";
        }

        private static string FormatSlackDate(DateTimeOffset date)
        {
            var unixTime = date.ToUnixTimeSeconds();
            if (date.TimeOfDay.Ticks == 0)
                return $"<!date^{unixTime}^{{date_pretty}}|{date}>";
            return $"<!date^{unixTime}^{{date_pretty}} at {{time_secs}}|{date}>";
        }
    }
}