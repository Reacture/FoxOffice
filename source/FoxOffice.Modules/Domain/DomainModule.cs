namespace FoxOffice.Domain
{
    using System;
    using Autofac;
    using Khala.EventSourcing.Azure;
    using Khala.Messaging;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;

    public class DomainModule : Module
    {
        private readonly string _storageConnectionString;
        private readonly string _eventStoreTableName;
        private readonly Lazy<CloudStorageAccount> _accountLazy;
        private readonly Lazy<CloudTable> _tableLazy;

        public DomainModule(
            string storageConnectionString, string eventStoreTableName)
        {
            _storageConnectionString = storageConnectionString;
            _eventStoreTableName = eventStoreTableName;
            _accountLazy = new Lazy<CloudStorageAccount>(GetAccount);
            _tableLazy = new Lazy<CloudTable>(GetTable);
        }

        private CloudStorageAccount GetAccount()
        {
            return CloudStorageAccount.Parse(_storageConnectionString);
        }

        private CloudTable GetTable()
        {
            CloudStorageAccount account = _accountLazy.Value;
            CloudTableClient client = account.CreateCloudTableClient();
            return client.GetTableReference(_eventStoreTableName);
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(GetEventStore);
            builder.Register(GetEventPublisher);

            builder.RegisterFactory(Theater.Factory);
            builder.RegisterRepository<Theater>();
            builder.RegisterType<TheaterCommandHandler>();

            builder.RegisterFactory(Movie.Factory);
            builder.RegisterRepository<Movie>();
            builder.RegisterType<MovieCommandHandler>();
        }

        private IAzureEventStore GetEventStore(IComponentContext context)
        {
            return new AzureEventStore(
                eventTable: _tableLazy.Value,
                serializer: context.Resolve<IMessageSerializer>());
        }

        private IAzureEventPublisher GetEventPublisher(
            IComponentContext context)
        {
            return new AzureEventPublisher(
                eventTable: _tableLazy.Value,
                serializer: context.Resolve<IMessageSerializer>(),
                messageBus: context.Resolve<IMessageBus>());
        }
    }
}
