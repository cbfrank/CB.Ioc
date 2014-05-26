using System;

namespace CB.Ioc
{
    public interface IRegisterOption
    {
        IRegisterOption AsSingleton();
        IRegisterOption SingletonPerLifetimeScope();
        IRegisterOption AsMultiInstance();
        IRegisterOption As(Type asType);
        IRegisterOption Name(string name);
    }
}
