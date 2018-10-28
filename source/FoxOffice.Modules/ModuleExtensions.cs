namespace FoxOffice
{
    using System;
    using Autofac;

    internal static class ModuleExtensions
    {
        public static void Register<T>(
            this ContainerBuilder builder, Func<T> factory)
        {
            builder.Register(_ => factory.Invoke());
        }

        public static void RegisterImplementation<T>(
            this ContainerBuilder builder)
        {
            builder.RegisterType<T>().AsImplementedInterfaces();
        }
    }
}
