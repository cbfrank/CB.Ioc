using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using CB.Ioc;

namespace CB.Ioc.Examine
{
    class Program
    {
        static void Main(string[] args)
        {
            var auto = new Autofac.ContainerBuilder();
            auto.RegisterType<A>();
            var c = auto.Build();
            var t = c.Resolve<ILifetimeScope>();
            var t2 = c.BeginLifetimeScope("c.t2");
            var t3 = t2.BeginLifetimeScope("c.t2.t3");

            var t2_ = t2.Resolve<ILifetimeScope>();
            var t2__ = t2.Resolve<ILifetimeScope>();
            var yes = t2 == t2_;
            yes = t2_ == t2__;
            var t3_ = t3.Resolve<ILifetimeScope>();


            CB.Ioc.IContainerBuilder cb = new CB.Ioc.Adapter.Autofac.AutofacBuilder();
            cb.Register<A>();
            var cbc = cb.BuildContainer();
            //var cbt2 = cbc.BeginLifetimeScope("cb.t2");
            //var cbt3 = cbt2.BeginLifetimeScope("cb.t2.t3");

            //var cbt2_ = cbt2.Resolve<IScopeResolver>();
            //var cbt2__ = cbt2.Resolve<IScopeResolver>();
            //var cbt2___ = cbt2_.Resolve<IScopeResolver>();

            //var cbt3_ = cbt3.Resolve<IScopeResolver>();
            var aa = cbc.Resolve<A>();
        }
    }

    public class A
    {
        public A(IScopeResolver container)
        {
            
        }
    }
}
