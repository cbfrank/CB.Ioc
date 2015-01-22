using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Core;

namespace CB.Ioc.Adapter.Autofac
{
    public class AutofacScopeResolver : IScopeResolver
    {
        public AutofacScopeResolver()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; private set; }
        public virtual ILifetimeScope ComponentContext { get; internal set; }

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

        ~AutofacScopeResolver()
        {
            Dispose(false);
        }

        public virtual object Resolve(Type resolveType, string name, params IResolveParameter[] parameters)
        {
            var @params = parameters.Select(p => (Parameter) new AutofacResolvedParameter(p)).ToArray();
            object instance;
            /*if (resolveType == typeof (IScopeResolver))
            {
                instance = string.IsNullOrEmpty(name) ? ComponentContext.Resolve<ILifetimeScope>(@params) : ComponentContext.ResolveNamed<ILifetimeScope>(name, @params);
                instance = new AutofacScopeResolver()
                {
                    ComponentContext = (ILifetimeScope) instance
                };
            }
            else*/
            {
                instance = string.IsNullOrEmpty(name) ? ComponentContext.Resolve(resolveType, @params) : ComponentContext.ResolveNamed(name, resolveType, @params);
            }
            BuildUp(instance);
            return instance;
        }

        public object Resolve(Type resolveType, params IResolveParameter[] parameters)
        {
            return Resolve(resolveType, null, parameters);
        }

        public virtual IEnumerable<object> ResolveAll(Type resolveType, params IResolveParameter[] parameters)
        {
            var type = typeof (IEnumerable<>);
            var @params = parameters.Select(p => (Parameter) new AutofacResolvedParameter(p)).ToArray();
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

        public IScopeResolver BeginLifetimeScope()
        {
            return BeginLifetimeScope(builder => { });
        }

        public IScopeResolver BeginLifetimeScope(object tag)
        {
            var scope = new AutofacScopeResolver();
            scope.ComponentContext = ComponentContext.BeginLifetimeScope(
                tag,
                builder => builder.Register<IScopeResolver>(context => scope));
            return scope;
        }

        public IScopeResolver BeginLifetimeScope(Action<IContainerBuilder> configurationAction)
        {
            var scope = new AutofacScopeResolver();
            scope.ComponentContext = ComponentContext.BeginLifetimeScope(
                builder =>
                {
                    var containerBuilder = new AutofacBuilder(builder);
                    configurationAction(containerBuilder);
                    builder.Register<IScopeResolver>(context => scope);
                });
            return scope;
        }

        public virtual void BuildUp(object resolvedInstance)
        {
            //although we have already register property injection this on "OnActivated" of AutofacRegisterOption
            //but we have do it again for some case that user directly build up an existing instance
            //and as property injection won't inject for a property that have value so it is ok to run this twice
            this.PropertyInjection<DependencyAttribute>(resolvedInstance, IoCExtension.DefaultOverrideTypeResolveFunc);
        }
    }
}