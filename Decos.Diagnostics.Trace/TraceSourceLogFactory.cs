using System.Diagnostics;
using System.Linq;

namespace Decos.Diagnostics.Trace
{
    public class TraceSourceLogFactory : ILogFactory
    {
        public TraceSourceLogFactory()
          : this(new TraceSourceLogFactoryOptions())
        {
        }

        public TraceSourceLogFactory(TraceSourceLogFactoryOptions options)
        {
            Options = options;
        }

        protected TraceSourceLogFactoryOptions Options { get; }

        public ILog Create<T>()
            => Create(typeof(T).Namespace);

        public ILog Create(string name)
        {
            var traceSource = CreateSource(name);

            return new TraceSourceLog(traceSource);
        }

        private TraceSource CreateSource(string name)
        {
            var traceSource = new TraceSource(name, Options.MinimumLogLevel.ToSourceLevels());
            // TODO: allow overriding switch level per source based on a dictionary in Options. Support inheritance like ASP.NET Core logging.
            // TODO: test with .NET Framework if overriding via config is still possible this way

            foreach (TraceListener listener in System.Diagnostics.Trace.Listeners)
            {
                if (!traceSource.Listeners.Cast<TraceListener>().Any(x => x.Name == listener.Name))
                    traceSource.Listeners.Add(listener);
            }

            return traceSource;
        }
    }
}
