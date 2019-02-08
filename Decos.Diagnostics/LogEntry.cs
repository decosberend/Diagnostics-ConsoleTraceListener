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
    }
}
