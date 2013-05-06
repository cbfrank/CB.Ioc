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

        public TypeInjectionAttribute(object name)
            : this()
        {
            Name = name.ToString();
        }

        public string Name { get; set; }
        public Type AsType { get; set; }
        public bool SingleInstance { get; set; }
    }
}
