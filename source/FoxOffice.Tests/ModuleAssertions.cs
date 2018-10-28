namespace FoxOffice
{
    using System;
    using System.Collections.Generic;
    using Autofac;
    using FluentAssertions;
    using Khala.EventSourcing;
    using Khala.EventSourcing.Azure;

    public static class ModuleAssertions
    {
        public static void AssertServiceRegistered<T>(
            this IContainer container)
            where T : class
        {
            T service = container.ResolveOptional<T>();
            service.Should().NotBeNull();
        }

        public static void AssertServiceRegistered<T>(
            this IContainer container, T expected)
            where T : class
        {
            T service = container.ResolveOptional<T>();
            service.Should().NotBeNull().And.Be(expected);
        }

        public static void AssertFactoryRegistered<T>(
            this IContainer container,
            Func<Guid, IEnumerable<IDomainEvent>, T> factory)
            where T : IEventSourced
        {
            AssertServiceRegistered(container, factory);
        }

        public static void AssertRepositoryRegistered<T>(
            this IContainer container)
             where T : class, IEventSourced
        {
            IEventSourcedRepository<T> service =
                container.ResolveOptional<IEventSourcedRepository<T>>();

            service.Should().BeOfType<AzureEventSourcedRepository<T>>();
        }

        public static void AssertServiceRegistered<TService, TImplementation>(
            this IContainer container)
            where TService : class
            where TImplementation : TService
        {
            TService service = container.ResolveOptional<TService>();

            service.Should().BeOfType<TImplementation>();
        }
    }
}
