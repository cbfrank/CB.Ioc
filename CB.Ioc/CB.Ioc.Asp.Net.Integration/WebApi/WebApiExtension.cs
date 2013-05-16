using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Http.Controllers;

namespace CB.Ioc.WebApi
{
    public static class WebApiExtension
    {
        public static void RegisterWebApiControllers(this IContainerBuilder builder, Assembly assembly)
        {
            foreach (var controllerType in
                assembly.GetTypes().Where(t => typeof(IHttpController).IsAssignableFrom(t) && t.Name.EndsWith("Controller", StringComparison.Ordinal)))
            {
                builder.Register(controllerType);
                builder.Register(controllerType).As(typeof(IHttpController));
            }
        }
    }
}
