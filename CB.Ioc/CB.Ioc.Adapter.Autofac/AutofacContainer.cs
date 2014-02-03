using System;
using Autofac;

namespace CB.Ioc.Adapter.Autofac
{
    public class AutofacContainer : AutofacScopeResolver, IContainer
    {
        public virtual global::Autofac.IContainer ContainerContext
        {
            get { return (global::Autofac.IContainer) ComponentContext; }
            internal set { ComponentContext = value; }
        }

        #region Overrides of AutofacScopeResolver

        public override ILifetimeScope ComponentContext
        {
            get { return base.ComponentContext; }
            internal set
            {
                if (value != null && (!(value is global::Autofac.IContainer)))
                {
                    throw new ArgumentException("ComponentContext should be Autofac.IContainer", "value");
                }
                base.ComponentContext = value;
            }
        }

        #endregion
    }
}
