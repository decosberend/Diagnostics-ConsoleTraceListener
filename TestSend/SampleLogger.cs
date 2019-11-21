using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Decos.Diagnostics;

namespace TestSend
{
    public class SampleLogger
    {
        private readonly ILog log = Program.LogFactory.Create<SampleLogger>();

        public SampleLogger()
        {
        }

        public void LogInThread()
        {
            var threadId = Environment.CurrentManagedThreadId;
            for (int i = 0; i < 10; i++)
            {
                System.Diagnostics.Trace.WriteLine($"sysway This log belongs to some customer and we are thread id {threadId}");
                log.Info($"logway This log belongs to some customer and we are thread id {threadId}");
                Thread.Sleep(500);
            }
            log.Debug("This log belongs to some customer that is unique", new LogSenderDetails(new Guid("fd760922-c420-4c27-ab7f-c0a640eb6a04")));
        }
    }
}
