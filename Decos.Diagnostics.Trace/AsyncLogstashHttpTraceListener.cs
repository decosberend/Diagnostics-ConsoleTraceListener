using System;
using System.Threading;
using System.Threading.Tasks;

namespace Decos.Diagnostics.Trace
{
    /// <summary>
    /// Represents a trace listener that sends logging events and data to a
    /// Logstash HTTP input plugin.
    /// </summary>
    public class AsyncLogstashHttpTraceListener : AsyncTraceListener
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="AsyncLogstashHttpTraceListener"/> using the specified endpoint.
        /// </summary>
        /// <param name="endpointAddress">
        /// The Logstash HTTP input plugin URL.
        /// </param>
        public AsyncLogstashHttpTraceListener(string endpointAddress)
            : this(new Uri(endpointAddress, UriKind.Absolute))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="AsyncLogstashHttpTraceListener"/> using the specified endpoint.
        /// </summary>
        /// <param name="endpointAddress">
        /// The Logstash HTTP input plugin URL.
        /// </param>
        public AsyncLogstashHttpTraceListener(Uri endpointAddress)
        {
            LogstashClient = new HttpLogstashClient(endpointAddress);
        }

        /// <summary>
        /// Gets a reference to the client used to send log entries to Logstash.
        /// </summary>
        protected HttpLogstashClient LogstashClient { get; }

        /// <summary>
        /// Sends a log entry to Logstash.
        /// </summary>
        /// <param name="logEntry">The log entry to be written.</param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests.
        /// </param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public override Task TraceAsync(LogEntry logEntry, CancellationToken cancellationToken)
        {
            // debug here later
            return LogstashClient.LogAsync(logEntry, cancellationToken);
        }
    }
}