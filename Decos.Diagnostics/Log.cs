using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Decos.Diagnostics
{
    /// <summary>
    /// Represents a log for a specific source.
    /// </summary>
    /// <typeparam name="TSource">
    /// The type (e.g. class) that acts as the source of logging information.
    /// </typeparam>
    /// <remarks>
    /// This class is generally only used to enable resolving <see cref="ILog"/>
    /// instances using dependency injection directly.
    /// </remarks>
    public class Log<TSource> : ILog<TSource>
    {
        private readonly ILog log;

        /// <summary>
        /// Initializes a new instance of the <see cref="Log{TSource}"/> class
        /// with the specified log factory.
        /// </summary>
        /// <param name="logFactory">
        /// A factory used to provide a configured <see cref="ILog"/>
        /// implementation.
        /// </param>
        public Log(ILogFactory logFactory)
        {
            if (logFactory == null)
                throw new ArgumentNullException(nameof(logFactory));

            log = logFactory.Create<TSource>();
        }

        /// <summary>
        /// Determines whether messages of the specified log level are accepted.
        /// </summary>
        /// <param name="logLevel">The log level to check.</param>
        /// <returns>
        /// <c>true</c> if message with the specified log level will be written
        /// to the log; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEnabled(LogLevel logLevel)
            => log.IsEnabled(logLevel);

        /// <summary>
        /// Writes a message to the log with the specified severity.
        /// </summary>
        /// <param name="logLevel">The severity of the message.</param>
        /// <param name="message">The text of the message to log.</param>
        public void Write(LogLevel logLevel, string message)
            => log.Write(logLevel, message);

        /// <summary>
        /// Writes structured data to the log with the specified severity.
        /// </summary>
        /// <typeparam name="T">The type of data to write.</typeparam>
        /// <param name="logLevel">The severity of the data.</param>
        /// <param name="data">The data to log.</param>
        public void Write<T>(LogLevel logLevel, T data)
            => log.Write(logLevel, data);

        /// <summary>
        /// Writes a message to the log with specified severity and customerID.
        /// </summary>
        /// <param name="logLevel">The severity of the message.</param>
        /// <param name="message">The text of the message to log.</param>
        /// <param name="customerID">The ID of the customer active on the moment of writing.</param>
        public void Write(LogLevel logLevel, string message, Guid customerID)
            => log.Write(logLevel, message, customerID);

        /// <summary>
        /// Writes structured data to the log with the specified and customerID.
        /// </summary>
        /// <typeparam name="T">The type of data to write.</typeparam>
        /// <param name="logLevel">The severity of the data.</param>
        /// <param name="data">The data to log.</param>
        /// <param name="customerID">The ID of the customer active on the moment of writing.</param>
        public void Write<T>(LogLevel logLevel, T data, Guid customerID)
            => log.Write(logLevel, data, customerID);

        /// <summary>
        /// Writes structured data to the log with the specified sender data.
        /// </summary>
        /// <typeparam name="T">The type of data to write.</typeparam>
        /// <param name="logLevel">The severity of the data.</param>
        /// <param name="data">The data to log.</param>
        /// <param name="senderDetails">Object containing data of the sender.</param>
        public void Write<T>(LogLevel logLevel, T data, LogSenderDetails senderDetails)
            => log.Write(logLevel, data, senderDetails);
    }
}