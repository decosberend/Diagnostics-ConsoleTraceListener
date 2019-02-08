using System;
using System.Threading;

namespace TestSend
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                cancellationTokenSource.Cancel();
            };

            Decos.Shared.Common.EnableTraceAutoReload();
            if (Decos.Shared.Tracer.Exception != null)
                throw Decos.Shared.Tracer.Exception;

            LogLoop(cancellationTokenSource.Token);
        }

        private static void LogLoop(CancellationToken cancellationToken)
        {
            var trace = Decos.Shared.Tracer.Create("TestSend");

            var i = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                using (trace.Time("Sending test message"))
                {
                    trace.Info("Test message #{0}", ++i);
                }

                cancellationToken.WaitHandle.WaitOne(5000);
            }

            trace.Info("User cancelled.");
        }
    }
}