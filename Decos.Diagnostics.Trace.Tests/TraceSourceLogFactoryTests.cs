using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Decos.Diagnostics.Trace.Tests
{
    [TestClass]
    public class TraceSourceLogFactoryTests
    {
        private TraceListenerCollection defaultListeners;

        [TestInitialize]
        public void OnTestStarting()
        {
            defaultListeners = System.Diagnostics.Trace.Listeners;
        }

        [TestCleanup]
        public void OnTestEnded()
        {
            System.Diagnostics.Trace.Listeners.Clear();
            System.Diagnostics.Trace.Listeners.AddRange(defaultListeners);
        }

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