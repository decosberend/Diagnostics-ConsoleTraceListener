using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;

namespace Decos.Diagnostics.AspNetCore
{
    /// <summary>
    /// Ensures long-running logging operations are finished when an application
    /// is shutting down.
    /// </summary>
    public class ApplicationShutdownHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="ApplicationShutdownHandler"/> class for the specified log
        /// factory and application lifetime.
        /// </summary>
        /// <param name="logFactory">
        /// The log factory to shut down gracefully.
        /// </param>
        /// <param name="applicationLifetime">
        /// The lifetime of the application to monitor.
        /// </param>
        [Obsolete("IApplicationLifetime is obsolete. Use the overload that accepts IHostApplicationLifetime instead.")]
        public ApplicationShutdownHandler(ILogFactory logFactory, IApplicationLifetime applicationLifetime)
        {
            LogFactory = logFactory;

            applicationLifetime.ApplicationStopping.Register(OnShutdown);
        }

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="ApplicationShutdownHandler"/> class for the specified log
        /// factory and application lifetime.
        /// </summary>
        /// <param name="logFactory">
        /// The log factory to shut down gracefully.
        /// </param>
        /// <param name="applicationLifetime">
        /// The lifetime of the application to monitor.
        /// </param>
        public ApplicationShutdownHandler(ILogFactory logFactory, IHostApplicationLifetime applicationLifetime)
        {
            LogFactory = logFactory;

            applicationLifetime.ApplicationStopping.Register(OnShutdown);
        }

        /// <summary>
        /// Gets the log factory to shut down gracefully.
        /// </summary>
        protected ILogFactory LogFactory { get; }

        /// <summary>
        /// Logs a message when the application host is shutting down and ensures
        /// long-running logging operations have finished.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected virtual void OnShutdown()
        {
            var log = LogFactory.Create<ApplicationShutdownHandler>();
            log.Info("Application host is shutting down.");

            LogFactory.ShutdownAsync().GetAwaiter().GetResult();
        }
    }
}