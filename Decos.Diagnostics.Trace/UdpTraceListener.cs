using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Xml.Serialization;
using System.Xml;

namespace Decos.Diagnostics.Trace
{
    /// <summary>
    /// Represents a trace listener that sends logging output to a remote server
    /// via UDP using the log4net XML layout.
    /// </summary>
    public class UdpTraceListener : TraceListenerBase
    {
        /// <summary>
        /// The maximum size in bytes of a datagram before messages will be 
        /// truncated. 
        /// </summary>
        public const int MaximumDatagramSize = 1472;

        // private static readonly ITracer _trace = Tracer.Create("Decos.Shared");
        private const string _defaultSource = "Trace";
        private UdpClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpTraceListener"/> class using 
        /// the specified server address.
        /// </summary>
        /// <param name="initializeData">A string in "hostname:port" format.</param>
        public UdpTraceListener(string initializeData)
        {
            int iSeparator = initializeData.LastIndexOf(':');
            if (iSeparator < 0)
                throw new ArgumentException(initializeData + " is not in the expected format \"hostname:port\"", "initializeData");

            string hostname = initializeData.Substring(0, iSeparator);
            int port = int.Parse(initializeData.Substring(iSeparator + 1));
            _client = new UdpClient(hostname, port);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpTraceListener"/> class using
        /// the specified hostname and port number.
        /// </summary>
        /// <param name="hostname">The name or IP address of the remote host.</param>
        /// <param name="port">The UDP port number to use.</param>
        public UdpTraceListener(string hostname, int port)
        {
            _client = new UdpClient(hostname, port);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpTraceListener"/> class using 
        /// the specified server address and name.
        /// </summary>
        /// <param name="initializeData">A string in "hostname:port" format.</param>
        /// <param name="name">The name of the trace listener.</param>
        public UdpTraceListener(string initializeData, string name) : base(name)
        {
            int iSeparator = initializeData.LastIndexOf(':');
            if (iSeparator < 0)
                throw new ArgumentException(initializeData + " is not in the expected format \"hostname:port\"", "initializeData");

            string hostname = initializeData.Substring(0, iSeparator);
            int port = int.Parse(initializeData.Substring(iSeparator + 1));
            _client = new UdpClient(hostname, port);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UdpTraceListener"/> class using
        /// the specified hostname, port number and name.
        /// </summary>
        /// <param name="hostname">The name or IP address of the remote host.</param>
        /// <param name="port">The UDP port number to use.</param>
        /// /// <param name="name">The name of the trace listener.</param>
        public UdpTraceListener(string hostname, int port, string name) : base(name)
        {
            _client = new UdpClient(hostname, port);
        }

        /// <summary>
        /// Gets a value indicating whether the trace listener is thread safe.
        /// </summary>
        public override bool IsThreadSafe => true;

        /// <summary>
        /// Sends trace information and a message to remote host.
        /// </summary>
        /// <param name="e">
        /// A <see cref="TraceListenerBase.TraceEventData"/> object that contains
        /// information about the trace event.
        /// </param>
        /// <param name="message">The message to write.</param>
        protected override void TraceInternal(TraceEventData e, string message)
        {
            Send(new Log4NetEvent
            {
                Message = ConstructLogMessageToWrite(e, message),
                Source = e.Source,
                Level = GetTraceEventDataTypeWithInformationAsDefault(e),
                Timestamp = e.Cache.DateTime,
                Thread = e.Cache.ThreadId ?? e.Cache.ThreadId.ToString(),
                Application = AppDomain.CurrentDomain.FriendlyName
            });
        }

        /// <summary>
        /// Sends trace information and an object to remote host.
        /// </summary>
        /// <param name="e">
        /// A <see cref="TraceListenerBase.TraceEventData"/> object that contains
        /// information about the trace event.
        /// </param>
        /// <param name="data">The object to write.</param>
        protected override void TraceInternal(TraceEventData e, object data)
        {
            Send(new Log4NetEvent {
                Message = ConstructLogMessageToWrite(e, data.ToString()),
                Source = e.Source,
                Level = GetTraceEventDataTypeWithInformationAsDefault(e),
                Timestamp = e.Cache.DateTime,
                Thread = e.Cache.ThreadId ?? e.Cache.ThreadId.ToString(),
                Application = AppDomain.CurrentDomain.FriendlyName
            });
        }

        /// <summary>
        /// Sends the specified string to the remote host.
        /// </summary>
        /// <param name="data">The string to send.</param>
        private void Send(Log4NetEvent data)
        {
            try
            {
                byte[] buffer = Encoding.UTF8.GetBytes(data.Serialize());
                if (buffer.Length > MaximumDatagramSize)
                {
                    System.Diagnostics.Trace.WriteLine("Datagram exceeds maximum size (" + buffer.Length + ", max: " + MaximumDatagramSize + ")");
                }
                _client.Send(buffer, buffer.Length);
                Console.WriteLine(data.Message);
            }
            catch (SocketException ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
            }
        }

        private string ConstructLogMessageToWrite(TraceEventData e, string message)
        {
            string newMessage = "[" + e.Cache.DateTime + "] ";
            newMessage += "[" + GetTraceEventDataTypeWithInformationAsDefault(e) + "] ";
            newMessage += message;
            newMessage += " [" + e.Source + "]";
            return newMessage;
        }

        private string GetTraceEventDataTypeWithInformationAsDefault(TraceEventData e)
        {
            if (string.IsNullOrEmpty(e.Type.ToString()))
                return "Information";
            else
                return e.Type.ToString();
        }
    }

    /// <summary>
    /// Represents a log event in the log4net XML layout.
    /// </summary>
    public class Log4NetEvent
    {
        private static readonly XmlParserContext parserContext = CreateXmlParserContext();
        private static readonly XmlSerializer serializer = new XmlSerializer(typeof(Log4NetEvent));

        /// <summary>
        /// The XML namespace used by log4net events.
        /// </summary>
        public const string Log4NetEvents = "http://logging.apache.org/log4net/schemas/log4net-events-1.2";

        /// <summary>
        /// The prefix used to identify the log4net events namespace.
        /// </summary>
        public const string Log4NetPrefix = "log4net";

        /// <summary>
        /// Initializes a new instance of the <see cref="Log4NetEvent"/> class.
        /// </summary>
        public Log4NetEvent()
        {
        }

        /// <summary>
        /// Gets or sets the source that logged the event.
        /// </summary>
        [XmlAttribute("logger")]
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the date and time the event was logged.
        /// </summary>
        [XmlIgnore]
        public DateTimeOffset Timestamp { get; set; }

        /// <summary>
        /// Gets or sets a string representing the date and time the event was 
        /// logged. This property exists for compatibility with XML serialization 
        /// only and should not be used in code directly; use the 
        /// <see cref="Timestamp"/> instead.
        /// </summary>
        [XmlAttribute("timestamp")]
        public string TimestampValue
        {
            get { return Timestamp.ToString("O"); }
            set
            {
                DateTimeOffset timestamp;
                if (DateTimeOffset.TryParse(value, out timestamp))
                    Timestamp = timestamp;
            }
        }

        /// <summary>
        /// Gets or sets the level of the log event, e.g. INFO.
        /// </summary>
        [XmlAttribute("level")]
        public string Level { get; set; }

        /// <summary>
        /// Gets or sets the name or managed ID of the current thread.
        /// </summary>
        [XmlAttribute("thread")]
        public string Thread { get; set; }

        /// <summary>
        /// Gets or sets the friendly name of the current application domain.
        /// </summary>
        [XmlAttribute("domain")]
        public string Application { get; set; }

        /// <summary>
        /// Gets or sets the name of the current user.
        /// </summary>
        [XmlAttribute("username")]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the message that describes the log event.
        /// </summary>
        [XmlElement("message", Namespace = Log4NetEvents)]
        public string Message { get; set; }

        /// <summary>
        /// Returns a value that indicates whether the specified XML fragment can be
        /// deserialized as a <see cref="Log4NetEvent"/> object.
        /// </summary>
        /// <param name="xmlFragment">
        /// A string containing the log4net:event XML fragment.
        /// </param>
        /// <returns>
        /// <c>true</c> if the XML fragment can be deserialized; otherwise, 
        /// <c>false</c>.
        /// </returns>
        public static bool CanDeserialize(string xmlFragment)
        {
            using (var reader = new XmlTextReader(xmlFragment, XmlNodeType.Element, parserContext))
            {
                return serializer.CanDeserialize(reader);
            }
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Log4NetEvent"/> class with a
        /// serialized XML fragment.
        /// </summary>
        /// <param name="xmlFragment">
        /// A string containing the log4net:event XML fragment.
        /// </param>
        /// <returns>A new <see cref="Log4NetEvent"/> instance.</returns>
        public static Log4NetEvent Deserialize(string xmlFragment)
        {
            using (var reader = new XmlTextReader(xmlFragment, XmlNodeType.Element, parserContext))
            {
                return (Log4NetEvent)serializer.Deserialize(reader);
            }
        }

        /// <summary>
        /// Returns a string representation of the log event.
        /// </summary>
        /// <returns>A string that contains the log4net event XML.</returns>
        public string Serialize()
        {
            var valueBuilder = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Encoding = Encoding.UTF8,
                NewLineHandling = NewLineHandling.None,
                CheckCharacters = false
            };

            // Define the log4net namespace prefix to ensure proper XML
            var ns = new XmlSerializerNamespaces();
            ns.Add(Log4NetPrefix, Log4NetEvents);
            using (var writer = XmlWriter.Create(valueBuilder, settings))
            {
                serializer.Serialize(writer, this, ns);
            }

            return valueBuilder.ToString();
        }

        /// <summary>
        /// Returns a string that represents the log event.
        /// </summary>
        /// <returns>A string representing the log event.</returns>
        public override string ToString()
        {
            return string.Format("{0}: {1}", Source, Message);
        }

        /// <summary>
        /// Creates an <see cref="XmlParserContext"/> object that defines the 
        /// log4net namespace prefix which is used in deserialization.
        /// </summary>
        /// <returns>
        /// An <see cref="XmlParserContext"/> object that can be used by an 
        /// <see cref="XmlTextReader"/> to properly read and deserialize log4net 
        /// events.
        /// </returns>
        private static XmlParserContext CreateXmlParserContext()
        {
            var nameTable = new NameTable();
            var namespaceManager = new XmlNamespaceManager(nameTable);
            namespaceManager.AddNamespace(Log4NetPrefix, Log4NetEvents);
            return new XmlParserContext(nameTable, namespaceManager, "en", XmlSpace.None, Encoding.UTF8);
        }
    }
}
