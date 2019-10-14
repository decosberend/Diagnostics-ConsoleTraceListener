using System;
using Microsoft.Extensions.Logging;

namespace Decos.Diagnostics.AspNetCore.MicrosoftExtensionsLogging
{
    /// <summary>
    /// Provides <see cref="ILogger"/> instances that use the Decos.Diagnostics logging system.
    /// </summary>
    public sealed class LoggerFactoryWrapper : ILoggerFactory
    {
        private readonly ILogFactory _logFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerFactoryWrapper"/> class that uses the
        /// specified <see cref="ILogFactory"/>.
        /// </summary>
        /// <param name="logFactory">
        /// Used to create new <see cref="LoggerWrapper"/> instances.
        /// </param>
        public LoggerFactoryWrapper(ILogFactory logFactory)
        {
            _logFactory = logFactory;
        }

        /// <summary>
        /// Adds an <see cref="ILoggerProvider"/> to the logging system.
        /// </summary>
        /// <param name="provider">The <see cref="ILoggerProvider"/>.</param>
        public void AddProvider(ILoggerProvider provider)
        {
        }

        /// <summary>
        /// Creates a new <see cref="ILogger"/> instance.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        /// <returns>The <see cref="ILogger"/>.</returns>
        public ILogger CreateLogger(string categoryName)
        {
            var log = _logFactory.Create(categoryName);
            return new LoggerWrapper(log);
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        public void Dispose()
        {
        }
    }
}