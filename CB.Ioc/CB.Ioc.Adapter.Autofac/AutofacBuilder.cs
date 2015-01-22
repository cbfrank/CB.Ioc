using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Builder;
using Autofac.Core;

namespace CB.Ioc.Adapter.Autofac
{
    public class AutofacBuilder : IContainerBuilder
    {
        public AutofacBuilder()
            : this(new ContainerBuilder())
        {

        }

        public AutofacBuilder(ContainerBuilder builder)
        {
            Builder = builder;
        }

        public ContainerBuilder Builder { get; private set; }

        public IRegisterOption Register(Type implementationType)
        {
            if (implementationType.IsGenericTypeDefinition)
            {
                return new AutofacRegisterOption<object, ReflectionActivatorData, DynamicRegistrationStyle>(Builder.RegisterGeneric(implementationType), implementationType);
            }
            else
                return new AutofacRegisterOption<object, ConcreteReflectionActivatorData, SingleRegistrationStyle>(Builder.RegisterType(implementationType), implementationType);
        }

        public IRegisterOption Register(object instance)
        {
            return new AutofacRegisterOption<object, SimpleActivatorData, SingleRegistrationStyle>(Builder.RegisterInstance(instance), instance.GetType());
        }

        public IRegisterOption Register<TImplementationType>(Func<IScopeResolver, IEnumerable<IResolveParameter>, TImplementationType> creationFunction)
        {
            return
                new AutofacRegisterOption<TImplementationType, SimpleActivatorData, SingleRegistrationStyle>(
                    Builder.Register(
                        (context, parameters) =>
                        {
                            var enumerable = parameters as Parameter[] ?? parameters.ToArray();
                            var container = context.Resolve<IScopeResolver>() ?? context.Resolve<IContainer>();
                            return creationFunction(container, enumerable.Select(p => new AutofacParameterWrapper(p)));
                        }), typeof (TImplementationType));
        }

        public IContainer BuildContainer()
        {
            var c= new AutofacContainer();
            //remove the auto register of IContainer, user should always try to resolve the IScopeResolver not the IContainer to make it always get the correct scope
            //Builder.RegisterInstance(c).As<IContainer>();
            //var rootScope = new AutofacScopeResolver();
            Builder.RegisterInstance(c).As<IScopeResolver>();
            c.ContainerContext = Builder.Build();
            //c.ComponentContext = c.ContainerContext.Resolve<ILifetimeScope>();
            return c;
        }

        public void UpdateContainer(IContainer container)
        {
            if (!(container is AutofacContainer))
            {
                throw new ArgumentException("The container should be AutofacContainer", "container");
            }
            Builder.Update(((AutofacContainer)container).ContainerContext);
        }
    }
}
