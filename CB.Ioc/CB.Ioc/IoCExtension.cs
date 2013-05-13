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

        public static TResolveType Resolve<TResolveType>(this IContainer container, string name, params IResolveParameter[] parameters)
            where TResolveType : class
        {
            if (container.CanResolve<TResolveType>(name))
            {
                return (TResolveType) (container.Resolve(typeof (TResolveType), name, parameters));
            }
            return default(TResolveType);
        }

        public static TResolveType Resolve<TResolveType>(this IContainer container, params IResolveParameter[] parameters)
            where TResolveType : class
        {
            return Resolve<TResolveType>(container, null, parameters);
        }

        public static IEnumerable<TResolveType> ResolveAll<TResolveType>(this IContainer container, params IResolveParameter[] parameters)
        {
            foreach (var instance in container.ResolveAll(typeof (TResolveType), parameters))
            {
                yield return (TResolveType) instance;
            }
        }

        private class TmpRegisterTypeInfo
        {
            public Type Type { get; set; }
            public TypeInjectionAttribute[] Attributes { get; set; }
        }

        public static void RegisterTypes(this IContainerBuilder builder, bool forceTypeInjectionAttribute = true, params Type[] typesArray)
        {
            var types = from t in typesArray
                        let attributes = t.GetCustomAttributes(typeof(TypeInjectionAttribute), false)
                        where (!forceTypeInjectionAttribute) || (attributes != null && attributes.Any())
                        select new TmpRegisterTypeInfo
                        {
                            Type = t,
                            Attributes = attributes == null ? new TypeInjectionAttribute[0] : attributes.Cast<TypeInjectionAttribute>().ToArray()
                        };
            foreach (var type in types)
            {
                //if no RegisterInfo is specified, means register it self
                if (type.Attributes == null || type.Attributes.Length <= 0)
                {
                    type.Attributes = new[] { new TypeInjectionAttribute() };
                }

                foreach (var attribute in type.Attributes)
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

        public static bool TryResolve(this IContainer container, Type resolvedType, string name, out object instance, params IResolveParameter[] parameters)
        {
            instance = null;
            if (!container.CanResolve(resolvedType, name))
            {
                return false;
            }
            if (string.IsNullOrEmpty(name))
            {
                instance = container.Resolve(resolvedType, parameters);
            }
            else
            {
                instance = container.Resolve(resolvedType, name, parameters);
            }
            return true;
        }

        public static bool TryResolve(this IContainer container, Type resolvedType, out object instance, params IResolveParameter[] parameters)
        {
            return TryResolve(container, resolvedType, null, out instance, parameters);
        }

        public static bool TryResolve<T>(this IContainer container, string name, out T instance, params IResolveParameter[] parameters) where T : class
        {
            object obj;
            var result = TryResolve(container, typeof (T), name, out obj, parameters);
            instance = (T)obj;
            return result;
        }

        public static bool TryResolve<T>(this IContainer container, out T instance, params IResolveParameter[] parameters) where T : class
        {
            return TryResolve(container, null, out instance, parameters);
        }

        public static void PropertyInjection(this IContainer container, object instance, Type attributeType, Func<object, PropertyInfo, IEnumerable<object>, Type> overrideTypeResolveFunc)
        {
            if (!attributeType.IsSubclassOf(typeof(Attribute)))
            {
                //ToDO Message
                throw new ArgumentOutOfRangeException("", "attributeType");
            }
            var properties = from p in instance.GetType().GetProperties()
                             where p.CanWrite && p.GetIndexParameters().Length <= 0
                             let attributes = p.GetCustomAttributes(attributeType, true)
                             where attributes.Length > 0
                             select new
                             {
                                 PropertyInfo = p,
                                 Attributes = attributes
                             };
            foreach (var prop in properties)
            {
                var value = prop.PropertyInfo.GetValue(instance, null);
                if (value != null)
                {
                    continue;
                }
                value = null;
                var overrideTypeResolve = prop.PropertyInfo.PropertyType;
                if (overrideTypeResolveFunc != null)
                {
                    overrideTypeResolve = overrideTypeResolveFunc(instance, prop.PropertyInfo, prop.Attributes);
                }
                if (!prop.PropertyInfo.PropertyType.IsAssignableFrom(overrideTypeResolve))
                {
                    throw new ArgumentException(
                        string.Format("the return value of overrideTypeResolveFunc should be able to assigned to property {0}.{1}",
                                      prop.PropertyInfo.Name, instance.GetType().FullName), "overrideTypeResolveFunc");
                }
                if (!TryResolve(container, overrideTypeResolve, out value))
                {
                    continue;
                }
                if (value != null)
                {
                    prop.PropertyInfo.SetValue(instance, value, null);
                }
            }
        }

        public static void PropertyInjection<TAttr>(this IContainer container, object instance,
                                                       Func<object, PropertyInfo, IEnumerable<TAttr>, Type> overrideTypeResolveFunc)
            where TAttr : Attribute
        {
            PropertyInjection(
                container, instance, typeof (TAttr),
                (ins, propertyInfo, attributes) =>
                overrideTypeResolveFunc(ins, propertyInfo, attributes.Cast<TAttr>()));
        }
    }
}
