namespace FoxOffice
{
    using System.Threading.Tasks;
    using Autofac;
    using Khala.Messaging;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    public static class ProcessorFunction
    {
        [FunctionName("ProcessorFunction")]
        public static Task Process(
            [QueueTrigger("messages")] string value,
            ExecutionContext context,
            ILogger log)
        {
            IContainer app = GetContainer(context);
            app.Resolve<AppInitializer>().Initialize();
            return app.HandleMessage(value);
        }

        private static IContainer GetContainer(ExecutionContext context)
        {
            IConfiguration config = GetConfiguration(context);
            return GetContainer(config);
        }

        private static IConfiguration GetConfiguration(ExecutionContext context)
        {
            return new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile(
                    "local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }

        private static IContainer GetContainer(IConfiguration config)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new AppModule(GetSettings(config)));
            return builder.Build();
        }

        private static AppSettings GetSettings(IConfiguration config) =>
            new AppSettings(
                config.GetConnectionString("Storage"),
                config["Messaging:Storage:QueueName"],
                config["Domain:Storage:EventStoreTableName"],
                config["ReadModel:CosmosDb:Endpoint"],
                config["ReadModel:CosmosDb:AuthKey"],
                config["ReadModel:CosmosDb:DatabaseId"],
                config["ReadModel:CosmosDb:CollectionId"]);

        private static async Task HandleMessage(
            this IContainer container, string value)
        {
            IMessageSerializer serializer =
                container.Resolve<IMessageSerializer>();

            if (serializer.Deserialize(value) is object message)
            {
                IMessageHandler handler = container.Resolve<IMessageHandler>();
                await handler.Handle(new Envelope(message));
            }
        }
    }
}
