namespace FoxOffice.ReadModel.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Linq;

    public class CosmosDbTheaterRepository : ITheaterRepository, ITheaterReader
    {
        private readonly IDocumentClient _client;
        private readonly CollectionReference _collection;

        public CosmosDbTheaterRepository(
            IDocumentClient client, CollectionReference collection)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _collection = collection ?? throw new ArgumentNullException(nameof(collection));
        }

        public Task CreateTheater(Theater theater)
        {
            if (theater == null)
            {
                throw new ArgumentNullException(nameof(theater));
            }

            TheaterDocument document = Translate(theater);
            return _client.CreateDocumentAsync(_collection.Uri.Value, document);
        }

        public Task<Theater> FindTheater(Guid theaterId)
        {
            if (theaterId == Guid.Empty)
            {
                string message = "Value cannot be empty.";
                throw new ArgumentException(message, nameof(theaterId));
            }

            return FindTheaterImpl(theaterId);
        }

        private async Task<Theater> FindTheaterImpl(Guid theaterId)
        {
            TheaterDocument document = await _client
                .CreateDocumentQuery<TheaterDocument>(_collection.Uri.Value)
                .Where(d => d.Id == theaterId)
                .AsDocumentQuery()
                .SingleOrDefault();

            return Translate(document);
        }

        public async Task<ImmutableArray<Theater>> GetAllTheaters()
        {
            IEnumerable<TheaterDocument> documents = await _client
                .CreateDocumentQuery<TheaterDocument>(_collection.Uri.Value)
                .Where(d => d.Discriminator == nameof(TheaterDocument))
                .AsDocumentQuery()
                .Execute();

            return documents.Select(Translate).ToImmutableArray();
        }

        private static TheaterDocument Translate(Theater theater)
        {
            return new TheaterDocument
            {
                Id = theater.Id,
                Name = theater.Name,
                SeatRowCount = theater.SeatRowCount,
                SeatColumnCount = theater.SeatColumnCount,
                CreatedAt = theater.CreatedAt,
            };
        }

        private static Theater Translate(TheaterDocument document)
        {
            return document == null ? default : new Theater
            {
                Id = document.Id,
                ETag = document.ETag,
                Name = document.Name,
                SeatRowCount = document.SeatRowCount,
                SeatColumnCount = document.SeatColumnCount,
                CreatedAt = document.CreatedAt,
            };
        }
    }
}
