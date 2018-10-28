namespace FoxOffice.Messaging
{
    using System;
    using Autofac;
    using FoxOffice.Domain;
    using FoxOffice.ReadModel;
    using Khala.Messaging;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Queue;

    public class MessagingModule : Module
    {
        private readonly string _storageConnectionString;
        private readonly string _messageQueueName;
        private readonly Lazy<CloudStorageAccount> _accountLazy;
        private readonly Lazy<CloudQueue> _queueLazy;

        public MessagingModule(
            string storageConnectionString, string messageQueueName)
        {
            _storageConnectionString = storageConnectionString;
            _messageQueueName = messageQueueName;
            _accountLazy = new Lazy<CloudStorageAccount>(GetAccount);
            _queueLazy = new Lazy<CloudQueue>(GetQueue);
        }

        private CloudStorageAccount GetAccount()
        {
            return CloudStorageAccount.Parse(_storageConnectionString);
        }

        private CloudQueue GetQueue()
        {
            CloudStorageAccount account = _accountLazy.Value;
            CloudQueueClient client = account.CreateCloudQueueClient();
            return client.GetQueueReference(_messageQueueName);
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterImplementation<JsonMessageSerializer>();
            builder.Register(GetMessageBus);
            builder.Register(GetMessageHandler);
        }

        private IMessageBus GetMessageBus() =>
            new QueueMessageBus(_queueLazy.Value);

        private IMessageHandler GetMessageHandler(IComponentContext context) =>
            new CompositeMessageHandler(
                context.Resolve<TheaterCommandHandler>(),
                context.Resolve<TheaterReadModelGenerator>(),
                context.Resolve<MovieCommandHandler>(),
                context.Resolve<MovieReadModelGenerator>());
    }
}
