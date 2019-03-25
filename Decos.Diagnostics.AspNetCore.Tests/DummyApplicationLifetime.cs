using System.Threading;

using Microsoft.Extensions.Hosting;

namespace Decos.Diagnostics.AspNetCore.Tests
{
    internal class DummyApplicationLifetime : IApplicationLifetime
    {
        private readonly int timeout;
        private readonly CancellationTokenSource applicationStarted = new CancellationTokenSource();
        private readonly CancellationTokenSource applicationStopping = new CancellationTokenSource();
        private readonly CancellationTokenSource applicationStopped = new CancellationTokenSource();

        public DummyApplicationLifetime(int timeout)
        {
            this.timeout = timeout;
        }

        public CancellationToken ApplicationStarted
            => applicationStarted.Token;

        public CancellationToken ApplicationStopping
            => applicationStopping.Token;

        public CancellationToken ApplicationStopped
            => applicationStopped.Token;

        public void StopApplication()
        {
            applicationStopping.Cancel();
        }
    }
}