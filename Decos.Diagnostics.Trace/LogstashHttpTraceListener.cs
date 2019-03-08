using System;

namespace Decos.Diagnostics.Trace
{
    /// <summary>
    /// Represents a trace listener that sends logging events and data to a Logstash HTTP input plugin.
    /// </summary>
    public class LogstashHttpTraceListener : TraceListenerBase
    {
        private static readonly Uri defaultEndpoint = new Uri("http://log.decos.nl:9090");

        /// <summary>
        /// Initializes a new instance of the <see cref="LogstashHttpTraceListener"/> using the default production endpoint.
        /// </summary>
        public LogstashHttpTraceListener()
        {
            LogstashClient = new HttpLogstashClient(defaultEndpoint);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogstashHttpTraceListener"/> using the specified endpoint.
        /// </summary>
        /// <param name="endpointAddress">The Logstash HTTP input plugin URL.</param>
        public LogstashHttpTraceListener(string endpointAddress)
        {
            var endpoint = new Uri(endpointAddress, UriKind.Absolute);
            LogstashClient = new HttpLogstashClient(endpoint);
        }

        /// <summary>
        /// Gets a reference to the client used to send log entries to Logstash.
        /// </summary>
        protected HttpLogstashClient LogstashClient { get; }

        /// <summary>
        /// Writes trace information and a message to Logstash.
        /// </summary>
        /// <param name="e">
        /// A <see cref="TraceListenerBase.TraceEventData" /> object that contains information about
        /// the trace event.
        /// </param>
        /// <param name="message">The message to write.</param>
        protected override void TraceInternal(TraceEventData e, string message)
        {
            var logEntry = new LogEntry
            {
                Level = e.Type.ToString(),
                Source = e.Source,
                Message = message,
                EventId = e.ID,
                ProcessId = e.Cache.ProcessId,
                ThreadId = e.Cache.ThreadId
            };

            LogstashClient.LogAsync(logEntry).GetAwaiter().GetResult();
        }
    }
}