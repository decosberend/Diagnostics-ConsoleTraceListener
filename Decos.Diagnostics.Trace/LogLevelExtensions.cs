using System.Collections.Generic;
using System.Diagnostics;

namespace Decos.Diagnostics.Trace
{
    internal static class LogLevelExtensions
    {
        private static readonly IDictionary<LogLevel, TraceEventType> logLevelTypes = new Dictionary<LogLevel, TraceEventType>()
        {
            [LogLevel.Debug] = TraceEventType.Verbose,
            [LogLevel.Information] = TraceEventType.Information,
            [LogLevel.Warning] = TraceEventType.Warning,
            [LogLevel.Error] = TraceEventType.Error,
            [LogLevel.Critical] = TraceEventType.Critical
        };

        private static readonly IDictionary<LogLevel, SourceLevels> logLevelSwitchValues = new Dictionary<LogLevel, SourceLevels>()
        {
            [LogLevel.None] = SourceLevels.Off,
            [LogLevel.Debug] = SourceLevels.Verbose,
            [LogLevel.Information] = SourceLevels.Information,
            [LogLevel.Warning] = SourceLevels.Warning,
            [LogLevel.Error] = SourceLevels.Error,
            [LogLevel.Critical] = SourceLevels.Critical
        };

        private static readonly IDictionary<TraceEventType, LogLevel> eventTypeLevels = new Dictionary<TraceEventType, LogLevel>()
        {
            [TraceEventType.Verbose] = LogLevel.Debug,
            [TraceEventType.Information] = LogLevel.Information,
            [TraceEventType.Warning] = LogLevel.Warning,
            [TraceEventType.Error] = LogLevel.Error,
            [TraceEventType.Critical] = LogLevel.Critical
        };

        public static TraceEventType? ToTraceEventType(this LogLevel logLevel)
        {
            if (logLevelTypes.TryGetValue(logLevel, out var value))
                return value;

            return null;
        }

        public static SourceLevels ToSourceLevels(this LogLevel logLevel)
        {
            if (logLevelSwitchValues.TryGetValue(logLevel, out var value))
                return value;

            return SourceLevels.Off;
        }

        public static LogLevel ToLogLevel(this TraceEventType traceEventType)
        {
            if (eventTypeLevels.TryGetValue(traceEventType, out var value))
                return value;

            return LogLevel.None;
        }
    }
}
