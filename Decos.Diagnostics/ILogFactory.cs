using System.Threading;
using System.Threading.Tasks;

namespace Decos.Diagnostics
{
    /// <summary>
    /// Defines methods for creating <see cref="ILog"/> instances.
    /// </summary>
    public interface ILogFactory
    {
        /// <summary>
        /// Creates a new <see cref="ILog"/> instance that writes logging
        /// information for the specified type.
        /// </summary>
        /// <typeparam name="T">
        /// The type (e.g. class) that acts as the source of logging information.
        /// </typeparam>
        /// <returns>
        /// A new <see cref="ILog"/> instance for the specified type.
        /// </returns>
        ILog Create<T>();

        /// <summary>
        /// Creates a new <see cref="ILog"/> instance that write logging
        /// information with the specified name.
        /// </summary>
        /// <param name="name">
        /// A name for the source of the logging information.
        /// </param>
        /// <returns>
        /// A new <see cref="ILog"/> instance with the specified name.
        /// </returns>
        ILog Create(SourceName name);

        /// <summary>
        /// Waits for any long-running logging operations to shut down
        /// gracefully.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task ShutdownAsync();

        /// <summary>
        /// Waits for any long-running logging operations to shut down
        /// gracefully.
        /// </summary>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests. If you cancel the
        /// shutdown, some logs may be lost.
        /// </param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task ShutdownAsync(CancellationToken cancellationToken);
    }
}