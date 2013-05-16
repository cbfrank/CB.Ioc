using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            return Container.Resolve(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return Container.ResolveAll(serviceType);
        }
    }
}
