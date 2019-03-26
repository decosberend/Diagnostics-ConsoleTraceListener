using Microsoft.AspNetCore.Hosting;

namespace Decos.Diagnostics.AspNetCore
{
    /// <summary>
    /// Provides a set of static methods for configuring the web host for loggin.
    /// </summary>
    public static class WebHostExtensions
    {
        /// <summary>
        /// Enables graceful shutdown of the logging system by ensuring
        /// long-running logging operations have finished before shutting down
        /// the application host.
        /// </summary>
        /// <param name="host">The configured web host.</param>
        /// <returns>A reference to the configured web host.</returns>
        public static IWebHost FlushLogsOnShutdown(this IWebHost host)
            => host.FlushLogsOnShutdown<ApplicationShutdownHandler>();

        /// <summary>
        /// Enables graceful shutdown of the logging system by ensuring
        /// long-running logging operations have finished before shutting down
        /// the application host.
        /// </summary>
        /// <typeparam name="T">
        /// The type that handles the application shutdown event.
        /// </typeparam>
        /// <param name="host">The configured web host.</param>
        /// <returns>A reference to the configured web host.</returns>
        public static IWebHost FlushLogsOnShutdown<T>(this IWebHost host)
            where T : ApplicationShutdownHandler
        {
            // Ensures the shutdown handler is initialized to allow graceful
            // shutdown of logs. This can be tested locally on IIS Express using
            // the tray icon ("Stop Site") or Ctrl+C in the Kestrel command
            // prompt.
            host.Services.GetService(typeof(T));
            return host;
        }
    }
}