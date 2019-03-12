using System;

namespace Decos.Diagnostics.Trace
{
    /// <summary>
    /// Provides <see cref="ILogFactory"/> instances that use the <see
    /// cref="System.Diagnostics.TraceSource"/> infrastructure.
    /// </summary>
    public class TraceSourceLogFactoryBuilder : LogFactoryBuilder<TraceSourceLogFactoryOptions>
    {
        internal TraceSourceLogFactoryBuilder()
        {
        }

        /// <summary>
        /// Creates a new <see cref="ILogFactory"/> instance with the configured
        /// options.
        /// </summary>
        /// <returns>A new <see cref="ILogFactory"/> instance.</returns>
        public override ILogFactory Build()
        {
            return new TraceSourceLogFactory(Options);
        }
    }
}