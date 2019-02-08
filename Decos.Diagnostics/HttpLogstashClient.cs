using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jil;

namespace Decos.Diagnostics
{
    public class HttpLogstashClient
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public HttpLogstashClient(Uri logstashEndpoint)
        {
            Endpoint = logstashEndpoint;
        }

        public Uri Endpoint { get; }

        public Task LogAsync(LogEntry logEntry)
            => LogAsync(logEntry, CancellationToken.None);

        public async Task LogAsync(LogEntry logEntry, CancellationToken cancellationToken)
        {
            var json = JSON.Serialize(logEntry, Options.ISO8601CamelCase);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await httpClient.PostAsync(Endpoint, content, cancellationToken).ConfigureAwait(false);
        }
    }
}
