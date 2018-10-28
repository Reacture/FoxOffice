namespace FoxOffice.ReadModel.DataAccess
{
    using System;
    using Microsoft.Azure.Documents.Client;

    public class CollectionReference
    {
        public CollectionReference(string databaseId, string collectionId)
        {
            DatabaseId = databaseId;
            CollectionId = collectionId;
            Uri = new Lazy<Uri>(CreateUri);
        }

        public string DatabaseId { get; }

        public string CollectionId { get; }

        public Lazy<Uri> Uri { get; }

        private Uri CreateUri()
            => UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId);
    }
}
