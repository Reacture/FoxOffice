namespace FoxOffice.Domain
{
    using System;
    using System.Collections.Generic;
    using Khala.EventSourcing;
    using Khala.EventSourcing.Sql;
    using Khala.Messaging;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Diagnostics;

    public static class EventSourcing
    {
        private static readonly Func<EventStoreDbContext> _contextFactory;
        private static readonly IMessageSerializer _serializer;
        private static readonly SqlEventStore _eventStore;

        static EventSourcing()
        {
            DbContextOptions options = new DbContextOptionsBuilder()
                .UseInMemoryDatabase(nameof(EventSourcing))
                .ConfigureWarnings(ConfigureWarnings)
                .Options;

            _contextFactory = () => new EventStoreDbContext(options);
            _serializer = new JsonMessageSerializer();
            _eventStore = new SqlEventStore(_contextFactory, _serializer);
        }

        private static void ConfigureWarnings(
            WarningsConfigurationBuilder builder)
        {
            builder.Ignore(InMemoryEventId.TransactionIgnoredWarning);
        }

        public static IEventSourcedRepository<T> GetRepository<T>(
            IMessageBus messageBus,
            Func<Guid, IEnumerable<IDomainEvent>, T> factory)
            where T : class, IEventSourced
        {
            return new SqlEventSourcedRepository<T>(
                _eventStore,
                new SqlEventPublisher(_contextFactory, _serializer, messageBus),
                factory);
        }

        public static IEventSourcedRepository<T> GetRepository<T>(
            Func<Guid, IEnumerable<IDomainEvent>, T> factory)
            where T : class, IEventSourced
        {
            return GetRepository<T>(new InProcessMessageLogger(), factory);
        }
    }
}
