using System;
using System.Reflection;

namespace CB.Ioc
{
    public class NamedResolveParameter : IResolveParameter
    {
        public NamedResolveParameter(string name, object value)
        {
            Value = value;
            Name = name;
        }

        public string Name { get; private set; }
        public object Value { get; private set; }

        public bool CanSupplyValue(ParameterInfo pi, IContainer context, out Func<object> valueProvider)
        {
            valueProvider = null;
            if (pi.Name == Name)
            {
                valueProvider = () => Value;
                return true;
            }
            return false;
        }
    }
}
