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

        private static TraceSourceLogFactory CreateFactory(LogLevel minLogLevel, params (SourceName, LogLevel)[] levels)
        {
            var options = new TraceSourceLogFactoryOptions();
            options.DefaultMinimumLogLevel = minLogLevel;

            foreach (var item in levels)
                options.Filters.Add(item.Item1, item.Item2);

            return new TraceSourceLogFactory(options);
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
    }
}
