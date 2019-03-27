using System;

namespace Decos.Diagnostics
{
    /// <summary>
    /// Represents a logged message with additional structured data.
    /// </summary>
    public class LogData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogData"/> class with
        /// the specified message and data.
        /// </summary>
        /// <param name="message">The text of the logged message.</param>
        /// <param name="data">An object that provides additional data.</param>
        public LogData(string message, object data)
        {
            Message = message;
            Data = data;
        }

        /// <summary>
        /// Gets the text of the logged message.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets an object that provides additional data.
        /// </summary>
        public object Data { get; }

        /// <summary>
        /// Returns a string that represents the logged message.
        /// </summary>
        /// <returns>A string that represents this logged message.</returns>
        public override string ToString()
            => $"{Message}\n{Data}";
    }
}