using Decos.Diagnostics;
using Decos.Diagnostics.Trace;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace TestSendCore
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    class Program
    {
        static void Main(string[] args)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                cancellationTokenSource.Cancel();
            };

            Console.WriteLine("Starting.");

            try
            {
                Trace.UseGlobalLock = false;

                var provider = ConfigureServices();
                var factory = provider.GetRequiredService<ILogFactory>();
                var log = provider.GetRequiredService<ILog<Program>>();

                Console.WriteLine("Sending logs...");

                var stopwatch = Stopwatch.StartNew();
                //Parallel.For(0, 1000, i =>
                //{
                //    log.Write((LogLevel)(i % 6), $"Test message {i + 1}");
                //});            
                try
                {
                    log.Warn("This message should not appear in Slack.");
                    throw new NotSupportedException();
                }
                catch (Exception ex)
                {
                    log.Info(ex);
                    log.Critical("An unexpected error occurred while sending test messages.", ex);
                    log.Critical(new { Data = Math.PI, DateTimeOffset = DateTimeOffset.Now.AddDays(1.2), DateTime = DateTime.Now.AddDays(-12.3), Date = DateTime.Today.AddDays(-1) });
                }
                stopwatch.Stop();

                Console.WriteLine($"Done ({stopwatch.Elapsed}).");

                stopwatch.Restart();
                factory.ShutdownAsync(cancellationTokenSource.Token).GetAwaiter().GetResult();
                stopwatch.Stop();

                Console.WriteLine($"Shut down took {stopwatch.Elapsed}.");
            }
            catch (OperationCanceledException) { }
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
                options.SetMinimumLogLevel(LogLevel.Debug);
                options.AddConsole();
                options.AddLogstash("http://log-dev.decos.nl:9090");

                var webhookAddress = Environment.GetEnvironmentVariable("SLACK_WEBHOOK");
                if (!string.IsNullOrEmpty(webhookAddress))
                    options.AddSlack(webhookAddress);
            });

            return services.BuildServiceProvider();
        }
    }
}
