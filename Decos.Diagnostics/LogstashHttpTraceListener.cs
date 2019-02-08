using System;

namespace Decos.Diagnostics
{
    public class LogstashHttpTraceListener : TraceListenerBase
    {
        private static readonly Uri defaultEndpoint = new Uri("http://log.decos.nl:9090");

        public LogstashHttpTraceListener()
        {
            LogstashClient = new HttpLogstashClient(defaultEndpoint);
        }

        public LogstashHttpTraceListener(string endpointAddress)
        {
            var endpoint = new Uri(endpointAddress, UriKind.Absolute);
            LogstashClient = new HttpLogstashClient(endpoint);
        }

        protected HttpLogstashClient LogstashClient { get; }

        protected override void TraceInternal(TraceEventData e, string message)
        {
            var hostName = System.Net.Dns.GetHostEntry("localhost").HostName;
            var logEntry = new LogEntry
            {
                Level = e.Type.ToString(),
                Source = e.Source,
                Message = message,
                EventId = e.ID,
                HostName = hostName,
                ProcessId = e.Cache.ProcessId,
                ThreadId = e.Cache.ThreadId
            };

            LogstashClient.LogAsync(logEntry).GetAwaiter().GetResult();
        }
    }
}