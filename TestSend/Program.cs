using System;
using System.Threading;
using Decos.Diagnostics;

namespace TestSend
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal class Program
    {
        [ThreadStatic] public static ILogFactory LogFactory = null;
        public static Random random = new Random();

        private static void Main(string[] args)
        {
            var logstashAddress = Environment.GetEnvironmentVariable("LOGSTASH_ADDRESS");
            var customerID = new Guid("a6835c7c-6095-4e35-809e-4242af81e0d6");
            string sessionID = "123456789az";
            LogFactory = new LogFactoryBuilder()
                .UseTraceSource()
                .AddConsole()
                .AddLogstash(logstashAddress)
                .AddListenersToTraceListenersCollection()
                .SetMinimumLogLevel(LogLevel.Debug)
                .SetStaticCustomerId(customerID, false)
                .SetStaticSessionId(sessionID)
                .Build();
            var log = LogFactory.Create<Program>();
            
            System.Diagnostics.Trace.WriteLine("Trace log");

            log.Write(LogLevel.Debug, "A");
            log.Write(LogLevel.Debug, "B", new LoggerContext(new Guid("fd760922-c420-4c27-ab7f-c0a640eb6a05")));
            log.Write(LogLevel.Debug, "C", new LoggerContext("mySessionID"));
            log.Write(LogLevel.Debug, "D", new LoggerContext(new Guid("fd760922-c420-4c27-ab7f-c0a640eb6a05"), "mySessionID"));

            log.Write(LogLevel.Debug, new { data1 = "E", data2 = 1 });
            log.Write(LogLevel.Debug, new { data1 = "F", data2 = 1 }, new LoggerContext(new Guid("fd760922-c420-4c27-ab7f-c0a640eb6a05")));
            log.Write(LogLevel.Debug, new { data1 = "G", data2 = 1 }, new LoggerContext("mySessionID"));
            log.Write(LogLevel.Debug, new { data1 = "H", data2 = 1 }, new LoggerContext(new Guid("fd760922-c420-4c27-ab7f-c0a640eb6a05"), "mySessionID"));

            log.Write(LogLevel.Debug, "I", new { data1 = "I", data2 = 2 });
            log.Write(LogLevel.Debug, "J", new { data1 = "J", data2 = 2 }, new LoggerContext(new Guid("fd760922-c420-4c27-ab7f-c0a640eb6a05")));
            log.Write(LogLevel.Debug, "K", new { data1 = "K", data2 = 2 }, new LoggerContext("mySessionID"));
            log.Write(LogLevel.Debug, "L", new { data1 = "L", data2 = 2 }, new LoggerContext(new Guid("fd760922-c420-4c27-ab7f-c0a640eb6a05"), "mySessionID"));


            log.Debug("Debug message.");
            log.Debug(new { data1 = "Debug data", data2 = 2 });

            log.Info("Info message.");
            log.Info(new { data1 = "Info data", data2 = 2 });
            log.Info("InfoMessage", new { data1 = "InfoData", data2 = 2 });

            log.Warn("Warning message.");
            log.Warn(new { data1 = "Warning data", data2 = 3 });

            try
            {
                log.Error("Error message.");
                throw new Exception();
            }
            catch (Exception ex)
            {
                log.Error(ex, new LoggerContext("errSess"));
            }

            log.Critical("Critical message.");
            log.Critical(new { data1 = "Critical data", data2 = 4 });

            /*
            Thread thread1 = new Thread(new ThreadStart(LogInThread));
            thread1.Start();

            Thread thread2 = new Thread(new ThreadStart(LogInThread));
            thread2.Start();
            */

            Console.ReadKey();
        }

        private static void LogInThread()
        {
            var customerId = Guid.NewGuid();
            var sessionId = "randomID" + random.Next(0, 9).ToString() ;

            LogFactory = new LogFactoryBuilder()
                .UseTraceSource()
                .AddConsole()
                .AddLogstash("http://logstashtest.decos.com:9090/")
                .AddListenersToTraceListenersCollection()
                .SetMinimumLogLevel(LogLevel.Debug)
                .SetStaticCustomerId(customerId, true)
                .SetStaticSessionId(sessionId)
                .Build();
            var log = LogFactory.Create<Program>();

            var sample = new SampleLogger();
            sample.LogInThread();
        }
    }
}