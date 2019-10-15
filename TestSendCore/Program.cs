using Decos.Diagnostics;
using Decos.Diagnostics.Trace;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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

                log.Info("String array", new string[] { "abc", "αβγ" });
                log.Info("Object array", new object[] { 1, DateTimeOffset.Now });
                log.Info("Dictionary", new Dictionary<string, int>()
                {
                    ["a"] = 1, ["b"] = 2, ["c"] = 3
                });

                log.Info("Something something", new { data = 1, date = DateTimeOffset.Now });
                try
                {
                    log.Warn("This message should not appear in Slack.");
                    try
                    {
                        System.IO.File.Open(Guid.NewGuid().ToString(), System.IO.FileMode.Open);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException("Outer exception message.", ex);
                    }
                }
                catch (Exception ex)
                {
                    log.Critical("An unexpected error occurred while sending test messages.", ex);
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

                var logstashAddress = Environment.GetEnvironmentVariable("LOGSTASH_ADDRESS");
                if (!string.IsNullOrEmpty(logstashAddress))
                    options.AddLogstash(logstashAddress);

                var webhookAddress = Environment.GetEnvironmentVariable("SLACK_WEBHOOK");
                if (!string.IsNullOrEmpty(webhookAddress))
                    options.AddSlack(webhookAddress);
            });

            return services.BuildServiceProvider();
        }
    }
}
