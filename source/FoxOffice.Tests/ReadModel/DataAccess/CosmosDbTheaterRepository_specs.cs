namespace FoxOffice.ReadModel.DataAccess
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture.Idioms;
    using FluentAssertions;
    using Microsoft.Azure.Documents;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using static CosmosDb;

    [TestClass]
    public class CosmosDbTheaterRepository_specs
    {
        [TestMethod, AutoData]
        public void sut_has_guard_clauses(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(CosmosDbTheaterRepository));
        }

        [TestMethod]
        public void sut_implements_ITheaterRepository()
        {
            typeof(CosmosDbTheaterRepository)
                .Should().Implement<ITheaterRepository>();
        }

        [TestMethod, AutoData]
        public async Task CreateTheater_creates_document_correctly(
            Theater theater)
        {
            // Arrange
            theater.ETag = default;
            IDocumentClient client = DocumentClient;
            var sut = new CosmosDbTheaterRepository(client, Collection);

            // Act
            await sut.CreateTheater(theater);

            // Assert
            IQueryable<TheaterDocument> query =
                from d in client.CreateDocumentQuery<TheaterDocument>()
                where
                    d.Discriminator == nameof(TheaterDocument) &&
                    d.Id == theater.Id
                select d;

            TheaterDocument actual = await query.SingleOrDefault();

            actual.Should().BeEquivalentTo(new
            {
                theater.Id,
                theater.Name,
                theater.SeatRowCount,
                theater.SeatColumnCount,
                theater.CreatedAt,
            });
        }

        [TestMethod]
        public void sut_implements_ITheaterReader()
        {
            typeof(CosmosDbTheaterRepository)
                .Should().Implement<ITheaterReader>();
        }

        [TestMethod, AutoData]
        public async Task FindTheater_restores_read_model_entity_correctly(
            Theater theater)
        {
            // Arrange
            theater.ETag = default;
            IDocumentClient client = DocumentClient;
            var sut = new CosmosDbTheaterRepository(client, Collection);
            await sut.CreateTheater(theater);

            // Act
            Theater actual = await sut.FindTheater(theater.Id);

            // Assert
            IQueryable<TheaterDocument> query =
                from d in client.CreateDocumentQuery<TheaterDocument>()
                where
                    d.Discriminator == nameof(TheaterDocument) &&
                    d.Id == theater.Id
                select d;

            TheaterDocument document = await query.SingleOrDefault();

            actual.Should().BeEquivalentTo(new
            {
                document.Id,
                document.ETag,
                document.Name,
                document.SeatRowCount,
                document.SeatColumnCount,
                document.CreatedAt,
            });
        }

        [TestMethod, AutoData]
        public async Task given_document_not_found_then_FindTheater_returns_null(
            Guid theaterId)
        {
            var sut = new CosmosDbTheaterRepository(DocumentClient, Collection);
            Theater actual = await sut.FindTheater(theaterId);
            actual.Should().BeNull();
        }

        [TestMethod, AutoData]
        public async Task GetAllTheaters_restores_all_read_model_entities_correctly(
            Theater[] theaters, SomeDocument[] residuals)
        {
            // Arrange
            string collectionId = "GetAllTheaters";

            await InitializeCollection(collectionId);

            var sut = new CosmosDbTheaterRepository(
                DocumentClient,
                new CollectionReference(DatabaseId, collectionId));

            await Task.WhenAll(theaters.Select(sut.CreateTheater));

            foreach (SomeDocument document in residuals)
            {
                await CreateDocument(collectionId, document);
            }

            // Act
            ImmutableArray<Theater> actual = await sut.GetAllTheaters();

            // Assert
            actual.Should().BeEquivalentTo(
                expectation: theaters,
                config: c => c.Excluding(x => x.ETag));
        }

        public class SomeDocument
        {
            public Guid Id { get; set; }
        }
    }
}
