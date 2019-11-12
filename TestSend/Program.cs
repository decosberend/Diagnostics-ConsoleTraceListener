using System;
using System.Threading;
using Decos.Diagnostics;

namespace TestSend
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal class Program
    {
        [ThreadStatic] public static Guid guidForThread = new Guid("12345678-abcd-1234-abcd-123456789876");
        static Guid guidForProcess = new Guid("12345678-abcd-1234-abcd-123456789876");
        [ThreadStatic] public static ILogFactory LogFactory = null;

        private static void Main(string[] args)
        {
            var logstashAddress = Environment.GetEnvironmentVariable("LOGSTASH_ADDRESS");
            logstashAddress = "http://logstashtest.decos.com:9090/";
            var customerID = new Guid("a6835c7c-6095-4e35-809e-4242af81e0d6");
            LogFactory = new LogFactoryBuilder()
                .UseTraceSource()
                .AddConsole()
                .AddLogstash(logstashAddress)
                .AddListenersToTraceListenersCollection()
                .SetMinimumLogLevel(LogLevel.Debug)
                .SetStaticCustomerId(customerID, false)
                .Build();
            //logFactory.SetCustomerId(customerID);
            var log = LogFactory.Create<Program>();
            
            System.Diagnostics.Trace.WriteLine("ThisIsALogWrittenByMe");
            
            log.Debug("Debug message.");
            log.Debug("Debug message.", new Guid("fd760922-c420-4c27-ab7f-c0a640eb6a04"));
            log.Debug(new { data = "Debug data", data2 = 1 });
            log.Debug(new { data = "Debug data", data2 = 1 }, new Guid("fd760922-c420-4c27-ab7f-c0a640eb6a04"));

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
            Thread thread1 = new Thread(new ThreadStart(LogInThread));
            thread1.Start();

            Thread thread2 = new Thread(new ThreadStart(LogInThread));
            thread2.Start();

            Console.ReadKey();
        }

        private static void LogInThread()
        {
            var customerId = Guid.NewGuid();
            guidForThread = customerId;

            LogFactory = new LogFactoryBuilder()
                .UseTraceSource()
                .AddConsole()
                .AddLogstash("http://logstashtest.decos.com:9090/")
                .AddListenersToTraceListenersCollection()
                .SetMinimumLogLevel(LogLevel.Debug)
                .SetStaticCustomerId(customerId, true)
                .Build();
            var log = LogFactory.Create<Program>();

            var sample = new SampleLogger();
            sample.LogInThread();
        }
    }
}