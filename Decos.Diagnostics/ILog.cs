using System;
using System.Collections.Generic;
using System.Text;

namespace Decos.Diagnostics
{
    public interface ILog
    {
        bool IsEnabled(LogLevel logLevel);

        void Write(LogLevel logLevel, string message);

        void Write<T>(LogLevel logLevel, T data);
    }
}
