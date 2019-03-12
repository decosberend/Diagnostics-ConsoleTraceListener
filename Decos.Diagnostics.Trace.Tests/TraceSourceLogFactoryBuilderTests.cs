using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Decos.Diagnostics.Trace.Tests
{
    [TestClass]
    public class TraceSourceLogFactoryBuilderTests
    {
        [TestMethod]
        public void BuilderCreatesFactoryWithDefaultOptions()
        {
            var factory = new LogFactoryBuilder()
                .UseTraceSource()
                .Build();

            var log = factory.Create("Test");
            Assert.IsTrue(log.IsEnabled(LogLevel.Information));
        }

        [TestMethod]
        public void BuilderCreatesFactoryWithCustomLogLevel()
        {
            var factory = new LogFactoryBuilder()
                .UseTraceSource()
                .ConfigureOptions(options => options.DefaultMinimumLogLevel = LogLevel.Debug)
                .Build();

            var log = factory.Create("Test");
            Assert.IsTrue(log.IsEnabled(LogLevel.Debug));
        }

        [TestMethod]
        public void BuilderCreatesFactoryWithCustomFilters()
        {
            var factory = new LogFactoryBuilder()
                .UseTraceSource()
                .ConfigureOptions(options =>
                {
                    options.DefaultMinimumLogLevel = LogLevel.Information;
                    options.Filters.Add("Decos.Diagnostics", LogLevel.Debug);
                })
                .Build();

            Assert.IsTrue(factory.Create("Decos.Diagnostics").IsEnabled(LogLevel.Debug));
            Assert.IsFalse(factory.Create("Decos").IsEnabled(LogLevel.Debug));
        }
    }
}
