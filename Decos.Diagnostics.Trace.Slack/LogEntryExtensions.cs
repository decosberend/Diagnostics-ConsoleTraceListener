using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Slack.Webhooks;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Decos.Diagnostics.Trace.Slack.Tests")]
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
                Footer = $"{logEntry.Level} - {logEntry.HostName} - PID {logEntry.ProcessId} thread {logEntry.ThreadId}",
                Timestamp = (int)logEntry.Timestamp.ToUnixTimeSeconds()
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
                foreach (var dataField in GetExceptionDataFields(ex))
                    yield return dataField;

                var inner = ex.InnerException;
                while (inner != null)
                {
                    yield return new SlackField { Title = $"---> {inner.GetType()}", Value = inner.Message, Short = false };
                    foreach (var innerField in GetExceptionDataFields(inner))
                        yield return innerField;

                    inner = inner.InnerException;
                }
            }
            else
            {
                var type = data.GetType();
                foreach (var property in type.GetProperties().Where(x => x.CanRead))
                {
                    var field = property.GetValueAsSlackField(data);
                    if (field != null)
                        yield return field;
                }
            }
        }

        private static IEnumerable<SlackField> GetExceptionDataFields(Exception ex)
        {
            var type = ex.GetType();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var property in properties.Where(x => x.CanRead && !IsOverridden(x)))
            {
                var field = property.GetValueAsSlackField(ex);
                if (field != null)
                    yield return field;
            }
        }

        private static bool IsOverridden(PropertyInfo property)
        {
            var getter = property.GetGetMethod(nonPublic: false);
            var baseGetter = getter.GetBaseDefinition();
            return baseGetter.DeclaringType != getter.DeclaringType;
        }
    }
}