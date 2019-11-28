using System;

namespace Decos.Diagnostics
{
    /// <summary>
    /// Represents an entry in a log.
    /// </summary>
    public class LogEntry
    {
        private static readonly string hostName = GetHostName();

        /// <summary>
        /// Initializes a new instance of the <see cref="LogEntry"/> class.
        /// </summary>
        public LogEntry()
        {
        }

        /// <summary>
        /// Gets or sets an event ID for the log entry.
        /// </summary>
        public int? EventId { get; set; }

        /// <summary>
        /// Gets or sets the severity level of the log entry.
        /// </summary>
        public LogLevel Level { get; set; }

        /// <summary>
        /// Gets or sets the source of the log entry.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the message text of the log entry.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets structured data of the log entry.
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Gets or sets the name or fully-qualified domain name of the host that
        /// created the log entry.
        /// </summary>
        public string HostName { get; set; } = hostName;

        /// <summary>
        /// Gets or sets the process ID of the process that created the log
        /// entry.
        /// </summary>
        public int ProcessId { get; set; }

        /// <summary>
        /// Gets or sets the managed thread ID of the thread that created the log
        /// entry.
        /// </summary>
        public string ThreadId { get; set; }

        /// <summary>
        /// Gets or sets the point in time the log entry was created.
        /// </summary>
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// Gets or sets the ID of the Customer who was active when the log was written.
        /// </summary>
        public Guid? CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the Session that was active when the log was written.
        /// </summary>
        public string SessionId { get; set; }

        private static string GetHostName()
        {
            try
            {
                // While a little convoluted, this will return the FQDN when
                // possible (unlike GetHostName alone), and otherwise the host
                // name (as opposed to simply "localhost").
                return System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).HostName;
            }
            catch
            {
                try
                {
                    return System.Net.Dns.GetHostName();
                }
                catch { }
            }

            return string.Empty;
        }
    }
}