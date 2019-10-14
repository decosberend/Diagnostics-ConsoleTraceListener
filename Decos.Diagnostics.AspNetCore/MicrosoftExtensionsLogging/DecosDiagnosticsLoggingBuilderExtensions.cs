using System;
using Decos.Diagnostics.AspNetCore.MicrosoftExtensionsLogging;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// Provides a set of static methods for configuration Decos Diagnostics logging through
    /// Microsoft.Extensions.Logging.
    /// </summary>
    public static class DecosDiagnosticsLoggingBuilderExtensions
    {
        /// <summary>
        /// Adds Decos Diagnostics logging to the factory.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
        /// <returns>The logging builder.</returns>
        public static ILoggingBuilder AddDecosDiagnostics(this ILoggingBuilder builder)
        {
            builder.Services.TryAddSingleton<DecosDiagnosticsLoggerFactory>();
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, DecosDiagnosticsLoggerProvider>());
            return builder;
        }

        /// <summary>
        /// Clears all logging providers and adds Decos Diagnostics logging to the factory.
        /// </summary>
        /// <param name="builder">The <see cref="ILoggingBuilder"/> to use.</param>
        /// <returns>The logging builder.</returns>
        public static ILoggingBuilder UseDecosDiagnostics(this ILoggingBuilder builder)
        {
            builder.ClearProviders();
            builder.AddDecosDiagnostics();
            return builder;
        }
    }
}