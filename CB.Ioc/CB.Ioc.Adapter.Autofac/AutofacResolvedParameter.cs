using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Autofac;
using Autofac.Core;

namespace CB.Ioc.Adapter.Autofac
{
    public class AutofacResolvedParameter : Parameter
    {
        public AutofacResolvedParameter(IResolveParameter resolveParameter)
        {
            ResolveParameter = resolveParameter;
        }

        public IResolveParameter ResolveParameter { get; private set; }

        public override bool CanSupplyValue(ParameterInfo pi, IComponentContext context, out Func<object> valueProvider)
        {
            return ResolveParameter.CanSupplyValue(pi, context.Resolve<IContainer>(), out valueProvider);
        }
    }
}
