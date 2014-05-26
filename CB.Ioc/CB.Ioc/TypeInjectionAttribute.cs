using System;

namespace CB.Ioc
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class TypeInjectionAttribute : Attribute
    {
        public TypeInjectionAttribute()
        {
            SingleInstance = SingleInstance.None;
        }

        public string Name { get; set; }
        public Type AsType { get; set; }
        public SingleInstance SingleInstance { get; set; }
    }

    public enum SingleInstance
    {
        None,
        SingleInstance,
        SingletonPerLifetimeScope
    }
}
