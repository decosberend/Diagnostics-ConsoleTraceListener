using System;

namespace Decos.Diagnostics.Trace
{
    /// <summary>
    /// Represents a trace listener that sends logging events and data to a
    /// Logstash HTTP input plugin.
    /// </summary>
    public class LogstashHttpTraceListener : TraceListenerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="LogstashHttpTraceListener"/> using the specified endpoint.
        /// </summary>
        /// <param name="endpointAddress">
        /// The Logstash HTTP input plugin URL.
        /// </param>
        public LogstashHttpTraceListener(string endpointAddress)
            : this(new Uri(endpointAddress, UriKind.Absolute))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="LogstashHttpTraceListener"/> using the specified endpoint.
        /// </summary>
        /// <param name="endpointAddress">
        /// The Logstash HTTP input plugin URL.
        /// </param>
        public LogstashHttpTraceListener(Uri endpointAddress)
        {
            LogstashClient = new HttpLogstashClient(endpointAddress);
        }

        /// <summary>
        /// Gets a reference to the client used to send log entries to Logstash.
        /// </summary>
        protected HttpLogstashClient LogstashClient { get; }

        /// <summary>
        /// Writes trace information and a message to Logstash.
        /// </summary>
        /// <param name="e">
        /// A <see cref="TraceListenerBase.TraceEventData"/> object that contains
        /// information about the trace event.
        /// </param>
        /// <param name="message">The message to write.</param>
        protected override void TraceInternal(TraceEventData e, string message)
        {
            var logEntry = new LogEntry
            {
                Level = e.Type.ToLogLevel(),
                Source = e.Source,
                Message = message,
                EventId = e.ID,
                ProcessId = e.Cache.ProcessId,
                ThreadId = e.Cache.ThreadId
            };

            WriteLogEntry(logEntry);
        }

        private void WriteLogEntry(LogEntry logEntry)
        {
            LogstashClient.LogAsync(logEntry).GetAwaiter().GetResult();
        }
    }
}