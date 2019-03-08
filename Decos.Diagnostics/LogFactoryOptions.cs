namespace Decos.Diagnostics
{
    /// <summary>
    /// Represents options for specifying the behavior of the log factory 
    /// classes and the instances they create.
    /// </summary>
    public class LogFactoryOptions
    {
        /// <summary>
        /// Gets or sets the minimum severity level messages must have to be
        /// written to a log.
        /// </summary>
        public LogLevel MinimumLogLevel { get; set; }
            = LogLevel.Information;
    }
}
