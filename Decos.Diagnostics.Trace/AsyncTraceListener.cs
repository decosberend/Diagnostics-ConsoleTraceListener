using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Decos.Diagnostics.Trace
{
    /// <summary>
    /// Defines a trace listener that queues up log entries to be processed
    /// asynchronously.
    /// </summary>
    public abstract class AsyncTraceListener : TraceListenerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncTraceListener"/>
        /// class.
        /// </summary>
        public AsyncTraceListener()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncTraceListener"/>
        /// class with the specified name.
        /// </summary>
        /// <param name="name">The name of the trace listener.</param>
        public AsyncTraceListener(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Gets a value indicating whether the trace listener is thread safe.
        /// </summary>
        public override bool IsThreadSafe => true;

        /// <summary>
        /// Gets or sets the delay in milliseconds in between checking whether
        /// there are log entries to send.
        /// </summary>
        public int EmptyQueueDelay { get; set; } = 100;

        /// <summary>
        /// Gets a queue that contains the log entries to be written.
        /// </summary>
        protected ConcurrentQueue<LogEntry> RequestQueue { get; } 
            = new ConcurrentQueue<LogEntry>();

        /// <summary>
        /// Sends log entries that have been queued up.
        /// </summary>
        /// <param name="shutdownToken">
        /// A token to monitor for graceful shutdown requests. When cancellation
        /// is requested, the task will complete after the queue has been
        /// emptied.
        /// </param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests. When cancellation is
        /// requested, the current operation will be aborted and the queue will
        /// not be finished.
        /// </param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public virtual async Task ProcessQueueAsync(CancellationToken shutdownToken, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                while (RequestQueue.TryDequeue(out var logEntry))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await TraceAsync(logEntry, cancellationToken);
                }

                if (RequestQueue.Count == 0 && shutdownToken.IsCancellationRequested)
                    break;

                try
                {
                    await Task.Delay(EmptyQueueDelay, shutdownToken);
                }
                catch (OperationCanceledException) { }
            }
        }

        /// <summary>
        /// When overridden in a derived class, processes a log entry to be
        /// written.
        /// </summary>
        /// <param name="logEntry">The log entry to be written.</param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests.
        /// </param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public abstract Task TraceAsync(LogEntry logEntry, CancellationToken cancellationToken);

        /// <summary>
        /// Enqueues trace information and a message to the queue for
        /// asynchronous processing.
        /// </summary>
        /// <param name="e">
        /// A <see cref="TraceListenerBase.TraceEventData"/> object that contains
        /// information about the trace event.
        /// </param>
        /// <param name="message">The message to write.</param>
        protected override void TraceInternal(TraceEventData e, string message)
        {
            var logEntry = new LogEntry
            {
                Level = e.Type.ToLogLevel(),
                Source = e.Source,
                Message = message,
                EventId = e.ID,
                ProcessId = e.Cache.ProcessId,
                ThreadId = e.Cache.ThreadId,
                CustomerId = e.CustomerID
            };

            RequestQueue.Enqueue(logEntry);
        }

        /// <summary>
        /// Enqueues trace information and data to the queue for
        /// asynchronous processing.
        /// </summary>
        /// <param name="e">
        /// A <see cref="TraceListenerBase.TraceEventData"/> object that contains
        /// information about the trace event.
        /// </param>
        /// <param name="data">The trace data to write.</param>
        protected override void TraceInternal(TraceEventData e, object data)
        {
            if (data != null && data is LogData logData)
            {
                TraceInternal(e, logData);
                return;
            }

            var logEntry = new LogEntry
            {
                Level = e.Type.ToLogLevel(),
                Source = e.Source,
                EventId = e.ID,
                ProcessId = e.Cache.ProcessId,
                ThreadId = e.Cache.ThreadId
            };
            if (data is CustomerLogData)
            {
                data = CustomerLogData.TryExtractCustomerId(data, out Guid? customerId);

                logEntry.CustomerId = e.CustomerID;
                logEntry.Message = data.ToString();
            }
            else
            {
                logEntry.Data = data;
            }

            RequestQueue.Enqueue(logEntry);
        }

        /// <summary>
        /// Enqueues trace information and exception data to the queue for
        /// asynchronous processing.
        /// </summary>
        /// <param name="e">
        /// A <see cref="TraceListenerBase.TraceEventData"/> object that contains
        /// information about the trace event.
        /// </param>
        /// <param name="data">The log data to write.</param>
        protected void TraceInternal(TraceEventData e, LogData data)
        {
            var logEntry = new LogEntry
            {
                Level = e.Type.ToLogLevel(),
                Source = e.Source,
                Message = data.Message,
                Data = data.Data,
                EventId = e.ID,
                ProcessId = e.Cache.ProcessId,
                ThreadId = e.Cache.ThreadId
            };

            RequestQueue.Enqueue(logEntry);
        }
    }
}