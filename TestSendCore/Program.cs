using Decos.Diagnostics;
using Decos.Diagnostics.Trace;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;

namespace TestSendCore
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting.");

            try
            {
                var provider = ConfigureServices();
                var log = provider.GetRequiredService<ILog<Program>>();

                Console.WriteLine("Sending logs...");

                var stopwatch = Stopwatch.StartNew();
                for (int i = 0; i < 100; i++)
                {
                    log.Info($"Test message {i}");
                }
                stopwatch.Stop();

                Console.WriteLine($"Done ({stopwatch.Elapsed}).");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unhandled exception!");
                Console.WriteLine(ex);
            }
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddTraceSourceLogging(options =>
            {
                options.AddLogstash("http://log-dev.decos.nl:9090");
            });

            return services.BuildServiceProvider();
        }
    }
}
