using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CB.Ioc
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class TypeInjectionByMethodAttribute : Attribute
    {
        
    }
}
