Decos.Diagnostics
=================

[![Nuget (Decos.Diagnostics)](https://img.shields.io/nuget/vpre/Decos.Diagnostics.svg?label=Decos.Diagnostics)](https://www.nuget.org/packages/Decos.Diagnostics/)
[![Nuget (Decos.Diagnostics.AspNetCore)](https://img.shields.io/nuget/vpre/Decos.Diagnostics.AspNetCore.svg?label=Decos.Diagnostics.AspNetCore)](https://www.nuget.org/packages/Decos.Diagnostics.AspNetCore/)
[![Nuget (Decos.Diagnostics.Trace)](https://img.shields.io/nuget/vpre/Decos.Diagnostics.Trace.svg?label=Decos.Diagnostics.Trace)](https://www.nuget.org/packages/Decos.Diagnostics.Trace/)
[![Nuget (Decos.Diagnostics.Trace.Slack)](https://img.shields.io/nuget/vpre/Decos.Diagnostics.Trace.Slack.svg?label=Decos.Diagnostics.Trace.Slack)](https://www.nuget.org/packages/Decos.Diagnostics.Trace.Slack/)

This repository contains the source code for the Decos.Diagnostics packages. There are currently four available packages:

- **Decos.Diagnostics** contains the basic abstractions and extensions for logging.
- **Decos.Diagnostics.Trace** contains a System.Diagnostics.TraceSource implementation and some trace listeners that write to the console or to a Logstash input.
- **Decos.Diagnostics.AspNetCore** provides both of the above packages in addition to some extensions for ASP.NET Core compatibility, such as dependency injection configuration extensions and a shutdown handler to allow for flushing logs that haven't been sent yet.
- **Decos.Diagnostics.Trace.Slack** provides a trace listener that sends logs to Slack using a web hook.

Usage
-----

Use the **LogFactoryBuilder** class to configure and obtain a **LogFactory** or use the **IServiceCollection.AddTraceSourceLogging** extension method to configure the **ILog**, **ILog&lt;T&gt;** and **ILogFactory** interfaces for dependency injection.

When using any async trace listener, such as the Logstash or Slack trace listeners, you need to give them time to gracefully shutdown in order to prevent the potential loss of logged information. This can be done with the **ShutdownAsync** method on the **LogFactory** instance that was constructed at the start. Alternatively, when using the **IServiceCollection.AddTraceSourceLogging** extension method, an application shutdown handler is installed to take care of this when ASP.NET Core is shutting down.
