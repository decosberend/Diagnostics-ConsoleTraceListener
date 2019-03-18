using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Decos.Diagnostics.Trace.Tests
{
    [TestClass]
    public class AsyncTraceListenerTests
    {
        private const int Delay = 100;
        private const int EventCount = 10;

        [TestMethod]
        public async Task AsyncTraceListenerEmptiesQueueBeforeShuttingDown()
        {
            var listener = new DelayAsyncTraceListener(Delay);

            for (int i = 0; i < EventCount; i++)
            {
                listener.WriteLine(i);
            }

            var shutdown = new CancellationTokenSource(Delay);
            var cancellation = new CancellationTokenSource(Delay * (EventCount + 1));
            try
            {
                await listener.ProcessQueueAsync(shutdown.Token, cancellation.Token);
            }
            catch (OperationCanceledException) { }

            Assert.AreEqual(0, listener.QueueCount);
        }

        [TestMethod]
        public async Task AsyncTraceListenerDoesNotEmptyQueueWhenAborting()
        {
            var listener = new DelayAsyncTraceListener(Delay);

            for (int i = 0; i < EventCount; i++)
            {
                listener.WriteLine(i);
            }

            var cancellation = new CancellationTokenSource(Delay);
            try
            {
                await listener.ProcessQueueAsync(CancellationToken.None, cancellation.Token);
            }
            catch (OperationCanceledException) { }

            Assert.AreNotEqual(0, listener.QueueCount);
        }
    }
}