using System;
using System.ComponentModel;

using Microsoft.Extensions.Logging;

namespace Decos.Diagnostics.AspNetCore.MicrosoftExtensionsLogging
{
    /// <summary>
    /// Represents a <see cref="Microsoft.Extensions.Logging"/> logger implementation that sends logs
    /// to an <see cref="ILog"/> instance.
    /// </summary>
    public class LoggerWrapper : ILogger
    {
        private readonly ILog _log;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerWrapper"/> class that uses the
        /// specified <see cref="ILog"/>.
        /// </summary>
        /// <param name="log">The log to use.</param>
        public LoggerWrapper(ILog log)
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log));

            _log = log;
        }

        /// <summary>
        /// Begins a logical operation scope. This method is not implemented.
        /// </summary>
        /// <param name="state">The identifier for the scope.</param>
        /// <typeparam name="TState">The type of the state to begin scope for.</typeparam>
        /// <returns>
        /// An <see cref="IDisposable"/> that ends the logical operation scope on dispose.
        /// </returns>
        public IDisposable BeginScope<TState>(TState state)
            => new Disposable();

        /// <summary>
        /// Checks if the given <paramref name="logLevel"/> is enabled.
        /// </summary>
        /// <param name="logLevel">level to be checked.</param>
        /// <returns><c>true</c> if enabled.</returns>
        public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
            => _log.IsEnabled(Translate(logLevel));

        /// <summary>
        /// Writes a log entry.
        /// </summary>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="eventId">Id of the event. This parameter is not used.</param>
        /// <param name="state">The entry to be written. Can be also an object.</param>
        /// <param name="exception">The exception related to this entry.</param>
        /// <param name="formatter">
        /// Function to create a <see cref="string"/> message of the <paramref name="state"/> and
        /// <paramref name="exception"/>.
        /// </param>
        /// <typeparam name="TState">The type of the object to be written.</typeparam>
        public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            var level = Translate(logLevel);
            var message = formatter(state, exception);
            if (exception != null)
                _log.Write(level, message, exception);
            else
                _log.Write(level, message, state);
        }

        private static LogLevel Translate(Microsoft.Extensions.Logging.LogLevel logLevel)
        {
            switch (logLevel)
            {
                case Microsoft.Extensions.Logging.LogLevel.Trace:
                case Microsoft.Extensions.Logging.LogLevel.Debug:
                    return LogLevel.Debug;

                case Microsoft.Extensions.Logging.LogLevel.Information:
                    return LogLevel.Information;

                case Microsoft.Extensions.Logging.LogLevel.Warning:
                    return LogLevel.Warning;

                case Microsoft.Extensions.Logging.LogLevel.Error:
                    return LogLevel.Error;

                case Microsoft.Extensions.Logging.LogLevel.Critical:
                    return LogLevel.Critical;

                case Microsoft.Extensions.Logging.LogLevel.None:
                    return LogLevel.None;

                default:
                    throw new InvalidEnumArgumentException(nameof(logLevel), (int)logLevel, typeof(Microsoft.Extensions.Logging.LogLevel));
            }
        }
    }

    /// <summary>
    /// Represents a <see cref="Microsoft.Extensions.Logging"/> logger implementation that sends logs
    /// to an <see cref="ILog"/> instance for the specified type.
    /// </summary>
    /// <typeparam name="T">The type who's name is used for the category name.</typeparam>
    public class LoggerWrapper<T> : LoggerWrapper, ILogger<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerWrapper{T}"/> class that uses the
        /// specified log factory.
        /// </summary>
        /// <param name="logFactory">
        /// Used to create <see cref="ILog"/> instances to write wrapped logs to.
        /// </param>
        public LoggerWrapper(ILogFactory logFactory)
            : base(logFactory.Create<T>())
        {
        }
    }
}