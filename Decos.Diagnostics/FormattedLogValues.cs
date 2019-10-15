using System;
using System.Collections.Generic;
using System.Linq;

namespace Decos.Diagnostics
{
    /// <summary>
    /// Represents a collection of values passed to logged message templates in ASP.NET Core logging.
    /// </summary>
    public class FormattedLogValues : Dictionary<string, object>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormattedLogValues"/> class with the
        /// specified formatted log values.
        /// </summary>
        /// <param name="items">The internal FormattedLogValues object.</param>
        public FormattedLogValues(IReadOnlyList<KeyValuePair<string, object>> items)
        {
            if (items.Count <= 1)
            {
                return;
            }

            // FormattedLogValues always inserts the original format at the end.
            foreach (var keyValuePair in items.Take(items.Count - 1))
                Add(keyValuePair.Key, keyValuePair.Value);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return string.Join(", ", this.Select(x => $"{x.Key}: {x.Value}"));
        }
    }
}