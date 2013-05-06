using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CB.Ioc
{
    public static class IoCExtension
    {
        public static IRegisterOption Register<TReg, TImp>(this IContainerBuilder builder, string name = null)
        {
            var opt = builder.Register(typeof (TImp)).As(typeof (TReg));
            if (!string.IsNullOrEmpty(name))
            {
                opt.Name(name);
            }
            return opt;
        }

        public static IRegisterOption Register<TReg>(this IContainerBuilder builder, string name = null)
        {
            return Register<TReg, TReg>(builder, name);
        }

        public static IRegisterOption Register<TReg>(this IContainerBuilder builder, TReg instance, string name = null)
        {
            return builder.Register(instance).As(typeof (TReg)).Name(name);
        }

        public static bool CanResolve<T>(this IContainer container, string name = null)
        {
            return container.CanResolve(typeof(T), name);
        }

        public static TResolveType Resolve<TResolveType>(this IContainer container, string name = null)
            where TResolveType : class
        {
            if (container.CanResolve<TResolveType>(name))
            {
                return (TResolveType) (container.Resolve(typeof (TResolveType), name));
            }
            return default(TResolveType);
        }

        public static IEnumerable<TResolveType> ResolveAll<TResolveType>(this IContainer container)
        {
            foreach (var instance in container.ResolveAll(typeof (TResolveType)))
            {
                yield return (TResolveType) instance;
            }
        }

        private class TmpRegisterTypeInfo
        {
            public Type Type { get; set; }
            public TypeInjectionAttribute[] Attrs { get; set; }
        }

        public static void RegisterTypes(this IContainerBuilder builder, bool forceTypeInjectionAttribute = true, params Type[] typesArray)
        {
            var types = from t in typesArray
                        let attrs = t.GetCustomAttributes(typeof(TypeInjectionAttribute), false)
                        where (!forceTypeInjectionAttribute) || (attrs != null && attrs.Any())
                        select new TmpRegisterTypeInfo
                        {
                            Type = t,
                            Attrs = attrs == null ? new TypeInjectionAttribute[0] : attrs.Cast<TypeInjectionAttribute>().ToArray()
                        };
            foreach (var type in types)
            {
                //if no RegisterInfo is specified, means register it self
                if (type.Attrs == null || type.Attrs.Length <= 0)
                {
                    type.Attrs = new[] { new TypeInjectionAttribute() };
                }

                foreach (var attribute in type.Attrs)
                {
                    var reg = builder.Register(type.Type);
                    if (attribute != null)
                    {
                        if (attribute.AsType != null)
                        {
                            reg = reg.As(attribute.AsType);
                        }

                        if (!string.IsNullOrEmpty(attribute.Name))
                        {
                            reg = reg.Name(attribute.Name);
                        }

                        if (attribute.SingleInstance)
                        {
                            reg.AsSingleton();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="assembly"></param>
        /// <param name="forceTypeInjectionAttribute">true then only register the type marked with TypeInjectionAttribute, false register all types</param>
        public static void RegisterAssemblyTypes(this IContainerBuilder builder, Assembly assembly, bool forceTypeInjectionAttribute = true)
        {
            RegisterTypes(builder, forceTypeInjectionAttribute, assembly.GetTypes());
        }
    }
}
