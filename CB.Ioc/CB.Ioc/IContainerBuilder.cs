using System;
using System.Collections.Generic;

namespace CB.Ioc
{
    public interface IContainerBuilder
    {
        IRegisterOption Register(Type implementationType);

        IRegisterOption Register(object instance);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="creationFunction">second is the resolve parameter, the result is the instance</param>
        /// <returns></returns>
        IRegisterOption Register<TImplementationType>(Func<IScopeResolver, IEnumerable<IResolveParameter>, TImplementationType> creationFunction);
        
        IContainer BuildContainer();

        void UpdateContainer(IContainer container);
    }
}
