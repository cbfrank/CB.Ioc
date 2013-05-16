using System;
using System.Collections;
using System.Collections.Generic;

namespace CB.Ioc
{
    public interface IContainer : IDisposable
    {
        object Resolve(Type resolveType, string name, params IResolveParameter[] parameters);
        object Resolve(Type resolveType, params IResolveParameter[] parameters);
        IEnumerable<object> ResolveAll(Type resolveType, params IResolveParameter[] parameters);
        bool CanResolve(Type resolveType, string name = null);
        void BuildUp(object resolvedInstance);
    }
}
