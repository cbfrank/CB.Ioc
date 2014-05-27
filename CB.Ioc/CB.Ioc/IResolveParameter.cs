using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CB.Ioc
{
    public interface IResolveParameter
    {
        bool CanSupplyValue(ParameterInfo pi, IScopeResolver context, out Func<object> valueProvider);
    }
}
