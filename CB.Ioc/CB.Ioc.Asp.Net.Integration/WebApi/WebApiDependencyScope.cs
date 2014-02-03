using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Web.Http.Dependencies;

namespace CB.Ioc.WebApi
{
    /// <summary>
    /// Autofac implementation of the <see cref="T:System.Web.Http.Dependencies.IDependencyScope"/> interface.
    /// 
    /// </summary>
    [SecurityCritical]
    public class WebApiDependencyScope : IDependencyScope
    {
        private bool _Disposed;
        private readonly IScopeResolver _LifetimeScope;

        /// <summary>
        /// Gets the lifetime scope for the current dependency scope.
        /// 
        /// </summary>
        public IScopeResolver LifetimeScope
        {
            get
            {
                return _LifetimeScope;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:WebApiDependencyScope"/> class.
        /// 
        /// </summary>
        /// <param name="lifetimeScope">The lifetime scope to resolve services from.</param>
        public WebApiDependencyScope(IScopeResolver lifetimeScope)
        {
            if (lifetimeScope == null)
                throw new ArgumentNullException("lifetimeScope");
            _LifetimeScope = lifetimeScope;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="T:WebApiDependencyScope"/> class.
        /// 
        /// </summary>
        [SecuritySafeCritical]
        ~WebApiDependencyScope()
        {
            Dispose(false);
        }

        /// <summary>
        /// Try to get a service of the given type.
        /// 
        /// </summary>
        /// <param name="serviceType">ControllerType of service to request.</param>
        /// <returns>
        /// An instance of the service, or null if the service is not found.
        /// </returns>
        [SecurityCritical]
        public object GetService(Type serviceType)
        {
            object service;
            _LifetimeScope.TryResolve(serviceType, out service);
            return service;
        }

        /// <summary>
        /// Try to get a list of services of the given type.
        /// 
        /// </summary>
        /// <param name="serviceType">ControllerType of services to request.</param>
        /// <returns>
        /// An enumeration (possibly empty) of the service.
        /// </returns>
        [SecurityCritical]
        public IEnumerable<object> GetServices(Type serviceType)
        {
            if (!_LifetimeScope.CanResolve(serviceType))
            {
                return Enumerable.Empty<object>();
            }
            return _LifetimeScope.ResolveAll(serviceType);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// 
        /// </summary>
        [SecuritySafeCritical]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_Disposed)
                return;
            if (disposing && _LifetimeScope != null) _LifetimeScope.Dispose();
            _Disposed = true;
        }
    }
}
