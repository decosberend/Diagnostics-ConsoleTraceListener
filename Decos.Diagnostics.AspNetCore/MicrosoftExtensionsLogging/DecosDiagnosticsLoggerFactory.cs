using System;

using Microsoft.Extensions.Logging;

namespace Decos.Diagnostics.AspNetCore.MicrosoftExtensionsLogging
{
    /// <summary>
    /// Provides <see cref="ILogger"/> instances that use the Decos.Diagnostics logging system.
    /// </summary>
    public sealed class DecosDiagnosticsLoggerFactory : ILoggerFactory
    {
        private readonly ILogFactory _logFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DecosDiagnosticsLoggerFactory"/> class that
        /// uses the specified <see cref="ILogFactory"/>.
        /// </summary>
        /// <param name="logFactory">
        /// Used to create new <see cref="DecosDiagnosticsLogger"/> instances.
        /// </param>
        public DecosDiagnosticsLoggerFactory(ILogFactory logFactory)
        {
            _logFactory = logFactory;
        }

        /// <summary>
        /// Creates a new <see cref="ILogger"/> instance.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        /// <returns>The <see cref="ILogger"/>.</returns>
        public ILogger CreateLogger(string categoryName)
        {
            var log = _logFactory.Create(categoryName);
            return new DecosDiagnosticsLogger(log);
        }

        void ILoggerFactory.AddProvider(ILoggerProvider provider)
        {
        }

#pragma warning disable CA1063 // Implement IDisposable Correctly

        void IDisposable.Dispose()
        {
        }

#pragma warning restore CA1063 // Implement IDisposable Correctly
    }
}