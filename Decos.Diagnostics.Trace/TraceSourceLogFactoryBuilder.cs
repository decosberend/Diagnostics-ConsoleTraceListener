using System;
using System.Diagnostics;

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
        /// Enables logging to the standard output stream.
        /// </summary>
        /// <returns>A reference to this builder.</returns>
        public TraceSourceLogFactoryBuilder AddConsole()
            => AddTraceListener<ConsoleTraceListener>();

        /// <summary>
        /// Enables logging to the standard output stream.
        /// </summary>
        /// <param name="minimumLogLevel">
        /// The minimum log level of messages to send to the trace listener.
        /// </param>
        /// <returns>A reference to this builder.</returns>
        public TraceSourceLogFactoryBuilder AddConsole(LogLevel minimumLogLevel)
            => AddTraceListener<ConsoleTraceListener>(minimumLogLevel);

        /// <summary>
        /// Enables logging to a Logstash HTTP input plugin.
        /// </summary>
        /// <param name="endpoint">
        /// The endpoint at which the Logstash HTTP input plugin is listening.
        /// </param>
        /// <returns>A reference to this builder.</returns>
        public TraceSourceLogFactoryBuilder AddLogstash(string endpoint)
            => AddLogstash(new Uri(endpoint, UriKind.Absolute));

        /// <summary>
        /// Enables logging to a Logstash HTTP input plugin.
        /// </summary>
        /// <param name="endpoint">
        /// The endpoint at which the Logstash HTTP input plugin is listening.
        /// </param>
        /// <param name="minimumLogLevel">
        /// The minimum log level of messages to send to the trace listener.
        /// </param>
        /// <returns>A reference to this builder.</returns>
        public TraceSourceLogFactoryBuilder AddLogstash(string endpoint, LogLevel minimumLogLevel)
            => AddLogstash(new Uri(endpoint, UriKind.Absolute), minimumLogLevel);

        /// <summary>
        /// Enables logging to a Logstash HTTP input plugin.
        /// </summary>
        /// <param name="endpoint">
        /// The endpoint at which the Logstash HTTP input plugin is listening.
        /// </param>
        /// <returns>A reference to this builder.</returns>
        public TraceSourceLogFactoryBuilder AddLogstash(Uri endpoint)
        {
            var listener = new AsyncLogstashHttpTraceListener(endpoint);
            return AddTraceListener(listener);
        }

        /// <summary>
        /// Enables logging to a Logstash HTTP input plugin.
        /// </summary>
        /// <param name="endpoint">
        /// The endpoint at which the Logstash HTTP input plugin is listening.
        /// </param>
        /// <param name="minimumLogLevel">
        /// The minimum log level of messages to send to the trace listener.
        /// </param>
        /// <returns>A reference to this builder.</returns>
        public TraceSourceLogFactoryBuilder AddLogstash(Uri endpoint, LogLevel minimumLogLevel)
        {
            var listener = new AsyncLogstashHttpTraceListener(endpoint);
            return AddTraceListener(listener, minimumLogLevel);
        }

        /// <summary>
        /// Enables logging to the specified trace listener.
        /// </summary>
        /// <param name="traceListener">The trace listener to add.</param>
        /// <returns>A reference to this builder.</returns>
        public TraceSourceLogFactoryBuilder AddTraceListener(TraceListener traceListener)
        {
            if (traceListener == null)
                throw new ArgumentNullException(nameof(traceListener));

            Options.Listeners.Add(traceListener);
            return this;
        }

        /// <summary>
        /// Enables logging to the specified trace listener for messages at a
        /// certain level.
        /// </summary>
        /// <param name="traceListener">The trace listener to add.</param>
        /// <param name="minimumLogLevel">
        /// The minimum log level of messages to send to the trace listener.
        /// </param>
        /// <returns>A reference to this builder.</returns>
        public TraceSourceLogFactoryBuilder AddTraceListener(TraceListener traceListener, LogLevel minimumLogLevel)
        {
            if (traceListener == null)
                throw new ArgumentNullException(nameof(traceListener));

            traceListener.Filter = new EventTypeFilter(minimumLogLevel.ToSourceLevels());
            return AddTraceListener(traceListener);
        }

        /// <summary>
        /// Enables logging a trace listener of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of trace listener to add.</typeparam>
        /// <returns>A reference to this builder.</returns>
        public TraceSourceLogFactoryBuilder AddTraceListener<T>()
            where T : TraceListener
        {
            var traceListener = Activator.CreateInstance<T>();
            return AddTraceListener(traceListener);
        }

        /// <summary>
        /// Enables logging a trace listener of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of trace listener to add.</typeparam>
        /// <param name="minimumLogLevel">
        /// The minimum log level of messages to send to the trace listener.
        /// </param>
        /// <returns>A reference to this builder.</returns>
        public TraceSourceLogFactoryBuilder AddTraceListener<T>(LogLevel minimumLogLevel)
            where T : TraceListener
        {
            var traceListener = Activator.CreateInstance<T>();
            traceListener.Filter = new EventTypeFilter(minimumLogLevel.ToSourceLevels());
            return AddTraceListener(traceListener);
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