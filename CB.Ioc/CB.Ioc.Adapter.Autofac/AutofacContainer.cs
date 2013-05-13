using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Core;

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

        public object Resolve(Type resolveType, string name, params IResolveParameter[] parameters)
        {
            object instance;
            var @params = parameters.Select(p => (Parameter) new AutofacResolvedParameter(p)).ToArray();
            if (string.IsNullOrEmpty(name))
            {
                instance = ComponentContext.Resolve(resolveType, @params);
            }
            else
            {
                instance = ComponentContext.ResolveNamed(name, resolveType, @params);
            }
            BuildUp(instance);
            return instance;
        }

        public object Resolve(Type resolveType, params IResolveParameter[] parameters)
        {
            return Resolve(resolveType, null, parameters);
        }

        public IEnumerable ResolveAll(Type resolveType, params IResolveParameter[] parameters)
        {
            var type = typeof(IEnumerable<>);
            var @params = parameters.Select(p => (Parameter)new AutofacResolvedParameter(p)).ToArray();
            foreach (var instance in (IEnumerable) ComponentContext.Resolve(type.MakeGenericType(resolveType), @params))
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
