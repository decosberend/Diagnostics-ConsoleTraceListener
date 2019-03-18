using Decos.Diagnostics.Trace;

namespace Decos.Diagnostics
{
    /// <summary>
    /// Provides a set of static methods for configuring the logging system to
    /// use the <see cref="System.Diagnostics.TraceSource"/> infrastructure.
    /// </summary>
    public static class LogFactoryBuilderExtensions
    {
        /// <summary>
        /// Specifies that the <see cref="System.Diagnostics.TraceSource"/>
        /// infrastructure should be used for logging.
        /// </summary>
        /// <param name="builder">
        /// The <see cref="LogFactoryBuilder"/> class that acts as an entry point
        /// for the logging configuration.
        /// </param>
        /// <returns>
        /// A new <see cref="TraceSourceLogFactoryBuilder"/> used to configure
        /// and create <see cref="ILogFactory"/> instances.
        /// </returns>
        public static TraceSourceLogFactoryBuilder UseTraceSource(this LogFactoryBuilder builder)
            => new TraceSourceLogFactoryBuilder();
    }
}