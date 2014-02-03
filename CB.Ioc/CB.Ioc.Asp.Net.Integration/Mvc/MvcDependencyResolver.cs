using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CB.Ioc.Mvc
{
    public class MvcDependencyResolver : IDependencyResolver
    {
        public MvcDependencyResolver(IContainer container)
        {
            Container = container;
        }

        public IContainer Container { get; private set; }

        public object GetService(Type serviceType)
        {
            object service;
            Container.TryResolve(serviceType, out service);
            return service;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            if (Container.CanResolve(serviceType))
            {
                return Container.ResolveAll(serviceType);
            }
            return new object[0];
        }
    }
}
