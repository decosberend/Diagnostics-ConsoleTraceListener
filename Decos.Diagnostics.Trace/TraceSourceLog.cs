using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Decos.Diagnostics.Trace
{
    /// <summary>
    /// Represents a log backed by a <see cref="TraceSource"/>.
    /// </summary>
    public class TraceSourceLog : ILog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TraceSourceLog"/> class
        /// with the specified <see cref="System.Diagnostics.TraceSource"/>.
        /// </summary>
        /// <param name="traceSource">
        /// The <see cref="System.Diagnostics.TraceSource"/> to write logging 
        /// events and data to.
        /// </param>
        public TraceSourceLog(TraceSource traceSource)
        {
            TraceSource = traceSource;
            DefaultCustomerID = new Guid(TraceSource.Attributes["customerid"]);
        }

        /// <summary>
        /// Gets the underlying <see cref="System.Diagnostics.TraceSource"/>
        /// that logging events and data are written to.
        /// </summary>
        public TraceSource TraceSource { get; }

        public Guid DefaultCustomerID { get; set; }

        /// <summary>
        /// Determines whether messages of the specified log level are accepted.
        /// </summary>
        /// <param name="logLevel">The log level to check.</param>
        /// <returns>
        /// <c>true</c> if message with the specified log level will be written
        /// to the log; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEnabled(LogLevel logLevel)
        {
            var eventType = logLevel.ToTraceEventType();
            if (eventType == null)
                return false;

            return TraceSource.Switch.ShouldTrace(eventType.Value);
        }

        /// <summary>
        /// Writes a message to the log with the specified severity.
        /// </summary>
        /// <param name="logLevel">The severity of the message.</param>
        /// <param name="message">The text of the message to log.</param>
        public virtual void Write(LogLevel logLevel, string message)
        {
            var eventType = logLevel.ToTraceEventType();
            if (eventType == null)
                return;

            Write(eventType.Value, message);
        }

        /// <summary>
        /// Writes a message to the log with the specified severity.
        /// </summary>
        /// <param name="eventType">The severity of the message.</param>
        /// <param name="message">The text of the message to log.</param>
        public virtual void Write(TraceEventType eventType, string message)
        {
            if (Guid.TryParse(TraceSource.Attributes["customerid"], out Guid customerId)) {
                var objectToLog = new CustomerLogData()
                {
                    CustomerId = customerId,
                    Data = message
                };
                Write(eventType, objectToLog);
            }
            else
                TraceSource.TraceEvent(eventType, 0, message);
        }

        /// <summary>
        /// Writes structured data to the log with the specified severity.
        /// </summary>
        /// <typeparam name="T">The type of data to write.</typeparam>
        /// <param name="logLevel">The severity of the data.</param>
        /// <param name="data">The data to log.</param>
        public virtual void Write<T>(LogLevel logLevel, T data)
        {
            var eventType = logLevel.ToTraceEventType();
            if (eventType == null)
                return;
            if (CustomerLogData.TryParseFromData(TraceSource, data, out CustomerLogData customerLogData))
                Write(eventType.Value, customerLogData);
            else Write(eventType.Value, data);
        }

        /// <summary>
        /// Writes structured data to the log with the specified severity.
        /// </summary>
        /// <typeparam name="T">The type of data to write.</typeparam>
        /// <param name="eventType">The severity of the data.</param>
        /// <param name="data">The data to log.</param>
        public virtual void Write<T>(TraceEventType eventType, T data)
        {
            TraceSource.TraceData(eventType, 0, data);
        }

        public void Write(LogLevel logLevel, string message, Guid customerID)
        {
            throw new NotImplementedException();
        }
    }

    public class CustomerLogData
    {
        public Guid? CustomerId { get; set; }
        public object Data { get; set; }

        public override string ToString()
        {
            if (CustomerId.HasValue)
            {
                return CustomerId.ToString() + " " + Data?.ToString();
            }
            return Data?.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool TryParseFromMessage(TraceSource source, string message, out CustomerLogData customerLogData)
        {
            customerLogData = null;
            return false;
        }

        internal static object TryExtractCustomerId(object data, out Guid? customerId)
        {
            customerId = null;
            if (data != null && data is CustomerLogData customerData)
            {
                customerId = customerData.CustomerId;
                return customerData.Data;
            }
            else return data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool TryParseFromData(TraceSource source, object data, out CustomerLogData customerLogData)
        {
            if (Guid.TryParse(source.Attributes["customerid"], out Guid customerId)
                && (!(data is CustomerLogData)))
            {
                customerLogData = new CustomerLogData()
                {
                    CustomerId = customerId,
                    Data = data
                };
                return true;
            }
            customerLogData = null;
            return false;
        }
    }
}
