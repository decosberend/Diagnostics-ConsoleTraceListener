using Decos.Diagnostics;
using Decos.Diagnostics.Trace;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;

namespace TestSendCore
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting.");

            try
            {
                var provider = ConfigureServices();
                var factory = provider.GetRequiredService<ILogFactory>();
                var log = provider.GetRequiredService<ILog<Program>>();

                Console.WriteLine("Sending logs...");

                var stopwatch = Stopwatch.StartNew();
                for (int i = 0; i < 1000; i++)
                {
                    log.Info($"Test message {i}");
                }
                stopwatch.Stop();

                Console.WriteLine($"Done ({stopwatch.Elapsed}).");

                stopwatch.Restart();
                factory.ShutdownAsync().GetAwaiter().GetResult();
                stopwatch.Stop();

                Console.WriteLine($"Shut down took {stopwatch.Elapsed}.");
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
