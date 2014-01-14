using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity;

namespace SUT.PrintEngine.Extensions
{
    public static class UnityContainerExtention
    {
        private static Dictionary<Type, Type> _registeredTypes;

        public static Dictionary<Type, Type> RegisteredTypes
        {
            get
            {
                if(_registeredTypes==null)
                    _registeredTypes=new Dictionary<Type, Type>();
                return _registeredTypes;
            }
        }

        public static void RegisterType(this IUnityContainer source, Type fromType, Type toType, string key)
        {
            if (RegisteredTypes.ContainsKey(fromType) == false)
            {
                RegisteredTypes.Add(fromType, toType);
            }
            source.RegisterType(fromType, toType, key, new ContainerControlledLifetimeManager());
        }

        public static void RegisterType(this IUnityContainer source, Type fromType, Type toType, bool registerAsSingleton)
        {
            if(RegisteredTypes.ContainsKey(fromType)==false)
            {
                RegisteredTypes.Add(fromType,toType);
            }
            if (registerAsSingleton)
            {
                source.RegisterType(fromType, toType, new ContainerControlledLifetimeManager());
            }
            else
            {
                source.RegisterType(fromType, toType);
            }
        }
        public static bool IsRegisteredType(this IUnityContainer source, Type fromType)
        {
            return RegisteredTypes.ContainsKey(fromType);
        }

        public static void RegisterInstance(this IUnityContainer source, Type type, object instance, bool registerAsSingleton)
        {
            if (registerAsSingleton)
            {
                source.RegisterInstance(type, instance, new ContainerControlledLifetimeManager());
            }
            else
            {
                source.RegisterInstance(type, instance);
            }
        }
    }
}
