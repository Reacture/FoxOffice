namespace FoxOffice.Domain
{
    using System;
    using System.Collections.Generic;
    using Autofac;
    using Khala.EventSourcing;
    using Khala.EventSourcing.Azure;

    internal static class DomainModuleExtensions
    {
        public static void RegisterFactory<T>(
            this ContainerBuilder builder,
            Func<Guid, IEnumerable<IDomainEvent>, T> factory)
        {
            builder.RegisterInstance(factory);
        }

        public static void RegisterRepository<T>(this ContainerBuilder builder)
             where T : class, IEventSourced
        {
            builder.RegisterImplementation<AzureEventSourcedRepository<T>>();
        }
    }
}
