using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Autofac;

namespace CB.Ioc.Adapter.Autofac
{
    public class AutofacContainer : IContainer
    {
        public global::Autofac.IContainer ComponentContext { get; internal set; }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (ComponentContext != null)
                {
                    ComponentContext.Dispose();
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~AutofacContainer()
        {
            Dispose(false);
        }

        public object Resolve(Type resolveType, string name = null)
        {
            var instance = string.IsNullOrEmpty(name) ? ComponentContext.Resolve(resolveType) : ComponentContext.ResolveNamed(name, resolveType);
            BuildUp(instance);
            return instance;
        }

        public IEnumerable ResolveAll(Type resolveType)
        {
            var type = typeof (IEnumerable<>);
            foreach (var instance in (IEnumerable) ComponentContext.Resolve(type.MakeGenericType(resolveType)))
            {
                BuildUp(instance);
                yield return instance;
            }
        }

        public bool CanResolve(Type resolveType, string name = null)
        {
            return string.IsNullOrEmpty(name) ? ComponentContext.IsRegistered(resolveType) : ComponentContext.IsRegisteredWithName(name, resolveType);
        }

        public virtual void BuildUp(object resolvedInstance)
        {
            this.PropertyInjection<DependencyAttribute>(
                resolvedInstance, (instance, propertyInfo, attributes) =>
                    {
                        var t = attributes.First().ResolveType;
                        return t ?? propertyInfo.PropertyType;
                    });
        }
    }
}
