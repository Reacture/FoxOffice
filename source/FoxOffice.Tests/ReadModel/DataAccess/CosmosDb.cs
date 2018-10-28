namespace FoxOffice.ReadModel.DataAccess
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Azure.Documents.Linq;
    using static Microsoft.Azure.Documents.Client.UriFactory;

    public static class CosmosDb
    {
        public const string Endpoint = "https://localhost:8081";
        public const string AuthKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        public const string DatabaseId = "FoxOfficeTestDatabase";
        public const string CollectionId = "DefaultCollection";

        public static DocumentClient DocumentClient { get; private set; }

        public static CollectionReference Collection { get; private set; }

        public static Uri DatabaseUri { get; private set; }

        public static Uri CollectionUri { get; private set; }

        public static async Task Initialize()
        {
            DocumentClient = new DocumentClient(new Uri(Endpoint), AuthKey);
            Collection = new CollectionReference(DatabaseId, CollectionId);
            DatabaseUri = CreateDatabaseUri(DatabaseId);
            CollectionUri = CreateDocumentCollectionUri(DatabaseId, CollectionId);

            await DocumentClient.CreateDatabaseIfNotExistsAsync(new Database { Id = DatabaseId });
            await InitializeCollection(CollectionId);
        }

        public static async Task InitializeCollection(string collectionId)
        {
            try
            {
                Uri collectionUri = CreateDocumentCollectionUri(DatabaseId, collectionId);
                await DocumentClient.DeleteDocumentCollectionAsync(collectionUri);
            }
            catch
            {
            }

            var collection = new DocumentCollection { Id = collectionId };
            await DocumentClient.CreateDocumentCollectionIfNotExistsAsync(DatabaseUri, collection);
        }

        public static IOrderedQueryable<T> CreateDocumentQuery<T>(
            this IDocumentClient client)
        {
            return client.CreateDocumentQuery<T>(CollectionUri);
        }

        public static async Task<T> SingleOrDefault<T>(this IQueryable<T> query)
        {
            FeedResponse<T> response = await
                query.AsDocumentQuery().ExecuteNextAsync<T>();

            return response.SingleOrDefault();
        }

        public static Task CreateDocument(string collectionId, object document)
        {
            Uri collectionUri =
                CreateDocumentCollectionUri(DatabaseId, collectionId);

            return DocumentClient.CreateDocumentAsync(collectionUri, document);
        }
    }
}
