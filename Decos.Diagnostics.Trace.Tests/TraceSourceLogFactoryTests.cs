using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Decos.Diagnostics.Trace.Tests
{
    [TestClass]
    public class TraceSourceLogFactoryTests
    {
        [TestMethod]
        public void CreatedTraceSourceUsesNamespaceFromType()
        {
            var factory = CreateFactory(LogLevel.None,
                ("Decos.Diagnostics.Trace.Tests", LogLevel.Information));

            var log = factory.Create<TraceSourceLogFactoryTests>();

            Assert.IsTrue(log.IsEnabled(LogLevel.Information));
        }

        [TestMethod]
        public void CreatedTraceSourceLogUsesConfiguredLogLevel()
        {
            var factory = CreateFactory(LogLevel.Information,
                ("Decos.Diagnostics", LogLevel.Error));

            var log = factory.Create("Decos.Diagnostics");

            Assert.IsTrue(log.IsEnabled(LogLevel.Error));
            Assert.IsFalse(log.IsEnabled(LogLevel.Warning));
        }

        [TestMethod]
        public void CreatedTraceSourceLogUsesInheritedLogLevel()
        {
            var factory = CreateFactory(LogLevel.Information,
                ("Decos", LogLevel.Warning));

            var log = factory.Create("Decos.Diagnostics");

            Assert.IsTrue(log.IsEnabled(LogLevel.Warning));
            Assert.IsFalse(log.IsEnabled(LogLevel.Information));
        }

        [TestMethod]
        public void CreatedTraceSourceLogUsesBestMatchingFilter()
        {
            // Test to ensure dictionary is sorted properly

            var factory = CreateFactory(LogLevel.None,
                ("Decos", LogLevel.Information),
                ("Decos.Diagnostics.Trace.Tests.Something", LogLevel.None),
                ("Decos.Diagnostics", LogLevel.Warning),
                ("Decos.Diagnostics.Trace.Tests", LogLevel.Debug),
                ("Decos.Diagnostics.Trace", LogLevel.Critical));

            var log = factory.Create("Decos.Diagnostics.Trace.Tests");

            Assert.IsTrue(log.IsEnabled(LogLevel.Debug));
        }

        [TestMethod]
        public void CreatedTraceSourceCanBeTurnedOffIndividually()
        {
            var factory = CreateFactory(LogLevel.Information,
                ("Decos", LogLevel.None));

            var log = factory.Create("Decos.Diagnostics");

            Assert.IsFalse(log.IsEnabled(LogLevel.Critical));
            Assert.IsFalse(log.IsEnabled(LogLevel.Error));
            Assert.IsFalse(log.IsEnabled(LogLevel.Warning));
            Assert.IsFalse(log.IsEnabled(LogLevel.Information));
            Assert.IsFalse(log.IsEnabled(LogLevel.Debug));
        }

        [TestMethod]
        public void CreatedTraceSourceLogUsesDefaultLogLevelIfNoneMatch()
        {
            var factory = CreateFactory(LogLevel.Information,
                ("Decos.Diagnostics.Trace", LogLevel.Debug));

            var log = factory.Create("Decos.Diagnostics");

            Assert.IsTrue(log.IsEnabled(LogLevel.Information));
            Assert.IsFalse(log.IsEnabled(LogLevel.Debug));
        }

        [TestMethod]
        public void CreatedTraceSourceLogSupportsMultipleTraceListenersWithNoName()
        {
            var listener1 = new NullTraceListener();
            var listener2 = new TextWriterTraceListener(Console.Out);
            System.Diagnostics.Trace.Listeners.Add(listener1);
            System.Diagnostics.Trace.Listeners.Add(listener2);
            var factory = CreateFactory(LogLevel.Information);

            var log = (TraceSourceLog)factory.Create("Decos.Diagnostics");

            CollectionAssert.Contains(log.TraceSource.Listeners, listener1);
            CollectionAssert.Contains(log.TraceSource.Listeners, listener2);
        }

        [TestMethod]
        public void CreatedTraceSourceLogDoesNotContainTwoDefaultListeners()
        {
            var factory = CreateFactory(LogLevel.Information);

            var log = (TraceSourceLog)factory.Create("Decos.Diagnostics");

            if (log.TraceSource.Listeners.OfType<DefaultTraceListener>().Count() > 1)
                Assert.Fail();
        }

        [TestMethod]
        public async Task FactoryAllowsForGracefulShutdown()
        {
            var listener = new DelayAsyncTraceListener(100);
            var factory = new LogFactoryBuilder()
                .UseTraceSource()
                .AddTraceListener(listener)
                .Build();

            var log = factory.Create("Test");
            for (int i = 0; i < 10; i++)
                log.Info(i);

            await factory.ShutdownAsync();

            Assert.AreEqual(0, listener.QueueCount);
        }

        [TestMethod]
        public async Task FactoryShutdownCanBeCancelled()
        {
            var listener = new DelayAsyncTraceListener(100);
            var factory = new LogFactoryBuilder()
                .UseTraceSource()
                .AddTraceListener(listener)
                .Build();

            var log = factory.Create("Test");
            for (int i = 0; i < 10; i++)
                log.Info(i);

            var cancellationTokenSource = new CancellationTokenSource(250);
            try
            {
                await factory.ShutdownAsync(cancellationTokenSource.Token);
            }
            catch (OperationCanceledException) { }

            Assert.AreNotEqual(0, listener.QueueCount);
        }

        private static TraceSourceLogFactory CreateFactory(LogLevel minLogLevel, params (SourceName, LogLevel)[] levels)
        {
            var options = new TraceSourceLogFactoryOptions();
            options.DefaultMinimumLogLevel = minLogLevel;

            foreach (var item in levels)
                options.Filters.Add(item.Item1, item.Item2);

            return new TraceSourceLogFactory(options);
        }
    }
}