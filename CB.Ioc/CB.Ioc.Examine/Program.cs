using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using CB.Ioc;
using CB.Ioc.Adapter.Autofac;

namespace CB.Ioc.Examine
{
    class Program
    {
        static void Main(string[] args)
        {
            var auto = new Autofac.ContainerBuilder();
            auto.RegisterType<A>();
            //auto.Register(context => new A(null)).As<A>();
            var c = auto.Build();
            var t = c.Resolve<ILifetimeScope>();
            var t2 = c.BeginLifetimeScope("c.t2");
            var t3 = t2.BeginLifetimeScope("c.t2.t3");

            var t2_ = t2.Resolve<ILifetimeScope>();
            var t2__ = t2.Resolve<ILifetimeScope>();
            var yes = t2 == t2_;
            yes = t2_ == t2__;
            var t3_ = t3.Resolve<ILifetimeScope>();
            //var aa = t3.Resolve<A>();


            CB.Ioc.IContainerBuilder cb = new CB.Ioc.Adapter.Autofac.AutofacBuilder();
            cb.Register<B>().SingletonPerLifetimeScope();
            cb.Register<A>();
            var cbc = cb.BuildContainer();
            var cbt2 = cbc.BeginLifetimeScope("cb.t2");
            var cbt3 = cbt2.BeginLifetimeScope("cb.t2.t3");

            var a = cbc.Resolve<A>();
            var b2 = cbt2.Resolve<B>();
            var a2 = cbt2.Resolve<A>();
            var a3 = cbt3.Resolve<A>();
            var b3 = cbt3.Resolve<B>();
            if (a2.B == b2)
            {
                
            }
        }
    }

    public class A
    {
        public A(IScopeResolver container)
        {
            Container = container;
        }

        [Dependency]
        public B B { get; set; }

        public IScopeResolver Container { get; private set; }
    }

    public class B
    {
        public B()
        {
            Tag = Guid.NewGuid().ToString();
        }

        public string Tag { get; private set; }
    }
}
