﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac.Builder;

namespace CB.Ioc.Adapter.Autofac
{
    public class AutofacRegisterOption<TLimit, TActivatorData, TRegistrationStyle> : IRegisterOption
    {
        public AutofacRegisterOption(IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> registrationBuilder, Type asType)
        {
            FRegistrationBuilder = registrationBuilder;
            _AsType = asType;
        }

        protected IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> FRegistrationBuilder;
        
        private bool? _Singleton;

        public IRegisterOption AsSingleton()
        {
            if (_Singleton.HasValue)
            {
                throw new InvalidOperationException("Can NOT invoke AsSingleton or AsMultiInstance twice");
            }
            _Singleton = true;
            FRegistrationBuilder = FRegistrationBuilder.SingleInstance();
            return this;
        }

        public IRegisterOption AsMultiInstance()
        {
            if (_Singleton.HasValue)
            {
                throw new InvalidOperationException("Can NOT invoke AsSingleton or AsMultiInstance twice");
            }
            _Singleton = false;
            return this;
        }

        private Type _AsType;

        public IRegisterOption As(Type asType)
        {
            FRegistrationBuilder = FRegistrationBuilder.As(asType);
            _AsType = asType;
            return this;
        }

        public IRegisterOption Name(string name)
        {
            FRegistrationBuilder = FRegistrationBuilder.Named(name, _AsType);
            return this;
        }
    }
}
