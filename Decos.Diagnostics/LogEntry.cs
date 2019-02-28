using System;

namespace Decos.Diagnostics
{
    public class LogEntry
    {
        public int? EventId { get; set; }

        public string Level { get; set; }

        public string Source { get; set; }

        public string Message { get; set; }

        public string HostName { get; set; }

        public int ProcessId { get; set; }

        public string ThreadId { get; set; }

        public LogEntry()
        {
            // While a little convoluted, this will return the FQDN when 
            // possible (unlike GetHostName alone), and otherwise the host name
            // (as opposed to simply "localhost").
            HostName = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).HostName;
        }
    }
}
