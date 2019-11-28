using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace Decos.Diagnostics.Trace
{
    /// <summary>
    /// Provides <see cref="ILog"/> instances that use the <see
    /// cref="TraceSource"/> infrastructure.
    /// </summary>
    public class TraceSourceLogFactory : ILogFactory
    {
        private readonly CancellationTokenSource shutdownTokenSource
            = new CancellationTokenSource();

        private readonly CancellationTokenSource cancellationTokenSource
            = new CancellationTokenSource();

        private readonly ICollection<Task> shutdownTasks
            = new List<Task>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceSourceLogFactory"/>
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
        /// Options that specify the behavior of the <see
        /// cref="TraceSourceLogFactory"/> class and the instances it creates.
        /// </param>
        public TraceSourceLogFactory(TraceSourceLogFactoryOptions options)
        {
            Options = options;
        }

        /// <summary>
        /// Gets the options that specify the behavior of the <see
        /// cref="TraceSourceLogFactory"/> class and the instances it creates.
        /// </summary>
        protected TraceSourceLogFactoryOptions Options { get; }

        /// <summary>
        /// Creates a new <see cref="ILog"/> instance that writes logging
        /// information to a <see cref="TraceSource"/> for the specified type.
        /// </summary>
        /// <typeparam name="T">
        /// The type (e.g. class) that acts as the source of logging information.
        /// </typeparam>
        /// <returns>
        /// A new <see cref="ILog"/> instance for the specified type.
        /// </returns>
        public ILog Create<T>()
            => Create(SourceName.FromType<T>());

        /// <summary>
        /// Creates a new <see cref="ILog"/> instance that write logging
        /// information to a <see cref="TraceSource"/> with the specified name.
        /// </summary>
        /// <param name="name">
        /// A name for the source of the logging information.
        /// </param>
        /// <returns>
        /// A new <see cref="ILog"/> instance with the specified name.
        /// </returns>
        public ILog Create(SourceName name)
        {
            var traceSource = CreateSource(name);

            return new TraceSourceLog(traceSource);
        }

        private TraceSource CreateSource(SourceName name)
        {
            var switchValue = Options.GetLogLevel(name).ToSourceLevels();
            var traceSource = new TraceSource(name, switchValue);

            var listeners = Options.Listeners
                .Concat(System.Diagnostics.Trace.Listeners.Cast<TraceListener>());
            foreach (var listener in listeners)
            {
                if (listener is DefaultTraceListener
                    && traceSource.Listeners.OfType<DefaultTraceListener>().Any())
                    continue;

                if (!traceSource.Listeners.Contains(listener))
                {
                    if (listener is AsyncTraceListener asyncListener)
                        HookShutdown(asyncListener);

                    traceSource.Listeners.Add(listener);
                }
            }

            return traceSource;
        }

        /// <summary>
        /// Waits for any long-running logging operations to shut down
        /// gracefully.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task ShutdownAsync()
        {
            shutdownTokenSource.Cancel();
            await Task.WhenAll(shutdownTasks);
        }

        /// <summary>
        /// Waits for any long-running logging operations to shut down
        /// gracefully.
        /// </summary>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests. If you cancel the
        /// shutdown, some logs may be lost.
        /// </param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public Task ShutdownAsync(CancellationToken cancellationToken)
        {
            cancellationToken.Register(() => cancellationTokenSource.Cancel());
            return ShutdownAsync();
        }

        private void HookShutdown(AsyncTraceListener listener)
        {
            var processTask = listener.ProcessQueueAsync(
                shutdownTokenSource.Token, cancellationTokenSource.Token);
            shutdownTasks.Add(processTask);
        }
    }
}