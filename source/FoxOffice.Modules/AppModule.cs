namespace FoxOffice
{
    using Autofac;
    using FoxOffice.Domain;
    using FoxOffice.Messaging;
    using FoxOffice.ReadModel;

    public class AppModule : Module
    {
        private readonly AppInitializer _appInitializer;
        private readonly Module[] _modules;

        public AppModule(AppSettings settings)
        {
            _appInitializer = new AppInitializer(settings);

            _modules = new Module[]
            {
                new MessagingModule(
                    settings.StorageConnectionString,
                    settings.MessageQueueName),

                new DomainModule(
                    settings.StorageConnectionString,
                    settings.EventStoreTableName),

                new ReadModelModule(
                    settings.CosmosDbEndpoint,
                    settings.CosmosDbAuthKey,
                    settings.ReadModelDatabaseId,
                    settings.ReadModelCollectionId),
            };
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_appInitializer);

            foreach (Module module in _modules)
            {
                builder.RegisterModule(module);
            }
        }
    }
}
