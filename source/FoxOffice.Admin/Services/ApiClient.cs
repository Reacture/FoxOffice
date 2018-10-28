namespace FoxOffice.Admin.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using FoxOffice.Commands;
    using FoxOffice.ReadModel;
    using Khala.TransientFaultHandling;
    using static System.Net.HttpStatusCode;

    public class ApiClient :
        ISendCreateTheaterCommandService,
        IGetAllTheatersService,
        ISendCreateMovieCommandService,
        IGetAllMoviesService,
        IFindMovieService,
        ISendAddScreeningCommandService,
        IResourceAwaiter
    {
        private readonly HttpClient _client;
        private readonly Uri _endpoint;

        public ApiClient(HttpClient client, Uri endpoint)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
        }

        private Lazy<RetryPolicy> Retry { get; } = new Lazy<RetryPolicy>(() =>
            new RetryPolicy(
                maximumRetryCount: 10,
                new TransientFaultDetectionStrategy(),
                new LinearRetryIntervalStrategy(
                    initialInterval: TimeSpan.FromMilliseconds(10),
                    increment: TimeSpan.FromMilliseconds(100),
                    maximumInterval: TimeSpan.FromMilliseconds(500),
                    immediateFirstRetry: false)));

        public Task<IResult<TheaterLocation>> SendCreateTheaterCommand(
            CreateNewTheater command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            return Run();

            async Task<IResult<TheaterLocation>> Run()
            {
                string path = "api/commands/SendCreateTheaterCommand";

                HttpResponseMessage response = await
                    _client.PostAsJsonAsync(new Uri(_endpoint, path), command);

                switch (response.StatusCode)
                {
                    case BadRequest:
                        return await ReadError<TheaterLocation>(response);
                }

                var location = new TheaterLocation(response.Headers.Location);

                return new Success<TheaterLocation>(location);
            }
        }

        public async Task<IEnumerable<TheaterDto>> GetAllTheaters()
        {
            var requestUri = new Uri(_endpoint, "api/queries/Theaters");
            HttpResponseMessage response = await _client.GetAsync(requestUri);
            HttpContent content = response.Content;
            return await content.ReadAsAsync<IEnumerable<TheaterDto>>();
        }

        public Task<IResult<MovieLocation>> SendCreateMovieCommand(
            CreateNewMovie command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            return Run();

            async Task<IResult<MovieLocation>> Run()
            {
                string path = "api/commands/SendCreateMovieCommand";

                HttpResponseMessage response = await
                    _client.PostAsJsonAsync(new Uri(_endpoint, path), command);

                switch (response.StatusCode)
                {
                    case BadRequest:
                        return await ReadError<MovieLocation>(response);
                }

                var location = new MovieLocation(response.Headers.Location);

                return new Success<MovieLocation>(location);
            }
        }

        public async Task<IEnumerable<MovieDto>> GetAllMovies()
        {
            var requestUri = new Uri(_endpoint, "api/queries/Movies");
            HttpResponseMessage response = await _client.GetAsync(requestUri);
            HttpContent content = response.Content;
            return await content.ReadAsAsync<IEnumerable<MovieDto>>();
        }

        public Task<MovieDto> FindMovie(Guid movieId)
        {
            if (movieId == Guid.Empty)
            {
                string message = "Value cannot be empty.";
                throw new ArgumentException(message, nameof(movieId));
            }

            return FindMovieImpl(movieId);
        }

        private async Task<MovieDto> FindMovieImpl(Guid movieId)
        {
            string relativeUri = $"api/queries/Movies/{movieId}";
            var requestUri = new Uri(_endpoint, relativeUri);
            HttpResponseMessage response = await _client.GetAsync(requestUri);

            switch (response.StatusCode)
            {
                case NotFound:
                    return default;

                default:
                    return await response.Content.ReadAsAsync<MovieDto>();
            }
        }

        public Task<IResult<ScreeningLocation>> SendAddScreeningCommand(
            AddNewScreening command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            return Run();

            async Task<IResult<ScreeningLocation>> Run()
            {
                string path = "api/commands/SendAddScreeningCommand";

                HttpResponseMessage response = await
                    _client.PostAsJsonAsync(new Uri(_endpoint, path), command);

                switch (response.StatusCode)
                {
                    case BadRequest:
                        return await ReadError<ScreeningLocation>(response);
                }

                var location = new ScreeningLocation(response.Headers.Location);

                return new Success<ScreeningLocation>(location);
            }
        }

        private static async Task<IResult<T>> ReadError<T>(
            HttpResponseMessage response)
        {
            Dictionary<string, dynamic> state = await
                response.Content.ReadAsAsync<Dictionary<string, dynamic>>();
            IEnumerable<dynamic> errors = state.First().Value;
            string message = errors.First();
            return new Error<T>(message);
        }

        public Task AwaitResource(Uri location)
        {
            if (location == null)
            {
                throw new ArgumentNullException(nameof(location));
            }

            var uri = new Uri(_endpoint, location);

            return Retry.Value.Run(async () =>
            {
                HttpResponseMessage response = await _client.GetAsync(uri);
                response.EnsureSuccessStatusCode();
            });
        }
    }
}
