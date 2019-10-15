using System;
using System.Collections;
using System.Linq;

namespace Decos.Diagnostics
{
    /// <summary>
    /// Represents a logged message with additional structured data.
    /// </summary>
    public class LogData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogData"/> class with the specified message
        /// and data.
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
        {
            var data = Format(Data);
            if (string.IsNullOrWhiteSpace(data))
                return Message;

            return $"{Message}\n{data}";
        }

        /// <summary>
        /// Returns a string that represents the additional data.
        /// </summary>
        /// <param name="data">The data to format.</param>
        /// <returns>A new string.</returns>
        protected virtual string Format(object data)
        {
            try
            {
                switch (data)
                {
                    // FormattedLogValues is used for message templates in ASP.NET Core logging and
                    // won't add any value when printed over the message itself
                    case FormattedLogValues _:
                    case null:
                        return null;

                    // We will want to show any other type of dictionary, though
                    case IDictionary dictionary:
                        return string.Join(", ", dictionary.Keys.OfType<object>().Select(key => $"{key}: {dictionary[key]}"));

                    case object[] items:
                        return string.Join(", ", items);
                }
            }
            catch { }
            return data?.ToString();
        }
    }
}