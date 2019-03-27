using System;

namespace Decos.Diagnostics
{
    /// <summary>
    /// Represents a logged message with additional structured data.
    /// </summary>
    public class LogData<T> : LogData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogData{T}"/> class with
        /// the specified message and data.
        /// </summary>
        /// <param name="message">The text of the logged message.</param>
        /// <param name="data">An object that provides additional data.</param>
        public LogData(string message, T data)
            : base(message, data)
        {
        }

        /// <summary>
        /// Gets an object that provides additional data.
        /// </summary>
        public new T Data => (T)base.Data;
    }
}