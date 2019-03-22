using System;
using System.Threading;
using System.Threading.Tasks;

using Slack.Webhooks;

namespace Decos.Diagnostics.Trace.Slack
{
    /// <summary>
    /// Represents an asynchronous trace listener that sends messages to Slack.
    /// </summary>
    public class SlackTraceListener : AsyncTraceListener
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SlackTraceListener"/>
        /// class using the specified webhook address.
        /// </summary>
        /// <param name="webhookAddress">
        /// The address of the Slack Incoming Webhook to post messages to.
        /// </param>
        public SlackTraceListener(Uri webhookAddress)
        {
            Client = new SlackClient(webhookAddress.ToString());
        }

        /// <summary>
        /// Gets a reference to the Slack client used to send messages to an
        /// Incoming Webhook.
        /// </summary>
        protected ISlackClient Client { get; }

        /// <summary>
        /// Sends the log entry to Slack.
        /// </summary>
        /// <param name="logEntry">The log entry to be written.</param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests.
        /// </param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public override Task TraceAsync(LogEntry logEntry, CancellationToken cancellationToken)
            => Client.PostAsync(logEntry.ToSlackMessage());
    }
}