using System;
using System.Collections.Generic;
using System.Text;

namespace Decos.Diagnostics
{
    public static class LogExtensions
    {
        public static void Debug(this ILog log, string message)
            => log.Write(LogLevel.Debug, message);

        public static void Debug<T>(this ILog log, T data)
            => log.Write(LogLevel.Debug, data);

        public static void Info(this ILog log, string message)
            => log.Write(LogLevel.Information, message);

        public static void Info<T>(this ILog log, T data)
            => log.Write(LogLevel.Information, data);

        public static void Warn(this ILog log, string message)
            => log.Write(LogLevel.Warning, message);

        public static void Warn<T>(this ILog log, T data)
            => log.Write(LogLevel.Warning, data);

        public static void Error(this ILog log, string message)
            => log.Write(LogLevel.Error, message);

        public static void Error<T>(this ILog log, T data)
            => log.Write(LogLevel.Error, data);

        public static void Critical(this ILog log, string message)
            => log.Write(LogLevel.Critical, message);

        public static void Critical<T>(this ILog log, T data)
            => log.Write(LogLevel.Critical, data);
    }
}
