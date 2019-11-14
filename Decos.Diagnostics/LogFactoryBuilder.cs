using System;

namespace Decos.Diagnostics
{
    /// <summary>
    /// Provides <see cref="ILogFactory"/> instances. Use one of the provided
    /// extension methods to specify which infrastructure to use.
    /// </summary>
    public class LogFactoryBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogFactoryBuilder"/>
        /// class.
        /// </summary>
        public LogFactoryBuilder()
        {
        }
    }

    /// <summary>
    /// Defines a class that provides <see cref="ILogFactory"/> instances.
    /// </summary>
    /// <typeparam name="TOptions">The type of options to configure.</typeparam>
    public abstract class LogFactoryBuilder<TOptions>
        where TOptions : LogFactoryOptions, new()
    {
        /// <summary>
        /// Gets the currently configured logging options.
        /// </summary>
        protected TOptions Options { get; }
            = new TOptions();

        /// <summary>
        /// Specifies the minimum severity level that messages must have to be
        /// written to a log if no other filters match.
        /// </summary>
        /// <param name="logLevel">The minimum log level.</param>
        /// <returns>A reference to this builder.</returns>
        public virtual LogFactoryBuilder<TOptions> SetMinimumLogLevel(LogLevel logLevel)
        {
            Options.DefaultMinimumLogLevel = logLevel;
            return this;
        }

        /// <summary>
        /// Specifies the default CustomerId to send it isn't specified when writing the log itself.
        /// </summary>
        /// <param name="customerID">The new default customerID</param>
        /// <returns>A reference to this builder.</returns>
        public virtual LogFactoryBuilder<TOptions> SetStaticCustomerId(Guid customerID, Boolean threadSpecific)
        {
            if (threadSpecific)
                Options.DefaultThreadCustomerID = customerID;
            else
                Options.DefaultCustomerID = customerID;
            return this;
        }

        /// <summary>
        /// Specifies the minimum severity level that messages from sources with
        /// the specified name must have to be written to a log.
        /// </summary>
        /// <param name="name">
        /// The name of sources that the filter applies to. This follows the same
        /// rules as namespaces and can be used to configure inheritance; for
        /// example, a filter on "Decos" will also match "Decos.Diagnostics".
        /// </param>
        /// <param name="logLevel">
        /// The minimum log level messages must have.
        /// </param>
        /// <returns>A reference to this builder.</returns>
        public virtual LogFactoryBuilder<TOptions> AddFilter(SourceName name, LogLevel logLevel)
        {
            Options.Filters.Add(name, logLevel);
            return this;
        }

        /// <summary>
        /// Specifies additional logging options.
        /// </summary>
        /// <param name="configure">
        /// A method that configures the logging options to be created.
        /// </param>
        /// <returns>A reference to this builder.</returns>
        public virtual LogFactoryBuilder<TOptions> ConfigureOptions(Action<TOptions> configure)
        {
            configure(Options);
            return this;
        }

        /// <summary>
        /// Creates a new <see cref="ILogFactory"/> instance with the configured
        /// options.
        /// </summary>
        /// <returns>A new <see cref="ILogFactory"/> instance.</returns>
        public abstract ILogFactory Build();
    }
}