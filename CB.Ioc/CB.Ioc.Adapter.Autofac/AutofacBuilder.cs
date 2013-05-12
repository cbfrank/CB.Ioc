using System;
using Autofac;
using Autofac.Builder;

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
            return new AutofacRegisterOption<object, ConcreteReflectionActivatorData, SingleRegistrationStyle>(Builder.RegisterType(implementationType), implementationType);
        }

        public IRegisterOption Register(object instance)
        {
            return new AutofacRegisterOption<object, SimpleActivatorData, SingleRegistrationStyle>(Builder.RegisterInstance(instance), instance.GetType());
        }

        public IRegisterOption Register<TImplementationType>(Func<IContainer, object, TImplementationType> creationFunction)
        {
            return
                new AutofacRegisterOption<TImplementationType, SimpleActivatorData, SingleRegistrationStyle>(
                    Builder.Register((context, parameters) => creationFunction(context.Resolve<IContainer>(), parameters)), typeof(TImplementationType));
        }

        public IContainer BuildContainer()
        {
            var c= new AutofacContainer();
            Builder.RegisterInstance(c).As<IContainer>();
            c.ComponentContext = Builder.Build();
            return c;
        }

        public void UpdateContainer(IContainer container)
        {
            if (!(container is AutofacContainer))
            {
                throw new ArgumentException("The container should be AutofacContainer", "container");
            }
            Builder.Update(((AutofacContainer) container).ComponentContext);
        }
    }
}
