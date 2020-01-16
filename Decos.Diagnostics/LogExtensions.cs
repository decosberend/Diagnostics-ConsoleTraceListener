using System;

namespace Decos.Diagnostics
{
    /// <summary>
    /// Provides a set of static methods for writing logging events and data.
    /// </summary>
    public static class LogExtensions
    {
        /// <summary>
        /// Writes a message to the log with the specified severity and additional data.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="logLevel">The severity of the message.</param>
        /// <param name="message">The text of the message to log.</param>
        /// <param name="data">An object that provides additional data.</param>
        public static void Write<T>(this ILog log, LogLevel logLevel, string message, T data)
            => log.Write(logLevel, new LogData<T>(message, data));

        /// <summary>
        /// Writes a message to the log with the specified severity, additional data and a specific customerID.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="logLevel">The severity of the message.</param>
        /// <param name="message">The text of the message to log.</param>
        /// <param name="data">An object that provides additional data.</param>
        /// <param name="customerID">The specific customerID to send with the log.</param>
        public static void Write<T>(this ILog log, LogLevel logLevel, string message, T data, Guid customerID)
            => log.Write(logLevel, new LogData<T>(message, data), customerID);

        /// <summary>
        /// Writes a message to the log with the specified severity, additional data and specific data from the sender.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="logLevel">The severity of the message.</param>
        /// <param name="message">The text of the message to log.</param>
        /// <param name="data">An object that provides additional data.</param>
        /// <param name="senderDetails">Object containing data from the sender.</param>
        public static void Write<T>(this ILog log, LogLevel logLevel, string message, T data, LoggerContext senderDetails)
            => log.Write(logLevel, new LogData<T>(message, data), senderDetails);

        /// <summary>
        /// Writes a message to the log with the specified severity and a specific customerID.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="logLevel">The severity of the message.</param>
        /// <param name="message">The text of the message to log.</param>
        /// <param name="customerID">The specific customerID to send with the log.</param>
        public static void Write(this ILog log, LogLevel logLevel, string message, Guid customerID)
        {
            CustomerLogData data = new CustomerLogData(message, customerID);
            log.Write(logLevel, data);
        }

        /// <summary>
        /// Writes an object to the log with the specified severity and a specific customerID.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="logLevel">The severity of the message.</param>
        /// <param name="objectToSend">The data to log.</param>
        /// <param name="customerID">The specific customerID to send with the log.</param>
        public static void Write<T>(this ILog log, LogLevel logLevel, T objectToSend, Guid customerID)
        {
            CustomerLogData data = new CustomerLogData(objectToSend, customerID);
            log.Write(logLevel, data);
        }

        /// <summary>
        /// Writes a message to the log with the specified severity and a specific customerID.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="logLevel">The severity of the message.</param>
        /// <param name="message">The text of the message to log.</param>
        /// <param name="senderDetails">Object containing data of the sender.</param>
        public static void Write(this ILog log, LogLevel logLevel, string message, LoggerContext senderDetails)
        {
            log.Write(logLevel, new CustomerLogData(message, senderDetails.CustomerId, senderDetails.SessionId));
        }

        /// <summary>
        /// Writes an object to the log with the specified severity and a specific customerID.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="logLevel">The severity of the message.</param>
        /// <param name="objectToSend">The data to log.</param>
        /// <param name="senderDetails">Object containing data of the sender.</param>
        public static void Write<T>(this ILog log, LogLevel logLevel, T objectToSend, LoggerContext senderDetails)
        {
            if (senderDetails.HasCustomerId() || senderDetails.HasSessionId())
                log.Write(logLevel, new CustomerLogData(objectToSend, senderDetails.CustomerId, senderDetails.SessionId));
            else
                log.Write(logLevel, objectToSend);
        }

        /// <summary>
        /// Logs a verbose message for development or debugging purposes. These are not enabled in production by default and should only be enabled temporarily for troubleshooting.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="message">The text of the message to log.</param>
        public static void Debug(this ILog log, string message)
            => log.Write(LogLevel.Debug, message);

        /// <summary>
        /// Logs a verbose message for development or debugging purposes. These are not enabled in production by default and should only be enabled temporarily for troubleshooting.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="message">The text of the message to log.</param>
        /// <param name="data">An object that provides additional data.</param>
        public static void Debug<T>(this ILog log, string message, T data)
            => log.Write(LogLevel.Debug, message, data);

        /// <summary>
        /// Logs verbose data for development or debugging purposes. These are not enabled in production by default and should only be enabled temporarily for troubleshooting.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="message">The text of the message to log</param>
        /// <param name="customerID">The ID of the customer who was active when writing the log.</param>
        public static void Debug(this ILog log, string message, Guid customerID)
            => log.Write(LogLevel.Debug, message, customerID);

        /// <summary>
        /// Logs verbose data for development or debugging purposes. These are not enabled in production by default and should only be enabled temporarily for troubleshooting.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="message">The text of the message to log.</param>
        /// <param name="senderDetails">Object containing data of the sender.</param>
        public static void Debug(this ILog log, string message, LoggerContext senderDetails)
            => log.Write(LogLevel.Debug, message, senderDetails);

        /// <summary>
        /// Logs verbose data for development or debugging purposes. These are not enabled in production by default and should only be enabled temporarily for troubleshooting.
        /// </summary>
        /// <typeparam name="T">The type of data to log.</typeparam>
        /// <param name="log">The log to write to.</param>
        /// <param name="data">The data to log.</param>
        public static void Debug<T>(this ILog log, T data)
            => log.Write(LogLevel.Debug, data);

        /// <summary>
        /// ogs verbose data for development or debugging purposes. These are not enabled in production by default and should only be enabled temporarily for troubleshooting.
        /// </summary>
        /// <typeparam name="T">The type of data to log.</typeparam>
        /// <param name="log">The log to write to.</param>
        /// <param name="data">The data to log.</param>
        /// <param name="customerID">The ID of the customer who was active when writing the log.</param>
        public static void Debug<T>(this ILog log, T data, Guid customerID)
            => log.Write(LogLevel.Debug, data, customerID);

        /// <summary>
        /// Logs verbose data for development or debugging purposes. These are not enabled in production by default and should only be enabled temporarily for troubleshooting.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="data">The data to log.</param>
        /// <param name="senderDetails">Object containing data of the sender.</param>
        public static void Debug<T>(this ILog log, T data, LoggerContext senderDetails)
            => log.Write(LogLevel.Debug, data, senderDetails);

        /// <summary>
        /// Logs an informational message. These are typically used to track the general flow of an application and should have long-term value and.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="message">The text of the message to log.</param>
        public static void Info(this ILog log, string message)
            => log.Write(LogLevel.Information, message);

        /// <summary>
        /// Logs an informational message. These are typically used to track the general flow of an application and should have long-term value and.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="message">The text of the message to log.</param>
        /// <param name="data">An object that provides additional data.</param>
        public static void Info<T>(this ILog log, string message, T data)
            => log.Write(LogLevel.Information, message, data);

        /// <summary>
        /// Logs an informational message. These are typically used to track the general flow of an application and should have long-term value and.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="message">The text of the message to log.</param>
        /// <param name="customerID">The ID of the customer who was active when writing the log.</param>
        public static void Info(this ILog log, string message, Guid customerID)
            => log.Write(LogLevel.Information, message, customerID);

        /// <summary>
        /// Logs an informational message. These are typically used to track the general flow of an application and should have long-term value and.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="message">The text of the message to log.</param>
        /// <param name="senderDetails">Object containing data of the sender.</param>
        public static void Info(this ILog log, string message, LoggerContext senderDetails)
            => log.Write(LogLevel.Information, message, senderDetails);

        /// <summary>
        /// Logs informational data. These are typically used to track the general flow of an application and should have long-term value .
        /// </summary>
        /// <typeparam name="T">The type of data to log.</typeparam>
        /// <param name="log">The log to write to.</param>
        /// <param name="data">The data to log.</param>
        public static void Info<T>(this ILog log, T data)
            => log.Write(LogLevel.Information, data);

        /// <summary>
        /// Logs an informational message. These are typically used to track the general flow of an application and should have long-term value and.
        /// </summary>
        /// <typeparam name="T">The type of data to log.</typeparam>
        /// <param name="log">The log to write to.</param>
        /// <param name="data">The data to log.</param>
        /// <param name="customerID">The ID of the customer who was active when writing the log.</param>
        public static void Info<T>(this ILog log, T data, Guid customerID)
            => log.Write(LogLevel.Information, data, customerID);

        /// <summary>
        /// Logs an informational message. These are typically used to track the general flow of an application and should have long-term value and.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="data">The data to log.</param>
        /// <param name="senderDetails">Object containing data of the sender.</param>
        public static void Info<T>(this ILog log, T data, LoggerContext senderDetails)
            => log.Write(LogLevel.Information, data, senderDetails);

        /// <summary>
        /// Logs a warning message highlighting an abnormal or unexpected event.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="message">The text of the message to log.</param>
        public static void Warn(this ILog log, string message)
            => log.Write(LogLevel.Warning, message);

        /// <summary>
        /// Logs a warning message highlighting an abnormal or unexpected event, such as a recoverable error that does not cause the execution of an operation to stop but might need to be investigated later.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="message">The text of the message to log.</param>
        /// <param name="data">An object that provides additional data.</param>
        public static void Warn<T>(this ILog log, string message, T data)
            => log.Write(LogLevel.Warning, message, data);

        /// <summary>
        /// Logs a warning message highlighting an abnormal or unexpected event, such as a recoverable error that does not cause the execution of an operation to stop but might need to be investigated later.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="message">The text of the message to log.</param>
        /// <param name="customerID">The ID of the customer who was active when writing the log.</param>
        public static void Warn(this ILog log, string message, Guid customerID)
            => log.Write(LogLevel.Warning, message, customerID);

        /// <summary>
        /// Logs a warning message highlighting an abnormal or unexpected event, such as a recoverable error that does not cause the execution of an operation to stop but might need to be investigated later.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="message">The text of the message to log.</param>
        /// <param name="senderDetails">Object containing data of the sender.</param>
        public static void Warn(this ILog log, string message, LoggerContext senderDetails)
            => log.Write(LogLevel.Warning, message, senderDetails);

        /// <summary>
        /// Logs data highlighting an abnormal or unexpected event.
        /// </summary>
        /// <typeparam name="T">The type of data to log.</typeparam>
        /// <param name="log">The log to write to.</param>
        /// <param name="data">The data to log.</param>
        public static void Warn<T>(this ILog log, T data)
            => log.Write(LogLevel.Warning, data);

        /// <summary>
        /// Logs a warning message highlighting an abnormal or unexpected event, such as a recoverable error that does not cause the execution of an operation to stop but might need to be investigated later.
        /// </summary>
        /// <typeparam name="T">The type of data to log.</typeparam>
        /// <param name="log">The log to write to.</param>
        /// <param name="data">The data to log.</param>
        /// <param name="customerID">The ID of the customer who was active when writing the log.</param>
        public static void Warn<T>(this ILog log, T data, Guid customerID)
            => log.Write(LogLevel.Warning, data, customerID);

        /// <summary>
        /// Logs a warning message highlighting an abnormal or unexpected event, such as a recoverable error that does not cause the execution of an operation to stop but might need to be investigated later.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="data">The data to log.</param>
        /// <param name="senderDetails">Object containing data of the sender.</param>
        public static void Warn<T>(this ILog log, T data, LoggerContext senderDetails)
            => log.Write(LogLevel.Warning, data, senderDetails);

        /// <summary>
        /// Logs a message indicating a failure that causes the execution of an operation to stop.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="message">The text of the message to log.</param>
        public static void Error(this ILog log, string message)
            => log.Write(LogLevel.Error, message);

        /// <summary>
        /// Logs a message indicating a failure that causes the execution of an operation to stop.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="message">The text of the message to log.</param>
        /// <param name="data">An object that provides additional data.</param>
        public static void Error<T>(this ILog log, string message, T data)
            => log.Write(LogLevel.Error, message, data);

        /// <summary>
        /// Logs a message indicating a failure that causes the execution of an operation to stop.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="message">The text of the message to log.</param>
        /// <param name="customerID">The ID of the customer who was active when writing the log.</param>
        public static void Error(this ILog log, string message, Guid customerID)
            => log.Write(LogLevel.Error, message, customerID);

        /// <summary>
        /// Logs a message indicating a failure that causes the execution of an operation to stop.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="message">The text of the message to log.</param>
        /// <param name="senderDetails">Object containing data of the sender.</param>
        public static void Error(this ILog log, string message, LoggerContext senderDetails)
            => log.Write(LogLevel.Error, message, senderDetails);

        /// <summary>
        /// Logs data indicating a failure that causes the execution of an operation to stop.
        /// </summary>
        /// <typeparam name="T">The type of data to log.</typeparam>
        /// <param name="log">The log to write to.</param>
        /// <param name="data">The data to log.</param>
        public static void Error<T>(this ILog log, T data)
            => log.Write(LogLevel.Error, data);

        /// <summary>
        /// Logs a message indicating a failure that causes the execution of an operation to stop.
        /// </summary>
        /// <typeparam name="T">The type of data to log.</typeparam>
        /// <param name="log">The log to write to.</param>
        /// <param name="data">The data to log.</param>
        /// <param name="customerID">The ID of the customer who was active when writing the log.</param>
        public static void Error<T>(this ILog log, T data, Guid customerID)
            => log.Write(LogLevel.Error, data, customerID);

        /// <summary>
        /// Logs a message indicating a failure that causes the execution of an operation to stop.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="data">The data to log.</param>
        /// <param name="senderDetails">Object containing data of the sender.</param>
        public static void Error<T>(this ILog log, T data, LoggerContext senderDetails)
            => log.Write(LogLevel.Error, data, senderDetails);

        /// <summary>
        /// Logs a message that requires immediate attention.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="message">The text of the message to log.</param>
        public static void Critical(this ILog log, string message)
            => log.Write(LogLevel.Critical, message);

        /// <summary>
        /// Logs a message that requires immediate attention.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="message">The text of the message to log.</param>
        /// <param name="data">An object that provides additional data.</param>
        public static void Critical<T>(this ILog log, string message, T data)
            => log.Write(LogLevel.Critical, message, data);

        /// <summary>
        /// Logs a message that requires immediate attention.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="message">The text of the message to log.</param>
        /// <param name="customerID">The ID of the customer who was active when writing the log.</param>
        public static void Critical(this ILog log, string message, Guid customerID)
            => log.Write(LogLevel.Critical, message, customerID);

        /// <summary>
        /// Logs a message that requires immediate attention.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="message">The text of the message to log.</param>
        /// <param name="senderDetails">Object containing data of the sender.</param>
        public static void Critical(this ILog log, string message, LoggerContext senderDetails)
            => log.Write(LogLevel.Critical, message, senderDetails);

        /// <summary>
        /// Logs data that requires immediate attention.
        /// </summary>
        /// <typeparam name="T">The type of data to log.</typeparam>
        /// <param name="log">The log to write to.</param>
        /// <param name="data">The data to log.</param>
        public static void Critical<T>(this ILog log, T data)
            => log.Write(LogLevel.Critical, data);

        /// <summary>
        /// Logs a message that requires immediate attention.
        /// </summary>
        /// <typeparam name="T">The type of data to log.</typeparam>
        /// <param name="log">The log to write to.</param>
        /// <param name="data">The data to log.</param>
        /// <param name="customerID">The ID of the customer who was active when writing the log.</param>
        public static void Critical<T>(this ILog log, T data, Guid customerID)
            => log.Write(LogLevel.Critical, data, customerID);

        /// <summary>
        /// Logs a message that requires immediate attention.
        /// </summary>
        /// <param name="log">The log to write to.</param>
        /// <param name="data">The data to log.</param>
        /// <param name="senderDetails">Object containing data of the sender.</param>
        public static void Critical<T>(this ILog log, T data, LoggerContext senderDetails)
            => log.Write(LogLevel.Critical, data, senderDetails);
    }
}
