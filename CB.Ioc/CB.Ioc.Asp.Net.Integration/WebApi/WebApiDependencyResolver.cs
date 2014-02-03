using System;
using System.Collections.Generic;
using System.Security;
using System.Web.Http.Dependencies;

namespace CB.Ioc.WebApi
{
    [SecurityCritical]
    public class WebApiDependencyResolver : IDependencyResolver
    {
        /// <summary>
        /// Tag used to identify registrations that are scoped to the API request level.
        /// 
        /// </summary>
        public const string API_REQUEST_TAG = "CB.WebApi.WebRequest";
        private bool _Disposed;
        private readonly IContainer _Container;
        private readonly IDependencyScope _RootDependencyScope;

        /// <summary>
        /// Gets the root container provided to the dependency resolver.
        /// 
        /// </summary>
        public IContainer Container
        {
            get
            {
                return _Container;
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="T:Autofac.Integration.WebApi.AutofacWebApiDependencyResolver"/> class.
        /// 
        /// </summary>
        /// <param name="container">The container that nested lifetime scopes will be create from.</param>
        public WebApiDependencyResolver(IContainer container)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            _Container = container;
            _RootDependencyScope = new WebApiDependencyScope(container);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="T:Autofac.Integration.WebApi.AutofacWebApiDependencyResolver"/> class.
        /// 
        /// </summary>
        [SecuritySafeCritical]
        ~WebApiDependencyResolver()
        {
            Dispose(false);
        }

        /// <summary>
        /// Try to get a service of the given type.
        /// 
        /// </summary>
        /// <param name="serviceType">Type of service to request.</param>
        /// <returns>
        /// An instance of the service, or null if the service is not found.
        /// </returns>
        [SecurityCritical]
        public object GetService(Type serviceType)
        {
            return _RootDependencyScope.GetService(serviceType);
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
            return _RootDependencyScope.GetServices(serviceType);
        }

        /// <summary>
        /// Starts a resolution scope. Objects which are resolved in the given scope will belong to
        ///             that scope, and when the scope is disposed, those objects are returned to the container.
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// The dependency scope.
        /// 
        /// </returns>
        [SecurityCritical]
        public IDependencyScope BeginScope()
        {
            return new WebApiDependencyScope(_Container.BeginLifetimeScope(API_REQUEST_TAG));
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
            if (disposing && _RootDependencyScope != null)
                _RootDependencyScope.Dispose();
            _Disposed = true;
        }
    }
}
