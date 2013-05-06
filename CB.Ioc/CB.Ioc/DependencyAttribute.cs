using System;

namespace CB.Ioc
{
    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class DependencyAttribute : Attribute
    {
        public Type ResolveType { get; set; }
    }
}
