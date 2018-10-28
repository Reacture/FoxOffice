namespace FoxOffice.ReadModel
{
    using System;
    using Autofac;
    using FoxOffice.ReadModel.DataAccess;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;

    public class ReadModelModule : Module
    {
        private readonly string _cosmosDbEndpoint;
        private readonly string _cosmosDbAuthKey;
        private readonly string _databaseId;
        private readonly string _collectionId;

        public ReadModelModule(
            string cosmosDbEndpoint,
            string cosmosDbAuthKey,
            string databaseId,
            string collectionId)
        {
            _cosmosDbEndpoint = cosmosDbEndpoint;
            _cosmosDbAuthKey = cosmosDbAuthKey;
            _databaseId = databaseId;
            _collectionId = collectionId;
        }

        private IDocumentClient GetDocumentClient()
        {
            var endpoint = new Uri(_cosmosDbEndpoint);
            return new DocumentClient(endpoint, _cosmosDbAuthKey);
        }

        private CollectionReference GetCollectionReference() =>
            new CollectionReference(_databaseId, _collectionId);

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(GetDocumentClient);
            builder.Register(GetCollectionReference);

            builder.RegisterImplementation<CosmosDbTheaterRepository>();
            builder.RegisterType<TheaterReadModelGenerator>();
            builder.RegisterType<TheaterReadModelFacade>();

            builder.RegisterImplementation<CosmosDbMovieRepository>();
            builder.RegisterType<MovieReadModelGenerator>();
            builder.RegisterType<MovieReadModelFacade>();
        }
    }
}
