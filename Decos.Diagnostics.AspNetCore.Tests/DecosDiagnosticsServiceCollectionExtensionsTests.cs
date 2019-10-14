using System;
using System.Collections.Generic;
using Decos.Diagnostics.AspNetCore.MicrosoftExtensionsLogging;
using Decos.Diagnostics.Trace.Tests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Decos.Diagnostics.AspNetCore.Tests
{
    [TestClass]
    public class DecosDiagnosticsServiceCollectionExtensionsTests
    {
        [TestMethod]
        public void LogFactoryCanBeResolved()
        {
            var services = new ServiceCollection();

            services.AddTraceSourceLogging();

            var provider = services.BuildServiceProvider();
            Assert.IsNotNull(provider.GetRequiredService<ILogFactory>());
        }

        [TestMethod]
        public void LogFactoryCanBeResolvedWithCustomOptions()
        {
            var services = new ServiceCollection();

            services.AddTraceSourceLogging(options =>
            {
                options.SetMinimumLogLevel(LogLevel.Debug);
            });

            var provider = services.BuildServiceProvider();
            var factory = provider.GetRequiredService<ILogFactory>();
            Assert.IsTrue(factory.Create("Test").IsEnabled(LogLevel.Debug));
        }

        [TestMethod]
        public void GenericLogCanBeResolved()
        {
            var services = new ServiceCollection();

            services.AddTraceSourceLogging();

            var provider = services.BuildServiceProvider();
            Assert.IsNotNull(provider.GetRequiredService<ILog<DecosDiagnosticsServiceCollectionExtensionsTests>>());
        }

        [TestMethod]
        public void MicrosoftExtensionsLoggerCanBeResolved()
        {
            var services = new ServiceCollection();

            services.AddTraceSourceLogging();

            var provider = services.BuildServiceProvider();
            var logger = provider.GetRequiredService<ILogger<DecosDiagnosticsServiceCollectionExtensionsTests>>();
            Assert.IsInstanceOfType(logger, typeof(LoggerWrapper));
        }

        [TestMethod]
        public void MicrosoftExtensionsLoggerLogMessagesToTrace()
        {
            var listener = new DelayAsyncTraceListener(0);
            var services = new ServiceCollection();
            services.AddTraceSourceLogging(options =>
            {
                options.AddTraceListener(listener);
            });

            var provider = services.BuildServiceProvider();
            var logger = provider.GetRequiredService<ILogger<DecosDiagnosticsServiceCollectionExtensionsTests>>();
            var test = 1;
            logger.LogInformation("Test {Test}", test);

            Assert.IsTrue(listener.TraceCalled);
        }

        [TestMethod]
        public void MicrosoftExtensionsLoggerLogExceptionsToTrace()
        {
            var listener = new DelayAsyncTraceListener(0);
            var services = new ServiceCollection();
            services.AddTraceSourceLogging(options =>
            {
                options.AddTraceListener(listener);
            });

            var provider = services.BuildServiceProvider();
            var logger = provider.GetRequiredService<ILogger<DecosDiagnosticsServiceCollectionExtensionsTests>>();
            try
            {
                throw new Exception("Test exception");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "test");
            }

            Assert.IsTrue(listener.TraceCalled);
        }

        [TestMethod]
        public void MicrosoftExtensionsLoggerFactoryCreatesWrapperLoggers()
        {
            var services = new ServiceCollection();

            services.AddTraceSourceLogging();

            var provider = services.BuildServiceProvider();
            var factory = provider.GetRequiredService<ILoggerFactory>();
            Assert.IsInstanceOfType(factory.CreateLogger("Test"), typeof(LoggerWrapper));
        }

        [TestMethod]
        public void GenericLogCanBeResolvedWithCustomOptions()
        {
            var services = new ServiceCollection();

            services.AddTraceSourceLogging(options =>
            {
                options.AddFilter(GetType().Namespace, LogLevel.None);
            });

            var provider = services.BuildServiceProvider();
            var log = provider.GetRequiredService<ILog<DecosDiagnosticsServiceCollectionExtensionsTests>>();
            Assert.IsFalse(log.IsEnabled(LogLevel.Information));
        }

        [TestMethod]
        public void ApplicationPerformsGracefulShutdownAfterInjection()
        {
            var listener = new DelayAsyncTraceListener(500);
            var services = new ServiceCollection();
            services.AddSingleton<IApplicationLifetime>(new DummyApplicationLifetime(10));
            services.AddTraceSourceLogging(options =>
            {
                options.AddTraceListener(listener);
            });
            var provider = services.BuildServiceProvider();
            var log = provider.GetRequiredService<ILog<DecosDiagnosticsServiceCollectionExtensionsTests>>();
            for (int i = 0; i < 10; i++)
                log.Info("Test");

            Assert.AreNotEqual(0, listener.QueueCount);
            var handler = provider.GetRequiredService<ApplicationShutdownHandler>();
            var lifeTime = provider.GetRequiredService<IApplicationLifetime>();
            lifeTime.StopApplication();
            Assert.AreEqual(0, listener.QueueCount);
        }
    }
}