using System;

namespace Decos.Diagnostics
{
    /// <summary>
    /// Represents a logged exception.
    /// </summary>
    public class ExceptionData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionData"/> class with the specified context message and exception.
        /// </summary>
        /// <param name="context">A message that provides context for the error.</param>
        /// <param name="exception">The exception that represents the error that occurred</param>
        public ExceptionData(string context, Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof(exception));

            Context = context;
            Exception = exception;
        }

        /// <summary>
        /// Gets a message that provides context for the error.
        /// </summary>
        public string Context { get; }

        /// <summary>
        /// Gets the exception that represents the error that occurred.
        /// </summary>
        public Exception Exception { get; }
    }
}
