using System.Diagnostics;
using System.Linq;

namespace Decos.Diagnostics.Trace
{
    /// <summary>
    /// Provides <see cref="ILog"/> instances that use the <see cref="TraceSource"/>
    /// infrastructure.
    /// </summary>
    public class TraceSourceLogFactory : ILogFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TraceSourceLogFactory/>
        /// class with the default options.
        /// </summary>
        public TraceSourceLogFactory()
          : this(new TraceSourceLogFactoryOptions())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceSourceLogFactory"/>
        /// class with the specified options.
        /// </summary>
        /// <param name="options">
        /// Options that specify the behavior of the 
        /// <see cref="TraceSourceLogFactory"/> class and the instances it 
        /// creates.
        /// </param>
        public TraceSourceLogFactory(TraceSourceLogFactoryOptions options)
        {
            Options = options;
        }

        /// <summary>
        /// Gets the options that specify the behavior of the 
        /// <see cref="TraceSourceLogFactory"/> class and the instances it 
        /// creates.
        /// </summary>
        protected TraceSourceLogFactoryOptions Options { get; }

        /// <summary>
        /// Creates a new <see cref="ILog"/> instance that writes logging 
        /// information to a <see cref="TraceSource"/> for the specified type.
        /// </summary>
        /// <typeparam name="T">The type (e.g. class) that acts as the source of logging information.</typeparam>
        /// <returns>A new <see cref="ILog"/> instance for the specified type.</returns>
        public ILog Create<T>()
            => Create(typeof(T).Namespace);

        /// <summary>
        /// Creates a new <see cref="ILog"/> instance that write logging 
        /// information to a <see cref="TraceSource"/> with the specified name.
        /// </summary>
        /// <param name="name">A name for the source of the logging information.</param>
        /// <returns>A new <see cref="ILog"/> instance with the specified name.</returns>
        public ILog Create(string name)
        {
            var traceSource = CreateSource(name);

            return new TraceSourceLog(traceSource);
        }

        private TraceSource CreateSource(string name)
        {
            var traceSource = new TraceSource(name, Options.MinimumLogLevel.ToSourceLevels());
            // TODO: allow overriding switch level per source based on a dictionary in Options. Support inheritance like ASP.NET Core logging.

            foreach (TraceListener listener in System.Diagnostics.Trace.Listeners)
            {
                if (!traceSource.Listeners.Cast<TraceListener>().Any(x => x.Name == listener.Name))
                    traceSource.Listeners.Add(listener);
            }

            return traceSource;
        }
    }
}
