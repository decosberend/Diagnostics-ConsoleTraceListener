using System;
using System.Threading;
using System.Threading.Tasks;

namespace Decos.Diagnostics.Trace.Tests
{
    internal class DelayAsyncTraceListener : AsyncTraceListener
    {
        public DelayAsyncTraceListener(int delay)
        {
            Delay = delay;
        }

        public int Delay { get; }

        public int QueueCount => RequestQueue.Count;

        public bool ProcessQueueAsyncCalled { get; private set; }

        public override Task TraceAsync(LogEntry logEntry, CancellationToken cancellationToken)
        {
            return Task.Delay(Delay, cancellationToken);
        }

        public override Task ProcessQueueAsync(CancellationToken shutdownToken, CancellationToken cancellationToken)
        {
            ProcessQueueAsyncCalled = true;
            return base.ProcessQueueAsync(shutdownToken, cancellationToken);
        }
    }
}