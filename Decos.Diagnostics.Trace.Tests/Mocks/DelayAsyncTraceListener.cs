using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Decos.Diagnostics.Trace.Tests.Mocks
{
    public class DelayAsyncTraceListener : AsyncTraceListener
    {
        public DelayAsyncTraceListener(int delay)
        {
            Delay = delay;
            Invocations = new List<object>();
        }

        public int Delay { get; }

        public int QueueCount => RequestQueue.Count;

        public bool ProcessQueueAsyncCalled { get; private set; }

        public bool TraceCalled { get; private set; }

        public List<object> Invocations { get; }

        protected override void TraceInternal(TraceEventData e, string message)
        {
            TraceCalled = true;
            Invocations.Add(message);
            base.TraceInternal(e, message);
        }

        protected override void TraceInternal(TraceEventData e, object data)
        {
            TraceCalled = true;
            Invocations.Add(data);
            base.TraceInternal(e, data);
        }

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