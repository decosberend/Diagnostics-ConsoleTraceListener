using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Decos.Diagnostics.Trace
{
    public class TraceSourceLog : ILog
    {
        public TraceSourceLog(TraceSource traceSource)
        {
            TraceSource = traceSource;
        }

        public TraceSource TraceSource { get; }

        public bool IsEnabled(LogLevel logLevel)
        {
            var eventType = logLevel.ToTraceEventType();
            if (eventType == null)
                return false;

            return TraceSource.Switch.ShouldTrace(eventType.Value);
        }

        public void Write(LogLevel logLevel, string message)
        {
            var eventType = logLevel.ToTraceEventType();
            if (eventType == null)
                return;
            
            TraceSource.TraceEvent(eventType.Value, 0, message);
        }

        public void Write<T>(LogLevel logLevel, T data)
        {
            var eventType = logLevel.ToTraceEventType();
            if (eventType == null)
                return;

            TraceSource.TraceData(eventType.Value, 0, data);
        }
    }
}
