using System;

namespace CB.Ioc
{
    public interface IRegisterOption
    {
        IRegisterOption AsSingleton();
        IRegisterOption AsMultiInstance();
        IRegisterOption As(Type asType);
        IRegisterOption Name(string name);
    }
}
