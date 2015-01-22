using System;
using System.Collections.Generic;

namespace CB.Ioc
{
    public interface IScopeResolver : IDisposable
    {
        object Resolve(Type resolveType, string name, params IResolveParameter[] parameters);

        object Resolve(Type resolveType, params IResolveParameter[] parameters);

        IEnumerable<object> ResolveAll(Type resolveType, params IResolveParameter[] parameters);

        bool CanResolve(Type resolveType, string name = null);

        IScopeResolver BeginLifetimeScope();

        IScopeResolver BeginLifetimeScope(object tag);

        IScopeResolver BeginLifetimeScope(Action<IContainerBuilder> configurationAction);
        
        void BuildUp(object resolvedInstance);
    }
}