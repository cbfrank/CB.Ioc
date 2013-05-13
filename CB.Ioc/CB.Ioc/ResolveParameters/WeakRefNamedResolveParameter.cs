using System;
using System.Reflection;

namespace CB.Ioc
{
    public class WeakRefNamedResolveParameter: IResolveParameter
    {
        public WeakRefNamedResolveParameter(string name, object value)
        {
            Value = new WeakReference(value);
            Name = name;
        }

        public string Name { get; private set; }
        public WeakReference Value { get; private set; }

        public bool CanSupplyValue(ParameterInfo pi, IContainer context, out Func<object> valueProvider)
        {
            valueProvider = null;
            if (pi.Name == Name)
            {
                valueProvider = () => Value.IsAlive ? Value.Target : null;
                return true;
            }
            return false;
        }
    }
}
