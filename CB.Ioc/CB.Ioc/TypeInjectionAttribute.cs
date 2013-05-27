using System;

namespace CB.Ioc
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class TypeInjectionAttribute : Attribute
    {
        public TypeInjectionAttribute()
        {
            SingleInstance = false;
        }

        public string Name { get; set; }
        public Type AsType { get; set; }
        public bool SingleInstance { get; set; }
    }
}
