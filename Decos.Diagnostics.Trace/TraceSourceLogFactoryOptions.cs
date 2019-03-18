using System.Collections.Generic;
using System.Diagnostics;

namespace Decos.Diagnostics.Trace
{
    /// <summary>
    /// Represents options for specifying the behavior of the <see
    /// cref="TraceSourceLogFactory"/> class and the instances it creates.
    /// </summary>
    public class TraceSourceLogFactoryOptions : LogFactoryOptions
    {
        /// <summary>
        /// Gets a collection of trace listeners to be added.
        /// </summary>
        public ICollection<TraceListener> Listeners { get; }
            = new List<TraceListener>();
    }
}