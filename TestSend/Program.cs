using Decos.Diagnostics;
using Decos.Diagnostics.Trace;
using System;
using System.Threading;

namespace TestSend
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal class Program
    {
        private static void Main(string[] args)
        {
            var logstashAddress = Environment.GetEnvironmentVariable("LOGSTASH_ADDRESS");
            logstashAddress = "http://logstashtest.decos.com:9090/";
            var customerID = Guid.NewGuid();
            var logFactory = new LogFactoryBuilder()
                .UseTraceSource()
                .AddConsole()
                .AddLogstash(logstashAddress)
                .AddListenersToTraceListenersCollection()
                .SetMinimumLogLevel(LogLevel.Debug)
                .SetStaticCustomerId(customerID)
                .Build();
            var log = logFactory.Create<Program>();

            // System.Diagnostics.Trace.WriteLine("I guess the Diagnostics library completely ignores this trace at all");
            
            log.Debug("Debug message.");
            //log.Debug("Debug message.", new Guid("fd760922-c420-4c27-ab7f-c0a640eb6a04"));
            log.Debug(new { data = "Debug data", data2 = 1 });

            log.Info("Info message.");
            log.Info(new { data = "Info data", data2 = 2 });

            log.Warn("Warning message.");
            log.Warn(new { data = "Warning data", data2 = 3 });

            try
            {
                log.Error("Error message.");
                throw new Exception();
            }
            catch (Exception ex)
            {
                log.Error(new { exception = ex, message = "Error message." });
            }

            log.Critical("Critical message.");
            log.Critical(new { data = "Critical data", data2 = 4 });
            Console.ReadKey();
        }
    }
}