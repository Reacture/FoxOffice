namespace FoxOffice.ReadModel.DataAccess
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using AutoFixture.Idioms;
    using FluentAssertions;
    using Microsoft.Azure.Documents;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using static CosmosDb;

    [TestClass]
    public class CosmosDbMovieRepository_specs
    {
        [TestMethod, AutoData]
        public void sut_has_guard_clauses(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(CosmosDbMovieRepository));
        }

        [TestMethod]
        public void sut_implements_IMovieRepository()
        {
            typeof(CosmosDbMovieRepository)
                .Should().Implement<IMovieRepository>();
        }

        [TestMethod, AutoData]
        public async Task CreateMovie_creates_document_correctly(Movie movie)
        {
            // Arrange
            movie.ETag = default;
            IDocumentClient client = DocumentClient;
            var sut = new CosmosDbMovieRepository(client, Collection);

            // Act
            await sut.CreateMovie(movie);

            // Assert
            IQueryable<MovieDocument> query =
                from d in client.CreateDocumentQuery<MovieDocument>()
                where
                    d.Discriminator == nameof(MovieDocument) &&
                    d.Id == movie.Id
                select d;

            MovieDocument actual = await query.SingleOrDefault();

            actual.Should().BeEquivalentTo(new
            {
                movie.Id,
                movie.Title,
                Screenings = from s in movie.Screenings
                             select new
                             {
                                 s.Id,
                                 s.TheaterId,
                                 s.TheaterName,
                                 s.Seats,
                                 s.ScreeningTime,
                                 s.DefaultFee,
                                 s.ChildrenFee,
                                 s.CreatedAt,
                             },
                movie.CreatedAt,
            });
        }

        [TestMethod, AutoData]
        public async Task UpdateMovie_replaces_document_correctly(
            Movie source, IFixture builder)
        {
            // Arrange
            source.ETag = default;

            IDocumentClient client = DocumentClient;
            var sut = new CosmosDbMovieRepository(client, Collection);

            await sut.CreateMovie(source);

            IQueryable<string> query =
                from d in DocumentClient.CreateDocumentQuery<MovieDocument>()
                where d.Id == source.Id
                select d.ETag;

            Movie movie = builder
                .Build<Movie>()
                .With(x => x.Id, source.Id)
                .With(x => x.ETag, await query.SingleOrDefault())
                .Create();

            // Act
            await sut.UpdateMovie(movie);

            // Assert
            Movie actual = await sut.FindMovie(movie.Id);
            actual.Should().BeEquivalentTo(movie, c =>
            {
                c.Excluding(x => x.ETag);
                c.Excluding(x => x.Screenings[0].ETag);
                c.Excluding(x => x.Screenings[1].ETag);
                c.Excluding(x => x.Screenings[2].ETag);
                return c;
            });
        }

        [TestMethod, AutoData]
        public void given_ETag_is_null_then_UpdateMovie_fails(Movie movie)
        {
            movie.ETag = default;
            IDocumentClient client = DocumentClient;
            var sut = new CosmosDbMovieRepository(client, Collection);

            Func<Task> action = () => sut.UpdateMovie(movie);

            action.Should()
                .Throw<ArgumentException>()
                .Where(x => x.ParamName == "movie");
        }

        [TestMethod]
        public void sut_implements_IMovieReader()
        {
            typeof(CosmosDbMovieRepository).Should().Implement<IMovieReader>();
        }

        [TestMethod, AutoData]
        public async Task given_document_found_then_FindMovie_restores_read_model_entity_correctly(
            Movie movie)
        {
            // Arrange
            var sut = new CosmosDbMovieRepository(DocumentClient, Collection);
            await sut.CreateMovie(movie);

            // Act
            Movie actual = await sut.FindMovie(movie.Id);

            // Assert
            IQueryable<MovieDocument> query =
                from d in DocumentClient.CreateDocumentQuery<MovieDocument>()
                where d.Id == movie.Id
                select d;

            MovieDocument document = await query.SingleOrDefault();

            actual.Should().BeEquivalentTo(new
            {
                document.Id,
                document.ETag,
                document.Title,
                Screenings = from s in document.Screenings
                             select new
                             {
                                 s.Id,
                                 ETag = default(string),
                                 s.TheaterId,
                                 s.TheaterName,
                                 s.Seats,
                                 s.ScreeningTime,
                                 s.DefaultFee,
                                 s.ChildrenFee,
                                 s.CreatedAt,
                             },
                document.CreatedAt,
            });
        }

        [TestMethod, AutoData]
        public async Task given_document_not_found_then_FindMovie_returns_null(
            Guid movieId)
        {
            var sut = new CosmosDbMovieRepository(DocumentClient, Collection);
            Movie actual = await sut.FindMovie(movieId);
            actual.Should().BeNull();
        }

        [TestMethod, AutoData]
        public async Task GetAllMovies_restores_all_read_model_entities_correctly(
            MovieDocument[] documents, SomeDocument[] residuals)
        {
            // Arrange
            string collectionId = "GetAllMovies";

            await InitializeCollection(collectionId);

            var sut = new CosmosDbMovieRepository(
                DocumentClient,
                new CollectionReference(DatabaseId, collectionId));

            Task Create(object d) => CreateDocument(collectionId, d);
            await Task.WhenAll(documents.Select(Create));
            await Task.WhenAll(residuals.Select(Create));

            // Act
            ImmutableArray<Movie> actual = await sut.GetAllMovies();

            // Assert
            actual.Should().BeEquivalentTo(
                expectation: documents,
                c => c.Excluding(x => x.ETag).ExcludingMissingMembers());
        }

        public class SomeDocument
        {
            public Guid Id { get; set; }
        }
    }
}
