namespace FoxOffice
{
    using System;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Microsoft.WindowsAzure.Storage.Table;

    public class AppInitializer
    {
        private static volatile bool _appInitialized = false;
        private readonly AppSettings _settings;

        public AppInitializer(AppSettings settings) => _settings = settings;

        public void Initialize()
        {
            if (_appInitialized == false)
            {
                InitializeApp();
                _appInitialized = true;
            }
        }

        private void InitializeApp()
        {
            string connectionString = _settings.StorageConnectionString;
            var storage = CloudStorageAccount.Parse(connectionString);

            CloudQueueClient queueClient = storage.CreateCloudQueueClient();
            CreateQueue(queueClient, _settings.MessageQueueName);

            CloudTableClient tableClient = storage.CreateCloudTableClient();
            CreateTable(tableClient, _settings.EventStoreTableName);

            var documentClient = new DocumentClient(new Uri(_settings.CosmosDbEndpoint), _settings.CosmosDbAuthKey);
            CreateCollection(documentClient, _settings.ReadModelDatabaseId, _settings.ReadModelCollectionId);
        }

        private static void CreateQueue(CloudQueueClient queueClient, string queueName)
        {
            CloudQueue queue = queueClient.GetQueueReference(queueName);
            queue.CreateIfNotExistsAsync().GetAwaiter().GetResult();
        }

        private static void CreateTable(CloudTableClient tableClient, string tableName)
        {
            CloudTable table = tableClient.GetTableReference(tableName);
            table.CreateIfNotExistsAsync().GetAwaiter().GetResult();
        }

        private static void CreateCollection(DocumentClient documentClient, string databaseId, string collectionId)
        {
            var database = new Database { Id = databaseId };
            documentClient.CreateDatabaseIfNotExistsAsync(database).GetAwaiter().GetResult();

            Uri databaseUri = UriFactory.CreateDatabaseUri(databaseId);
            var collection = new DocumentCollection { Id = collectionId };
            documentClient.CreateDocumentCollectionIfNotExistsAsync(databaseUri, collection).GetAwaiter().GetResult();
        }
    }
}
