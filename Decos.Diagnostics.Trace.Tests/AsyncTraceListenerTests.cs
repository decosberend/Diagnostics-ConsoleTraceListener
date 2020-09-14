using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Decos.Diagnostics.Trace.Tests.Mocks;

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

        [TestMethod]
        public async Task AsyncTraceListenerHandlesErrors()
        {
            const string TestString = "gvNE5nCNNbAhXdaTe8Yq";
            var stdErr = Console.Error;
            using var memoryStream = new MemoryStream();
            using var consoleWriter = new StreamWriter(memoryStream, Encoding.UTF8);
            {
                Console.SetError(consoleWriter);

                var listener = new ThrowingAsyncTraceListener();

                listener.WriteLine(TestString);

                var cancellation = new CancellationTokenSource(Delay);
                try
                {
                    await listener.ProcessQueueAsync(CancellationToken.None, cancellation.Token);
                }
                catch (OperationCanceledException) { }

                Console.Error.Close();
                Console.SetError(stdErr);
            }

            var log = Encoding.UTF8.GetString(memoryStream.ToArray());
            StringAssert.Contains(log, TestString);
        }
    }
}