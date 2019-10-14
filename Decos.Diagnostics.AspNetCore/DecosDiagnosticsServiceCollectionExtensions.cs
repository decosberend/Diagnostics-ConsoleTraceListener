using System;

using Decos.Diagnostics;
using Decos.Diagnostics.AspNetCore;
using Decos.Diagnostics.AspNetCore.MicrosoftExtensionsLogging;
using Decos.Diagnostics.Trace;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Provides a set of static methods for adding Decos Diagnostics logging
    /// services.
    /// </summary>
    public static class DecosDiagnosticsServiceCollectionExtensions
    {
        /// <summary>
        /// Adds logging services using the <see
        /// cref="System.Diagnostics.TraceSource"/> infrastructure with the
        /// default options.
        /// </summary>
        /// <param name="services">
        /// The service collection to add the services to.
        /// </param>
        /// <returns>A reference to the service collection.</returns>
        public static IServiceCollection AddTraceSourceLogging(
            this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            return services.AddTraceSourceLogging(_ => { });
        }

        /// <summary>
        /// Adds logging services using the <see
        /// cref="System.Diagnostics.TraceSource"/> infrastructure.
        /// </summary>
        /// <param name="services">
        /// The service collection to add the services to.
        /// </param>
        /// <param name="configure">
        /// A delegate to configure the logging options.
        /// </param>
        /// <returns>A reference to the service collection.</returns>
        public static IServiceCollection AddTraceSourceLogging(
            this IServiceCollection services,
            Action<TraceSourceLogFactoryBuilder> configure)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var builder = new LogFactoryBuilder()
                .UseTraceSource();
            configure(builder);
            var logFactory = builder.Build();

            services.AddSingleton(logFactory);
            services.AddTransient(typeof(ILog<>), typeof(Log<>));
            services.AddTransient(typeof(ILogger<>), typeof(LoggerWrapper<>));
            services.AddTransient(typeof(ILoggerFactory), typeof(LoggerFactoryWrapper));
            services.AddSingleton<ApplicationShutdownHandler>();

            return services;
        }
    }
}