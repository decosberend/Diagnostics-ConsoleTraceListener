using System;
using System.Diagnostics;

namespace Decos.Diagnostics.Trace.Tests.Mocks
{
    /// <summary>
    /// Represents a trace listener that does nothing.
    /// </summary>
    public class NullTraceListener : TraceListener
    {
        public NullTraceListener()
        {
        }

        public NullTraceListener(string name)
            : base(name)
        {
        }

        public override void Write(string message)
        {
        }

        public override void WriteLine(string message)
        {
        }
    }
}