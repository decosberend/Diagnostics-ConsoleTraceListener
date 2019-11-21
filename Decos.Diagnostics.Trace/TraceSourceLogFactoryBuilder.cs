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
        /// Enables logging to an UDP client.
        /// </summary>
        /// <param name="initializeData">A string in "hostname:port" format.</param>
        /// <returns>A reference to this builder.</returns>
        public TraceSourceLogFactoryBuilder AddUdp(string initializeData)
        {
            var listener = new UdpTraceListener(initializeData);
            return AddTraceListener(listener);
        }

        /// <summary>
        /// Enables logging to an UDP client.
        /// </summary>
        /// <param name="initializeData">A string in "hostname:port" format.</param>
        /// <param name="minimumLogLevel">The minimum log level of messages to send to the trace listener.</param>
        /// <returns>A reference to this builder.</returns>
        public TraceSourceLogFactoryBuilder AddUdp(string initializeData, LogLevel minimumLogLevel)
        {
            var listener = new UdpTraceListener(initializeData);
            return AddTraceListener(listener, minimumLogLevel);
        }

        /// <summary>
        /// Enables logging to an UDP client.
        /// </summary>
        /// <param name="hostname">The name or IP address of the remote host.</param>
        /// <param name="port">The UDP port number to use.</param>
        /// <returns>A reference to this builder.</returns>
        public TraceSourceLogFactoryBuilder AddUdp(string hostname, int port)
        {
            var listener = new UdpTraceListener(hostname, port);
            return AddTraceListener(listener);
        }

        /// <summary>
        /// Enables logging to an UDP client.
        /// </summary>
        /// <param name="hostname">The name or IP address of the remote host.</param>
        /// <param name="port">The UDP port number to use.</param>
        /// <param name="minimumLogLevel">The minimum log level of messages to send to the trace listener.</param>
        /// <returns>A reference to this builder.</returns>
        public TraceSourceLogFactoryBuilder AddUdp(string hostname, int port, LogLevel minimumLogLevel)
        {
            var listener = new UdpTraceListener(hostname, port);
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
        /// Clears all Trace listeners in Options.Listeners and System.Diagnostics.Trace.Listeners
        /// </summary>
        /// <returns>A reference to this builder.</returns>
        public TraceSourceLogFactoryBuilder ClearAllTraceListeners()
        {
            Options.Listeners.Clear();
            System.Diagnostics.Trace.Listeners.Clear();
            return this;
        }

        /// <summary>
        /// Adds the listeners in Options.Listeners to System.Diagnostics.Trace.Listeners
        /// if there aren't any of that type in there already
        /// </summary>
        /// <returns>A reference to this builder.</returns>
        public TraceSourceLogFactoryBuilder AddListenersToTraceListenersCollection()
        {
            foreach (var listener in Options.Listeners)
            {
                if (GetIndexOfListenerOfTypeInTraceListenerCollection(System.Diagnostics.Trace.Listeners, listener.GetType()) < 0)
                {
                    System.Diagnostics.Trace.Listeners.Add(listener);
                }
                else
                {
                    int listenerIndexToEdit = GetIndexOfListenerOfTypeInTraceListenerCollection(System.Diagnostics.Trace.Listeners, listener.GetType());
                    if (System.Diagnostics.Trace.Listeners[listenerIndexToEdit] is TraceListenerBase traceListenerBase)
                    {
                        Decos.Diagnostics.Trace.TraceListenerBase.ThreadCustomerId = Options.DefaultThreadCustomerID;
                    }
                }
            }

            return this;
        }
        
        /// <summary>
        /// Finds what index of a TraceListenerCollection has a listener of a specified type.
        /// </summary>
        /// <param name="listeners">The collection to check</param>
        /// <param name="type">The type of listener to find in the collection</param>
        /// <returns>The index number of the specified type, if there is non returns -1</returns>
        private int GetIndexOfListenerOfTypeInTraceListenerCollection(TraceListenerCollection listeners, Type type)
        {
            for (int i = 0; i < listeners.Count; i++)
            {
                if (listeners[i].GetType() == type)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Creates a new <see cref="ILogFactory"/> instance with the configured
        /// options.
        /// </summary>
        /// <returns>A new <see cref="ILogFactory"/> instance.</returns>
        public override ILogFactory Build()
        {
            SetCustomerIdInListenerCollection(Options);
            SetSessionIdInListenerCollection(Options);
            return new TraceSourceLogFactory(Options);
        }

        /// <summary>
        /// Sets the default CustomerId in every Listener in the options
        /// </summary>
        /// <param name="options">options containing listeners</param>
        private void SetCustomerIdInListenerCollection(TraceSourceLogFactoryOptions options)
        {
            if (options.DefaultCustomerID != null && options.DefaultCustomerID != Guid.Empty) // not null and not guid.empty 
            {
                Decos.Diagnostics.Trace.TraceListenerBase.SetDefaultCustomerId(options.DefaultCustomerID);
            }
            if (options.DefaultThreadCustomerID != null && options.DefaultThreadCustomerID != Guid.Empty) // not null and not guid.empty 
            {
                Decos.Diagnostics.Trace.TraceListenerBase.SetThreadCustomerId(options.DefaultThreadCustomerID);
            }
        }


        private void SetSessionIdInListenerCollection(TraceSourceLogFactoryOptions options)
        {
            if (!string.IsNullOrEmpty(options.DefaultThreadSessionID)) // not null
            {
                Decos.Diagnostics.Trace.TraceListenerBase.SetThreadSessionId(options.DefaultThreadSessionID);
            }
        }
    }
}