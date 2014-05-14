using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Autofac.Core;

namespace CB.Ioc.Adapter.Autofac
{
    public class AutofacParameterWrapper : IResolveParameter
    {
        public AutofacParameterWrapper(Parameter parameter)
        {
            Parameter = parameter;
        }

        public Parameter Parameter { get; private set; }

        #region Implementation of IResolveParameter

        public bool CanSupplyValue(ParameterInfo pi, IContainer context, out Func<object> valueProvider)
        {
            var autofacContainer = context as AutofacScopeResolver;
            if (autofacContainer == null)
            {
                throw new ArgumentException("context should be AutofacScopeResolver or children class", "context");
            }
            return Parameter.CanSupplyValue(pi, autofacContainer.ComponentContext, out valueProvider);
        }

        #endregion
    }
}
