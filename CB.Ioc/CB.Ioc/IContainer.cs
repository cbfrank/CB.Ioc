using System;
using System.Collections;

namespace CB.Ioc
{
    public interface IContainer : IDisposable
    {
        object Resolve(Type resolveType, string name = null);
        IEnumerable ResolveAll(Type resolveType);
        bool CanResolve(Type resolveType, string name = null);
        void BuildUp(object resolvedInstance);
    }
}
