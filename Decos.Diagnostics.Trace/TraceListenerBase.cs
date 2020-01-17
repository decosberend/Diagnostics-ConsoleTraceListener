using System;
using System.Diagnostics;
using System.Text;

namespace Decos.Diagnostics.Trace 
{
    /// <summary>
    /// Provides an abstract base class for listeners who monitor trace and debug
    /// output. Only a single method needs to be implemented.
    /// </summary>
    public abstract class TraceListenerBase : TraceListener 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TraceListenerBase"/>
        /// class.
        /// </summary>
        protected TraceListenerBase() { }

        /// <summary>
        /// Gets or sets the defaultCustomerID to send with the logs 
        /// if it isn't specified when sending the log itself.
        /// </summary>
        public static Guid DefaultCustomerId { get; set; }

        /// <summary>
        /// Sets the defaultCustomerID to send with the logs 
        /// if it isn't specified when sending the log itself.
        /// </summary>
        /// <param name="newDefaultCustomerId">the new DefaultCustomerID</param>
        public static void SetDefaultCustomerId(Guid newDefaultCustomerId)
        {
            DefaultCustomerId = newDefaultCustomerId;
        }

        /// <summary>
        /// the defaultCustomerID for the current specific thread to send 
        /// with the logs if it isn't specified when sending the log itself.
        /// </summary>
        [ThreadStatic] public static Guid ThreadCustomerId;

        /// <summary>
        /// Sets the defaultCustomerID for the current specific thread to send 
        /// with the logs if it isn't specified when sending the log itself.
        /// </summary>
        /// <param name="newThreadCustomerId"></param>
        public static void SetThreadCustomerId(Guid newThreadCustomerId)
        {
            ThreadCustomerId = newThreadCustomerId;
        }

        /// <summary>
        /// the defaultSessionID for the current specific thread to send with the logs.
        /// </summary>
        [ThreadStatic] public static string ThreadSessionId;

        /// <summary>
        /// Sets the defaultSessionID for the current specific thread to send with the logs.
        /// </summary>
        /// <param name="newThreadSessionId"></param>
        public static void SetThreadSessionId(string newThreadSessionId)
        {
            ThreadSessionId = newThreadSessionId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceListenerBase"/>
        /// class using the specified name.
        /// </summary>
        /// <param name="name">The name of the trace listener.</param>
        protected TraceListenerBase(string name)
            : base(name) { }

        /// <summary>
        /// Writes trace information, a data object and event information to the
        /// listener specific output.
        /// </summary>
        /// <param name="eventCache">
        /// A <see cref="T:System.Diagnostics.TraceEventCache"/> object that
        /// contains the current process ID, thread ID, and stack trace
        /// information.
        /// </param>
        /// <param name="source">
        /// A name used to identify the output, typically the name of the
        /// application that generated the trace event.
        /// </param>
        /// <param name="eventType">
        /// One of the <see cref="T:System.Diagnostics.TraceEventType"/> values
        /// specifying the type of event that has caused the trace.
        /// </param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="data">The trace data to emit.</param>
        public sealed override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            TraceEventData eventData;
            if (data != null && data is CustomerLogData customerData)
            {
                if (customerData.HasCustomerId() && customerData.HasSessionId())
                {
                    eventData = new TraceEventData(eventCache, source, eventType, id, customerData.CustomerId, customerData.SessionId);
                }
                else if (customerData.HasCustomerId())
                {
                    eventData = new TraceEventData(eventCache, source, eventType, id, customerData.CustomerId);
                    if (ADefaultSessionIdIsSet(out string sessionIdToUse))
                        eventData.SessionID = sessionIdToUse;
                }
                else if (customerData.HasSessionId())
                {
                    eventData = new TraceEventData(eventCache, source, eventType, id, null, customerData.SessionId);
                    if (ADefaultCustomerIdIsSet(out Guid customerIdToUse))
                        eventData.CustomerID = customerIdToUse;
                }
                else
                {
                    eventData = AddCustomerIdAndSessionIdToEventDataIfTheyAreSet(new TraceEventData(eventCache, source, eventType, id));
                }
                Trace(eventData, data);
            }
            else if (data != null)
            {
                eventData = AddCustomerIdAndSessionIdToEventDataIfTheyAreSet(new TraceEventData(eventCache, source, eventType, id));
                Trace(eventData, data);
            }
        }

        /// <summary>
        /// Writes trace information, a data object and event information to the
        /// listener specific output.
        /// </summary>
        /// <param name="eventCache">
        /// A <see cref="T:System.Diagnostics.TraceEventCache"/> object that
        /// contains the current process ID, thread ID, and stack trace
        /// information.
        /// </param>
        /// <param name="source">
        /// A name used to identify the output, typically the name of the
        /// application that generated the trace event.
        /// </param>
        /// <param name="eventType">
        /// One of the <see cref="T:System.Diagnostics.TraceEventType"/> values
        /// specifying the type of event that has caused the trace.
        /// </param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="data">The trace data to emit.</param>
        public sealed override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
        {
            if (data != null)
            {
                TraceEventData eventData = AddCustomerIdAndSessionIdToEventDataIfTheyAreSet(new TraceEventData(eventCache, source, eventType, id));
                Trace(eventData, data);
            }
        }

        /// <summary>
        /// Writes trace and event information to the listener specific output.
        /// </summary>
        /// <param name="eventCache">
        /// A <see cref="T:System.Diagnostics.TraceEventCache"/> object that
        /// contains the current process ID, thread ID, and stack trace
        /// information.
        /// </param>
        /// <param name="source">
        /// A name used to identify the output, typically the name of the
        /// application that generated the trace event.
        /// </param>
        /// <param name="eventType">
        /// One of the <see cref="T:System.Diagnostics.TraceEventType"/> values
        /// specifying the type of event that has caused the trace.
        /// </param>
        /// <param name="id">A numeric identifier for the event.</param>
        public sealed override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
        {
            TraceEventData eventData = AddCustomerIdAndSessionIdToEventDataIfTheyAreSet(new TraceEventData(eventCache, source, eventType, id));
            Trace(eventData, string.Empty);
        }

        /// <summary>
        /// Writes trace information, a formatted array of objects and event
        /// information to the listener specific output.
        /// </summary>
        /// <param name="eventCache">
        /// A <see cref="T:System.Diagnostics.TraceEventCache"/> object that
        /// contains the current process ID, thread ID, and stack trace
        /// information.
        /// </param>
        /// <param name="source">
        /// A name used to identify the output, typically the name of the
        /// application that generated the trace event.
        /// </param>
        /// <param name="eventType">
        /// One of the <see cref="T:System.Diagnostics.TraceEventType"/> values
        /// specifying the type of event that has caused the trace.
        /// </param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="format">
        /// A format string that contains zero or more format items, which
        /// correspond to objects in the <paramref name="args"/> array.
        /// </param>
        /// <param name="args">
        /// An object array containing zero or more objects to format.
        /// </param>
        public sealed override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            TraceEventData eventData = AddCustomerIdAndSessionIdToEventDataIfTheyAreSet(new TraceEventData(eventCache, source, eventType, id));
            Trace(eventData, format, args);
        }

        /// <summary>
        /// Writes trace information, a message, and event information to the
        /// listener specific output.
        /// </summary>
        /// <param name="eventCache">
        /// A <see cref="T:System.Diagnostics.TraceEventCache"/> object that
        /// contains the current process ID, thread ID, and stack trace
        /// information.
        /// </param>
        /// <param name="source">
        /// A name used to identify the output, typically the name of the
        /// application that generated the trace event.
        /// </param>
        /// <param name="eventType">
        /// One of the <see cref="T:System.Diagnostics.TraceEventType"/> values
        /// specifying the type of event that has caused the trace.
        /// </param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="message">A message to write.</param>
        public sealed override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            TraceEventData eventData = AddCustomerIdAndSessionIdToEventDataIfTheyAreSet(new TraceEventData(eventCache, source, eventType, id));
            Trace(eventData, message);
        }

        /// <summary>
        /// Writes a message.
        /// </summary>
        /// <param name="message">The message to write.</param>
        public sealed override void Write(string message)
        {
            TraceEventData eventData = AddCustomerIdAndSessionIdToEventDataIfTheyAreSet(new TraceEventData());
            Trace(eventData, message);
        }

        /// <summary>
        /// Writes a message, using the category as event type.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="category">The category to use.</param>
        public sealed override void Write(string message, string category)
        {
            TraceEventData eventData = AddCustomerIdAndSessionIdToEventDataIfTheyAreSet(new TraceEventData(ParseCategory(category)));
            Trace(eventData, message);
        }

        /// <summary>
        /// Writes the value of an object.
        /// </summary>
        /// <param name="o">The object to write.</param>
        public sealed override void Write(object o) 
        {
            if (o is Exception)
            {
                TraceEventData eventData = AddCustomerIdAndSessionIdToEventDataIfTheyAreSet(new TraceEventData(TraceEventType.Error));
                Trace(eventData, o);
            }
            else
            {
                TraceEventData eventData = AddCustomerIdAndSessionIdToEventDataIfTheyAreSet(new TraceEventData());
                Trace(eventData, o);
            }
        }

        /// <summary>
        /// Writes a data object, using the category as event type.
        /// </summary>
        /// <param name="o">The object to write.</param>
        /// <param name="category">The category to use.</param>
        public sealed override void Write(object o, string category)
        {
            TraceEventData eventData = AddCustomerIdAndSessionIdToEventDataIfTheyAreSet(new TraceEventData(ParseCategory(category)));
            Trace(eventData, o);
        }

        /// <summary>
        /// Writes a message.
        /// </summary>
        /// <param name="message">The message to write.</param>
        public sealed override void WriteLine(string message)
        {
            TraceEventData eventData = AddCustomerIdAndSessionIdToEventDataIfTheyAreSet(new TraceEventData());
            Trace(eventData, message);
        }

        /// <summary>
        /// Writes a message, using the category as event type.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="category">The category to use.</param>
        public sealed override void WriteLine(string message, string category)
        {
            TraceEventData eventData = AddCustomerIdAndSessionIdToEventDataIfTheyAreSet(new TraceEventData(ParseCategory(category)));
            Trace(eventData, message);
        }

        /// <summary>
        /// Writes the value of an object.
        /// </summary>
        /// <param name="o">The object to write.</param>
        public sealed override void WriteLine(object o) 
        {
            if (o is Exception)
            {
                TraceEventData eventData = AddCustomerIdAndSessionIdToEventDataIfTheyAreSet(new TraceEventData(TraceEventType.Error));
                Trace(eventData, o);
            }
            else
            {
                TraceEventData eventData = AddCustomerIdAndSessionIdToEventDataIfTheyAreSet(new TraceEventData());
                Trace(eventData, o);
            }
        }

        /// <summary>
        /// Writes a data object, using the category as event type.
        /// </summary>
        /// <param name="o">The object to write.</param>
        /// <param name="category">The category to use.</param>
        public sealed override void WriteLine(object o, string category)
        {
            TraceEventData eventData = AddCustomerIdAndSessionIdToEventDataIfTheyAreSet(new TraceEventData(ParseCategory(category)));
            Trace(eventData, o);
        }

        /// <summary>
        /// Determines the <see cref="TraceEventType"/> to use based on the
        /// specified category name.
        /// </summary>
        /// <param name="category">The category name to parse.</param>
        /// <returns>A <see cref="TraceEventType"/> value.</returns>
        protected virtual TraceEventType ParseCategory(string category) 
        {
            if (category.StartsWith("SQL:"))
                return TraceEventType.Verbose;

            switch (category.ToUpperInvariant()) 
            {
                case "CRITICAL": return TraceEventType.Critical;
                case "ERROR": return TraceEventType.Error;
                case "WARNING": return TraceEventType.Warning;
                case "VERBOSE": return TraceEventType.Verbose;
                case "START": return TraceEventType.Start;
                case "STOP": return TraceEventType.Stop;
                case "SUSPEND": return TraceEventType.Suspend;
                case "RESUME": return TraceEventType.Resume;
                case "TRANSFER": return TraceEventType.Transfer;

                default:
                case "INFORMATION":
                    return TraceEventType.Information;
            }
        }

        /// <summary>
        /// Determines whether the trace listener should trace the event.
        /// </summary>
        /// <param name="cache">
        /// The <see cref="TraceEventCache"/> that contains information for the
        /// trace event.
        /// </param>
        /// <param name="source">The name of the source.</param>
        /// <param name="type">
        /// One of the <see cref="TraceEventType"/> values specifying the type of
        /// event that has caused the trace.
        /// </param>
        /// <param name="id">A trace identifier number.</param>
        /// <param name="message">
        /// Either the format to use for writing an array of arguments specified
        /// by the <paramref name="args"/> parameter, or a message to write.
        /// </param>
        /// <param name="args">An array of argument objects.</param>
        /// <param name="data1">A trace data object.</param>
        /// <param name="data">An array of trace data objects.</param>
        /// <returns>
        /// <see langword="true"/> to trace the specified event; otherwise, <see
        /// langword="false"/>.
        /// </returns>
        protected virtual bool ShouldTrace(TraceEventCache cache, string source, TraceEventType type, int id, string message, object[] args, object data1, object[] data) 
        {
            if (Filter == null)
                return true;
            return Filter.ShouldTrace(cache, source, type, id, message, args, data1, data);
        }

        /// <summary>
        /// Writes trace information and a message to the listener specific
        /// output.
        /// </summary>
        /// <param name="e">
        /// A <see cref="TraceEventData"/> object that contains information about
        /// the trace event.
        /// </param>
        /// <param name="message">The message to write.</param>
        protected void Trace(TraceEventData e, string message) 
        {
            var type = e.Type.GetValueOrDefault(TraceEventType.Information);
            if (ShouldTrace(e.Cache, e.Source, type, e.ID, message, null, null, null))
                TraceInternal(e, message);
        }

        /// <summary>
        /// Writes trace information and a formatted message to the listener
        /// specific output.
        /// </summary>
        /// <param name="e">
        /// A <see cref="TraceEventData"/> object that contains information about
        /// the trace event.
        /// </param>
        /// <param name="format">
        /// A format string that contains zero or more format items, which
        /// correspond to objects in the <paramref name="args"/> array.
        /// </param>
        /// <param name="args">
        /// An object array containing zero or more objects to format.
        /// </param>
        protected void Trace(TraceEventData e, string format, params object[] args) 
        {
            var type = e.Type.GetValueOrDefault(TraceEventType.Information);
            if (ShouldTrace(e.Cache, e.Source, type, e.ID, format, args, null, null))
                TraceInternal(e, string.Format(format, args));
        }

        /// <summary>
        /// Writes trace information and a data object to the listener specific
        /// output if.
        /// </summary>
        /// <param name="e">
        /// A <see cref="TraceEventData"/> object that contains information about
        /// the trace event.
        /// </param>
        /// <param name="data">The trace data to write.</param>
        protected void Trace(TraceEventData e, object data) 
        {
            var type = e.Type.GetValueOrDefault(TraceEventType.Information);
            if (ShouldTrace(e.Cache, e.Source, type, e.ID, null, null, data, null))
                TraceInternal(e, data);
        }

        /// <summary>
        /// When overridden in a derived class, writes trace information and a
        /// message to the listener specific output.
        /// </summary>
        /// <param name="e">
        /// A <see cref="TraceEventData"/> object that contains information about
        /// the trace event.
        /// </param>
        /// <param name="message">The message to write.</param>
        protected abstract void TraceInternal(TraceEventData e, string message);

        /// <summary>
        /// Writes trace information and a formatted message to the listener
        /// specific output.
        /// </summary>
        /// <param name="e">
        /// A <see cref="TraceEventData"/> object that contains information about
        /// the trace event.
        /// </param>
        /// <param name="format">
        /// A format string that contains zero or more format items, which
        /// correspond to objects in the <paramref name="args"/> array.
        /// </param>
        /// <param name="args">
        /// An object array containing zero or more objects to format.
        /// </param>
        protected virtual void TraceInternal(TraceEventData e, string format, params object[] args) => TraceInternal(e, string.Format(format, args));

        /// <summary>
        /// Writes trace information and a data object to the listener specific
        /// output.
        /// </summary>
        /// <param name="e">
        /// A <see cref="TraceEventData"/> object that contains information about
        /// the trace event.
        /// </param>
        /// <param name="data">The trace data to write.</param>
        protected virtual void TraceInternal(TraceEventData e, object data) 
        {
            string message;
            if (data is object[])
                message = ConvertDataToString((object[])data);
            else
                message = data.ToString();
            TraceInternal(e, message);
        }

        private static string ConvertDataToString(object[] data) 
        {
            if (data == null)
                return null;

            var messageBuilder = new StringBuilder();
            for (var i = 0; i < data.Length; i++) 
            {
                if (i != 0)
                    messageBuilder.Append(", ");

                if (data[i] != null)
                    messageBuilder.Append(data[i]);
            }

            return messageBuilder.ToString();
        }

        private TraceEventData AddCustomerIdAndSessionIdToEventDataIfTheyAreSet(TraceEventData eventData)
        {
            if (ADefaultCustomerIdIsSet(out Guid customerIdToUse))
                eventData.CustomerID = customerIdToUse;
            if (ADefaultSessionIdIsSet(out string sessionIdToUse))
                eventData.SessionID = sessionIdToUse;
            return eventData;
        }

        private bool ADefaultCustomerIdIsSet(out Guid customerIdToUse)
        {
            if (ThreadCustomerId != Guid.Empty)
            {
                customerIdToUse = ThreadCustomerId;
                return true;
            }
            else if (DefaultCustomerId != Guid.Empty)
            {
                customerIdToUse = DefaultCustomerId;
                return true;
            }
            customerIdToUse = Guid.Empty;
            return false;
        }

        private bool ADefaultSessionIdIsSet(out string sessionIdToUse)
        {
            if (!string.IsNullOrEmpty(ThreadSessionId))
            {
                sessionIdToUse = ThreadSessionId;
                return true;
            }
            sessionIdToUse = null;
            return false;
        }

        /// <summary>
        /// Provides information about a trace event.
        /// </summary>
        protected class TraceEventData 
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TraceEventData"/>
            /// class.
            /// </summary>
            public TraceEventData()
              : this(new TraceEventCache(), null, null, 0) { }

            /// <summary>
            /// Initializes a new instance of the <see cref="TraceEventData"/>
            /// class with the specified event type.
            /// </summary>
            /// <param name="eventType">The type of event.</param>
            public TraceEventData(TraceEventType eventType)
              : this(new TraceEventCache(), null, eventType, 0) { }

            /// <summary>
            /// Initializes a new instance of the <see cref="TraceEventData"/>
            /// class with the specified customerID.
            /// </summary>
            /// <param name="customerID">The ID of the customer active when sending the log.</param>
            public TraceEventData(Guid customerID)
              : this(new TraceEventCache(), null, null, 0, customerID) { }

            /// <summary>
            /// Initializes a new instance of the <see cref="TraceEventData"/>
            /// class with the specified event type and customerID.
            /// </summary>
            /// <param name="eventType">The type of event.</param>
            /// <param name="customerID">The ID of the customer active when sending the log.</param>
            public TraceEventData(TraceEventType eventType, Guid customerID)
              : this(new TraceEventCache(), null, eventType, 0, customerID) { }

            /// <summary>
            /// Initializes a new instance of the <see cref="TraceEventData"/>
            /// class with the specified customerID and sessionID.
            /// </summary>
            /// <param name="customerID">The ID of the customer active when sending the log.</param>
            /// <param name="sessionID">The ID of the session active when sending the log.</param>
            public TraceEventData(Guid customerID, string sessionID)
              : this(new TraceEventCache(), null, null, 0, customerID, sessionID) { }

            /// <summary>
            /// Initializes a new instance of the <see cref="TraceEventData"/>
            /// class with the specified customerID and sessionID.
            /// </summary>
            /// <param name="eventType">The type of event.</param>
            /// <param name="customerID">The ID of the customer active when sending the log.</param>
            /// <param name="sessionID">The ID of the session active when sending the log.</param>
            public TraceEventData(TraceEventType eventType, Guid customerID, string sessionID)
              : this(new TraceEventCache(), null, eventType, 0, customerID, sessionID) { }

            /// <summary>
            /// Initializes a new instance of the <see cref="TraceEventData"/>
            /// class with the specified parameters.
            /// </summary>
            /// <param name="eventCache">
            /// The <see cref="TraceEventCache"/> object.
            /// </param>
            /// <param name="source">The source of the event.</param>
            /// <param name="eventType">The type of the event.</param>
            /// <param name="id">The ID of the event.</param>
            /// <param name="customerID">The ID of the Customer who was active on the moment of writing</param>
            /// <param name="sessionID">The ID of the Session that was active on the moment of writing</param>
            public TraceEventData(TraceEventCache eventCache, string source, TraceEventType? eventType, int id, Guid? customerID = null, string sessionID = null) 
            {
                Cache = eventCache;
                if (string.IsNullOrEmpty(source))
                    source = GetCallingSource();
                Source = source;
                Type = eventType;
                ID = id;
                CustomerID = customerID;
                SessionID = sessionID;
            }

            private string GetCallingSource([System.Runtime.CompilerServices.CallerMemberName] string memberName = "", [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "") {
                try {
                    var stackFrames = new System.Diagnostics.StackTrace().GetFrames();
                    if (stackFrames == null) return null;
                    foreach (var frame in stackFrames) {
                        var frameReflectedType = frame.GetMethod().ReflectedType;
                        var namespaceName = frameReflectedType.Namespace;
                        if (!namespaceName.StartsWith("System.Diagnostics") && !namespaceName.StartsWith("Decos.Diagnostics")) {
                            return frameReflectedType.ToString();
                        }
                    }
                }
                catch { }
                return null;
            }

            /// <summary>
            /// Gets a <see cref="TraceEventCache"/> object that provides trace
            /// event data specific to a thread and process.
            /// </summary>
            public TraceEventCache Cache { get; }

            /// <summary>
            /// Gets a numeric identifier for the event.
            /// </summary>
            public int ID { get; }

            /// <summary>
            /// Gets the name used to identify the output, typically the name of
            /// the application that generated the trace event.
            /// </summary>
            public string Source { get; }

            /// <summary>
            /// Gets the type of event, or <c>null</c> if no type was specified.
            /// </summary>
            public TraceEventType? Type { get; }

            /// <summary>
            /// The ID of the Customer who was active on the moment of writing.
            /// </summary>
            public Guid? CustomerID { get; set; }

            /// <summary>
            /// The ID of the the Session that was active on the moment of writing.
            /// </summary>
            public string SessionID { get; set; }
        }
    }
}