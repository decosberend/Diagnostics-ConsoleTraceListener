using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Decos.Diagnostics.Trace.Tests.Mocks
{
    internal class ThrowingAsyncTraceListener : AsyncTraceListener
    {
        public override Task TraceAsync(LogEntry logEntry, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
