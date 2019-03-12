using System.Collections.Generic;
using System.Linq;

namespace Decos.Diagnostics
{
    /// <summary>
    /// Represents options for specifying the behavior of the log factory classes
    /// and the instances they create.
    /// </summary>
    public class LogFactoryOptions
    {
        /// <summary>
        /// Gets or sets the minimum severity level messages must have to be
        /// written to a log if no other filters match.
        /// </summary>
        public LogLevel DefaultMinimumLogLevel { get; set; }
            = LogLevel.Information;

        /// <summary>
        /// Gets a dictionary that specifies the minimum severity levels messages
        /// from certain sources must have to be written to a log.
        /// </summary>
        public IDictionary<SourceName, LogLevel> MinimumLogLevels { get; }
            = new SortedDictionary<SourceName, LogLevel>();

        /// <summary>
        /// Determines the minimum severity level messages from the specified
        /// source must have to be written to a log.
        /// </summary>
        /// <param name="name">The name of the source to check.</param>
        /// <returns>
        /// The minimum <see cref="LogLevel"/> message from <paramref
        /// name="name"/> must have to be written to a log.
        /// </returns>
        public virtual LogLevel GetLogLevel(SourceName name)
        {
            if (MinimumLogLevels.Any(x => name.Matches(x.Key)))
            {
                return MinimumLogLevels.FirstOrDefault(x => name.Matches(x.Key)).Value;
            }

            return DefaultMinimumLogLevel;
        }
    }
}