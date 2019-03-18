namespace Decos.Diagnostics
{
    /// <summary>
    /// Defines severity levels of log messages.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Not used for writing log messages. Specifies that a logging category
        /// should not write any messages.
        /// </summary>
        None = 0,

        /// <summary>
        /// Logs that may be useful in development and debugging. These logs
        /// should only be enabled in production during troubleshooting.
        /// </summary>
        Debug,

        /// <summary>
        /// Logs that track the general flow of the application. These logs
        /// should have long-term value.
        /// </summary>
        Information,

        /// <summary>
        /// Logs that highlight an abnormal or unexpected event in the
        /// application flow. These logs may include errors that do not cause the
        /// operation to stop but might need to be investigated, such as handled
        /// exceptions.
        /// </summary>
        Warning,

        /// <summary>
        /// Logs that highlight when the current flow of execution is stopped due
        /// to a failure. These should indicate a failure in the current
        /// activity, not an application-wide failure.
        /// </summary>
        Error,

        /// <summary>
        /// Logs that describe an unrecoverable application or system crash, or
        /// anything else that requires immediate attention.
        /// </summary>
        Critical,
    }
}