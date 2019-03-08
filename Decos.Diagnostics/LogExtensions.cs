using System;

namespace Decos.Diagnostics
{
    /// <summary>
    /// Provides a set of static methods for writing logging events and data.
    /// </summary>
    public static class LogExtensions
    {
        /// <summary>
        /// Logs a verbose message for development or debugging purposes. These are not enabled in production by default and should only be enabled temporarily for troubleshooting.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="message">The text of the message to log.</param>
        public static void Debug(this ILog log, string message)
            => log.Write(LogLevel.Debug, message);

        /// <summary>
        /// Logs verbose data for development or debugging purposes. These are not enabled in production by default and should only be enabled temporarily for troubleshooting.
        /// </summary>
        /// <typeparam name="T">The type of data to log.</typeparam>
        /// <param name="log">The log to write to.</param>
        /// <param name="data">The data to log.</param>
        public static void Debug<T>(this ILog log, T data)
            => log.Write(LogLevel.Debug, data);

        /// <summary>
        /// Logs an informational message. These  are typically used to track the general flow of an application and should have long-term value and.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="message">The text of the message to log.</param>
        public static void Info(this ILog log, string message)
            => log.Write(LogLevel.Information, message);

        /// <summary>
        /// Logs informational data. These are typically used to track the general flow of an application and should have long-term value .
        /// </summary>
        /// <typeparam name="T">The type of data to log.</typeparam>
        /// <param name="log">The log to write to.</param>
        /// <param name="data">The data to log.</param>
        public static void Info<T>(this ILog log, T data)
            => log.Write(LogLevel.Information, data);

        /// <summary>
        /// Logs a warning message highlighting an abnormal or unexpected event.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="message">The text of the message to log.</param>
        public static void Warn(this ILog log, string message)
            => log.Write(LogLevel.Warning, message);

        /// <summary>
        /// Logs a recoverable error that does not cause the execution of an operation to stop but might need to be investigated later.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="context">A message that provides context for the error.</param>
        /// <param name="exception">The exception to log.</param>
        public static void Warn(this ILog log, string context, Exception exception)
            => log.Warn(new ExceptionData(context, exception));

        /// <summary>
        /// Logs data highlighting an abnormal or unexpected event.
        /// </summary>
        /// <typeparam name="T">The type of data to log.</typeparam>
        /// <param name="log">The log to write to.</param>
        /// <param name="data">The data to log.</param>
        public static void Warn<T>(this ILog log, T data)
            => log.Write(LogLevel.Warning, data);

        /// <summary>
        /// Logs a message indicating a failure that causes the execution of an operation to stop.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="message">The text of the message to log.</param>
        public static void Error(this ILog log, string message)
            => log.Write(LogLevel.Error, message);

        /// <summary>
        /// Logs an error that causes the execution of an operation to stop.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="context">A message that provides context for the error.</param>
        /// <param name="exception">The exception to log.</param>
        public static void Error(this ILog log, string context, Exception exception)
            => log.Error(new ExceptionData(context, exception));

        /// <summary>
        /// Logs data indicating a failure that causes the execution of an operation to stop.
        /// </summary>
        /// <typeparam name="T">The type of data to log.</typeparam>
        /// <param name="log">The log to write to.</param>
        /// <param name="data">The data to log.</param>
        public static void Error<T>(this ILog log, T data)
            => log.Write(LogLevel.Error, data);

        /// <summary>
        /// Logs a message that requires immediate attention.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="message">The text of the message to log.</param>
        public static void Critical(this ILog log, string message)
            => log.Write(LogLevel.Critical, message);

        /// <summary>
        /// Logs an error that requires immediate attention.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="context">A message that provides context for the error.</param>
        /// <param name="exception">The exception to log.</param>
        public static void Critical(this ILog log, string context, Exception exception)
            => log.Critical(new ExceptionData(context, exception));

        /// <summary>
        /// Logs data that requires immediate attention.
        /// </summary>
        /// <typeparam name="T">The type of data to log.</typeparam>
        /// <param name="log">The log to write to.</param>
        /// <param name="data">The data to log.</param>
        public static void Critical<T>(this ILog log, T data)
            => log.Write(LogLevel.Critical, data);
    }
}
