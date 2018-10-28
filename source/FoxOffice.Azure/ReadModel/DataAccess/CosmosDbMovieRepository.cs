namespace FoxOffice.ReadModel.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Azure.Documents.Linq;

    public class CosmosDbMovieRepository : IMovieRepository, IMovieReader
    {
        private static readonly IMapper _mapper;

        private readonly IDocumentClient _client;
        private readonly CollectionReference _collection;

        static CosmosDbMovieRepository()
        {
            _mapper = new MapperConfiguration(expr =>
            {
                expr.CreateMap<Screening, ScreeningEntity>();
                expr.CreateMap<ScreeningEntity, Screening>();
                expr.CreateMap<Movie, MovieDocument>();
                expr.CreateMap<MovieDocument, Movie>();
            }).CreateMapper();
        }

        public CosmosDbMovieRepository(
            IDocumentClient client, CollectionReference collection)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _collection = collection ?? throw new ArgumentNullException(nameof(collection));
        }

        public Task CreateMovie(Movie movie)
        {
            if (movie == null)
            {
                throw new ArgumentNullException(nameof(movie));
            }

            MovieDocument document = Translate(movie);
            return _client.CreateDocumentAsync(_collection.Uri.Value, document);
        }

        public Task UpdateMovie(Movie movie)
        {
            if (movie == null)
            {
                throw new ArgumentNullException(nameof(movie));
            }

            if (movie.ETag == null)
            {
                string message = $"ETag should not be null.";
                throw new ArgumentException(message, nameof(movie));
            }

            string databaseId = _collection.DatabaseId;
            string collectionId = _collection.CollectionId;

            Uri uri = UriFactory.CreateDocumentUri(
                databaseId, collectionId, $"{movie.Id}");

            var options = new RequestOptions
            {
                AccessCondition = new AccessCondition
                {
                    Type = AccessConditionType.IfMatch,
                    Condition = movie.ETag,
                },
            };

            return _client.ReplaceDocumentAsync(uri, Translate(movie), options);
        }

        public Task<Movie> FindMovie(Guid movieId)
        {
            if (movieId == Guid.Empty)
            {
                string message = "Value cannot be empty.";
                throw new ArgumentException(message, nameof(movieId));
            }

            return FindMovieImpl(movieId);
        }

        private async Task<Movie> FindMovieImpl(Guid movieId)
        {
            MovieDocument document = await _client
                .CreateDocumentQuery<MovieDocument>(_collection.Uri.Value)
                .Where(d => d.Id == movieId)
                .AsDocumentQuery()
                .SingleOrDefault();

            return Translate(document);
        }

        public async Task<ImmutableArray<Movie>> GetAllMovies()
        {
            IEnumerable<MovieDocument> documents = await _client
                .CreateDocumentQuery<MovieDocument>(_collection.Uri.Value)
                .Where(d => d.Discriminator == nameof(MovieDocument))
                .AsDocumentQuery()
                .Execute();

            return documents.Select(Translate).ToImmutableArray();
        }

        private MovieDocument Translate(Movie movie)
            => _mapper.Map<MovieDocument>(movie);

        private static Movie Translate(MovieDocument document)
            => _mapper.Map<Movie>(document);
    }
}
