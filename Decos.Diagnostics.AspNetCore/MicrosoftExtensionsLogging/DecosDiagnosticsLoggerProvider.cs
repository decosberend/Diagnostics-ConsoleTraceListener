using System;

using Microsoft.Extensions.Logging;

namespace Decos.Diagnostics.AspNetCore.MicrosoftExtensionsLogging
{
    /// <summary>
    /// Provides <see cref="ILogger"/> instances that use the Decos.Diagnostics logging system.
    /// </summary>
    public sealed class DecosDiagnosticsLoggerProvider : ILoggerProvider
    {
        private readonly DecosDiagnosticsLoggerFactory _loggerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DecosDiagnosticsLoggerProvider"/> class.
        /// </summary>
        /// <param name="loggerFactory">Used to create new logger instances.</param>
        public DecosDiagnosticsLoggerProvider(DecosDiagnosticsLoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));

            _loggerFactory = loggerFactory;
        }

        /// <summary>
        /// Creates a new <see cref="ILogger"/> instance.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        /// <returns>A new <see cref="ILogger"/>.</returns>
        public ILogger CreateLogger(string categoryName)
            => _loggerFactory.CreateLogger(categoryName);

#pragma warning disable CA1063 // Implement IDisposable Correctly

        void IDisposable.Dispose()
        {
        }

#pragma warning restore CA1063 // Implement IDisposable Correctly
    }
}