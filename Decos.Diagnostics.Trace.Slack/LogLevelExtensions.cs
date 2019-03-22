using System;
using System.Collections.Generic;
using System.Text;

namespace Decos.Diagnostics.Trace.Slack
{
    internal static class LogLevelExtensions
    {
        private static readonly IDictionary<LogLevel, string> logLevelColors
            = new Dictionary<LogLevel, string>()
            {
                [LogLevel.Critical] = "danger",
                [LogLevel.Error] = "danger",
                [LogLevel.Warning] = "warning",
                [LogLevel.Information] = "good"
            };

        public static string GetLogLevelColor(this LogLevel logLevel)
        {
            if (logLevelColors.TryGetValue(logLevel, out var colorValue))
                return colorValue;
            return string.Empty;
        }
    }
}
