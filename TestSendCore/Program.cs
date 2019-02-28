using Decos.Diagnostics;
using Decos.Diagnostics.Trace;
using System;

namespace TestSendCore
{
    class Program
    {
        static void Main(string[] args)
        {
            var logFactory = new TraceSourceLogFactory();
            var log = logFactory.Create<Program>();

            log.Debug("Debug message.");
            log.Debug(new { data = "Debug data", data2 = 1 });

            log.Info("Info message.");
            log.Info(new { data = "Info data", data2 = 2 });

            log.Warn("Warning message.");
            log.Warn(new { data = "Warning data", data2 = 3 });

            try
            {
                log.Error("Error message.");
                throw new Exception();
            }
            catch(Exception ex)
            {
                log.Error(new { exception = ex, message = "Error message." });
            }

            log.Critical("Critical message.");
            log.Critical(new { data = "Critical data", data2 = 4 });
        }
    }
}
