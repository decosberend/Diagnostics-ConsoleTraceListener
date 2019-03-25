using System;
using System.Reflection;

using Slack.Webhooks;

namespace Decos.Diagnostics.Trace.Slack
{
    internal static class SlackExtensions
    {
        public static string ToSlackDate(this DateTimeOffset date)
        {
            if (date.TimeOfDay.Ticks == 0)
                return date.ToSlackDate("{{date_pretty}}", "d MMM y");
            return date.ToSlackDate("{{date_pretty}} at {{time_secs}}", "d MMM y 'at' HH:mm:ssK");
        }

        public static string ToSlackDate(this DateTimeOffset date, string displayFormat, string fallbackFormat)
        {
            var unixTime = date.ToUnixTimeSeconds();
            return $"<!date^{unixTime}^{displayFormat}|{date.ToString(fallbackFormat)}>";
        }

        public static SlackField GetValueAsSlackField(this PropertyInfo property, object obj, bool @short = true)
        {
            var value = property.GetValue(obj);
            if (value != null)
            {
                var formattedValue = FormatFieldValue(value);
                if (!string.IsNullOrEmpty(formattedValue))
                {
                    return new SlackField
                    {
                        Title = property.Name,
                        Value = formattedValue,
                        Short = @short
                    };
                }
            }

            return null;
        }

        private static string FormatFieldValue(object value)
        {
            if (value is DateTime dateTime)
            {
                var date = new DateTimeOffset(dateTime);
                return date.ToSlackDate();
            }
            else if (value is DateTimeOffset dateTimeOffset)
            {
                return dateTimeOffset.ToSlackDate();
            }

            var formattedValue = $"{value}";
            if (formattedValue != value.GetType().ToString())
                return formattedValue;

            return null;
        }
    }
}