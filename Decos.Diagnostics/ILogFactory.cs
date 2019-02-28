using System;
using System.Collections.Generic;
using System.Text;

namespace Decos.Diagnostics
{
    public interface ILogFactory
    {
        ILog Create<T>();

        ILog Create(string name);
    }
}
