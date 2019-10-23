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
            => Trace(new TraceEventData(eventCache, source, eventType, id), data);

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
            => Trace(new TraceEventData(eventCache, source, eventType, id), data);

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
            => Trace(new TraceEventData(eventCache, source, eventType, id), string.Empty);

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
            => Trace(new TraceEventData(eventCache, source, eventType, id), format, args);

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
            => Trace(new TraceEventData(eventCache, source, eventType, id), message);

        /// <summary>
        /// Writes a message.
        /// </summary>
        /// <param name="message">The message to write.</param>
        public sealed override void Write(string message)
            => Trace(new TraceEventData(), message);

        /// <summary>
        /// Writes a message, using the category as event type.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="category">The category to use.</param>
        public sealed override void Write(string message, string category)
            => Trace(new TraceEventData(ParseCategory(category)), message);

        /// <summary>
        /// Writes the value of an object.
        /// </summary>
        /// <param name="o">The object to write.</param>
        public sealed override void Write(object o) 
        {
            if (o is Exception)
                Trace(new TraceEventData(TraceEventType.Error), o);
            else
                Trace(new TraceEventData(), o);
        }

        /// <summary>
        /// Writes a data object, using the category as event type.
        /// </summary>
        /// <param name="o">The object to write.</param>
        /// <param name="category">The category to use.</param>
        public sealed override void Write(object o, string category)
            => Trace(new TraceEventData(ParseCategory(category)), o);

        /// <summary>
        /// Writes a message.
        /// </summary>
        /// <param name="message">The message to write.</param>
        public sealed override void WriteLine(string message)
        => Trace(new TraceEventData(), message);

        /// <summary>
        /// Writes a message, using the category as event type.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="category">The category to use.</param>
        public sealed override void WriteLine(string message, string category)
            => Trace(new TraceEventData(ParseCategory(category)), message);

        /// <summary>
        /// Writes the value of an object.
        /// </summary>
        /// <param name="o">The object to write.</param>
        public sealed override void WriteLine(object o) 
        {
            if (o is Exception)
                Trace(new TraceEventData(TraceEventType.Error), o);
            else
                Trace(new TraceEventData(), o);
        }

        /// <summary>
        /// Writes a data object, using the category as event type.
        /// </summary>
        /// <param name="o">The object to write.</param>
        /// <param name="category">The category to use.</param>
        public sealed override void WriteLine(object o, string category)
            => Trace(new TraceEventData(ParseCategory(category)), o);

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

        /// <summary>
        /// Provides information about a trace event.
        /// </summary>
        protected class TraceEventData 
        {
            //private static System.Collections.Generic.Dictionary<string, Type> typePerSourceFileCache = new System.Collections.Generic.Dictionary<string, Type>();

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
            /// class with the specified parameters.
            /// </summary>
            /// <param name="eventCache">
            /// The <see cref="TraceEventCache"/> object.
            /// </param>
            /// <param name="source">The source of the event.</param>
            /// <param name="eventType">The type of the event.</param>
            /// <param name="id">The ID of the event.</param>
            public TraceEventData(TraceEventCache eventCache, string source, TraceEventType? eventType, int id) 
            {
                Cache = eventCache;
                if (string.IsNullOrEmpty(source))
                    source = GetCallingSource();
                Source = source;
                Type = eventType;
                ID = id;
            }

            private string GetCallingSource([System.Runtime.CompilerServices.CallerMemberName] string memberName = "", [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "") {
                try {
                    //if (typePerSourceFileCache.TryGetValue(sourceFilePath, out Type type)) 
                    //    return type.ToString();
                    var stackFrames = new System.Diagnostics.StackTrace().GetFrames();
                    if (stackFrames == null) return null;
                    foreach (var frame in stackFrames) {
                        var frameReflectedType = frame.GetMethod().ReflectedType;
                        var namespaceName = frameReflectedType.Namespace;
                        if (!namespaceName.StartsWith("System.Diagnostics") && !namespaceName.StartsWith("Decos.Diagnostics")) {
                            //typePerSourceFileCache.Add(sourceFilePath, frameReflectedType);
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
        }
    }
}