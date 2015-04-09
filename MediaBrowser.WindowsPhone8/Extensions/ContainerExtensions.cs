using System;
using GalaSoft.MvvmLight.Ioc;

namespace Emby.WindowsPhone.Extensions
{
    public static class ContainerExtensions
    {
        public static void RegisterIf<TInterface, TClass>(this SimpleIoc container)
            where TInterface : class
            where TClass : class
        {
            if(!container.IsRegistered<TInterface>())
                container.Register<TInterface, TClass>();
        }

        public static void RegisterIf<TInterface, TClass>(this SimpleIoc container, bool createInstanceImmediately)
            where TInterface : class
            where TClass : class
        {
            if(!container.IsRegistered<TInterface>())
                container.Register<TInterface, TClass>();
        }

        public static void RegisterIf<TClass>(this SimpleIoc container, bool createInstanceImmediately) where TClass : class
        {
            if(!container.IsRegistered<TClass>())
                container.Register<TClass>(createInstanceImmediately);
        }

        public static void RegisterIf<TClass>(this SimpleIoc container) where TClass : class
        {
            if (!container.IsRegistered<TClass>())
                container.Register<TClass>();
        }

        public static void RegisterIf<TClass>(this SimpleIoc container, Func<TClass> factory) where TClass : class
        {
            if(!container.IsRegistered<TClass>())
                container.Register<TClass>(factory);
        }
    }
}
