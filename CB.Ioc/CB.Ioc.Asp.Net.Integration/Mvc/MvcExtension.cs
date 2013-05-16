using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace CB.Ioc.Mvc
{
    public static class MvcExtension
    {
        public static void RegisterMvcControllers(this IContainerBuilder builder, Assembly assembly)
        {
            foreach (var controllerType in
                assembly.GetTypes()
                        .Where(t => typeof (IController).IsAssignableFrom(t) && t.Name.EndsWith("Controller", StringComparison.Ordinal)))
            {
                builder.Register(controllerType);
                builder.Register(controllerType).As(typeof(IController));
            }
        }
    }
}
