namespace FoxOffice.Admin.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Threading.Tasks;
    using AutoFixture.Idioms;
    using FluentAssertions;
    using FoxOffice.Commands;
    using FoxOffice.ReadModel;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ApiClient_specs
    {
        [TestMethod, AutoData]
        public void sut_has_guard_clauses(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(ApiClient));
        }

        [TestMethod]
        public void sut_implements_ISendCreateTheaterCommandService()
        {
            typeof(ApiClient).Should()
                .Implement<ISendCreateTheaterCommandService>();
        }

        [TestMethod, AutoData]
        public async Task given_valid_command_then_SendCreateTheaterCommand_returns_location_result(
            HttpMessageHandlerDouble handlerStub,
            Uri endpoint,
            CreateNewTheater command,
            TheaterLocation location)
        {
            // Arrange
            bool predicate(HttpRequestMessage req)
            {
                string path = "api/commands/SendCreateTheaterCommand";
                return req.Method == HttpMethod.Post
                    && req.RequestUri == new Uri(endpoint, path)
                    && req.Content is ObjectContent<CreateNewTheater> content
                    && content.Value == command
                    && content.Formatter is JsonMediaTypeFormatter;
            }

            var response = new HttpResponseMessage(HttpStatusCode.Accepted);
            response.Headers.Location = location.Uri;

            handlerStub.AddAnswer(predicate, answer: response);

            var sut = new ApiClient(new HttpClient(handlerStub), endpoint);

            // Act
            IResult<TheaterLocation> actual = await
                sut.SendCreateTheaterCommand(command);

            // Assert
            actual.Should().BeOfType<Success<TheaterLocation>>();
            actual.Should().BeEquivalentTo(new { Value = location });
        }

        [TestMethod, AutoData]
        public async Task given_bad_command_then_SendCreateTheaterCommand_returns_error_result(
            HttpMessageHandlerDouble handlerStub,
            Uri endpoint,
            CreateNewTheater command,
            ModelStateDictionary state,
            string key,
            string errorMessage)
        {
            // Arrange
            bool predicate(HttpRequestMessage req)
            {
                string path = "api/commands/SendCreateTheaterCommand";
                return req.Method == HttpMethod.Post
                    && req.RequestUri == new Uri(endpoint, path)
                    && req.Content is ObjectContent<CreateNewTheater> content
                    && content.Value == command
                    && content.Formatter is JsonMediaTypeFormatter;
            }

            var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            state.AddModelError(key, errorMessage);
            response.Content = CreateContent(new SerializableError(state));

            handlerStub.AddAnswer(predicate, answer: response);

            var sut = new ApiClient(new HttpClient(handlerStub), endpoint);

            // Act
            IResult<TheaterLocation> actual = await
                sut.SendCreateTheaterCommand(command);

            // Assert
            actual.Should().BeOfType<Error<TheaterLocation>>();
            actual.Should().BeEquivalentTo(new { Message = errorMessage });
        }

        [TestMethod]
        public void sut_implements_IGetAllTheatersService()
        {
            typeof(ApiClient).Should().Implement<IGetAllTheatersService>();
        }

        [TestMethod, AutoData]
        public async Task GetAllTheaters_returns_response_content_correctly(
            HttpMessageHandlerDouble handlerStub,
            Uri endpoint,
            ImmutableArray<TheaterDto> content)
        {
            // Arrange
            bool predicate(HttpRequestMessage request)
            {
                string path = "api/queries/Theaters";
                return request.Method == HttpMethod.Get
                    && request.RequestUri == new Uri(endpoint, path)
                    && request.Content == default;
            }

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = CreateContent(content),
            };

            handlerStub.AddAnswer(predicate, answer: response);

            var sut = new ApiClient(new HttpClient(handlerStub), endpoint);

            // Act
            IEnumerable<TheaterDto> actual = await sut.GetAllTheaters();

            // Assert
            actual.Should().BeEquivalentTo(content);
        }

        [TestMethod]
        public void sut_implements_ISendCreateMovieCommandService()
        {
            typeof(ApiClient).Should()
                .Implement<ISendCreateMovieCommandService>();
        }

        [TestMethod, AutoData]
        public async Task given_valid_command_then_SendCreateMovieCommand_returns_location_result(
            HttpMessageHandlerDouble handlerStub,
            Uri endpoint,
            CreateNewMovie command,
            MovieLocation location)
        {
            // Arrange
            bool predicate(HttpRequestMessage req)
            {
                string path = "api/commands/SendCreateMovieCommand";
                return req.Method == HttpMethod.Post
                    && req.RequestUri == new Uri(endpoint, path)
                    && req.Content is ObjectContent<CreateNewMovie> content
                    && content.Value == command
                    && content.Formatter is JsonMediaTypeFormatter;
            }

            var response = new HttpResponseMessage(HttpStatusCode.Accepted);
            response.Headers.Location = location.Uri;

            handlerStub.AddAnswer(predicate, answer: response);

            var sut = new ApiClient(new HttpClient(handlerStub), endpoint);

            // Act
            IResult<MovieLocation> actual = await
                sut.SendCreateMovieCommand(command);

            // Assert
            actual.Should().BeOfType<Success<MovieLocation>>();
            actual.Should().BeEquivalentTo(new { Value = location });
        }

        [TestMethod, AutoData]
        public async Task given_bad_command_then_SendCreateMovieCommand_returns_error_result(
            HttpMessageHandlerDouble handlerStub,
            Uri endpoint,
            CreateNewMovie command,
            ModelStateDictionary state,
            string key,
            string errorMessage)
        {
            // Arrange
            bool predicate(HttpRequestMessage req)
            {
                string path = "api/commands/SendCreateMovieCommand";
                return req.Method == HttpMethod.Post
                    && req.RequestUri == new Uri(endpoint, path)
                    && req.Content is ObjectContent<CreateNewMovie> content
                    && content.Value == command
                    && content.Formatter is JsonMediaTypeFormatter;
            }

            var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            state.AddModelError(key, errorMessage);
            response.Content = CreateContent(new SerializableError(state));

            handlerStub.AddAnswer(predicate, answer: response);

            var sut = new ApiClient(new HttpClient(handlerStub), endpoint);

            // Act
            IResult<MovieLocation> actual = await
                sut.SendCreateMovieCommand(command);

            // Assert
            actual.Should().BeOfType<Error<MovieLocation>>();
            actual.Should().BeEquivalentTo(new { Message = errorMessage });
        }

        [TestMethod]
        public void sut_implements_ISendAddScreeningCommandService()
        {
            typeof(ApiClient).Should()
                .Implement<ISendAddScreeningCommandService>();
        }

        [TestMethod, AutoData]
        public async Task given_valid_command_then_SendAddScreeningCommand_returns_location_result(
            HttpMessageHandlerDouble handlerStub,
            Uri endpoint,
            AddNewScreening command,
            ScreeningLocation location)
        {
            // Arrange
            bool predicate(HttpRequestMessage req)
            {
                string path = "api/commands/SendAddScreeningCommand";
                return req.Method == HttpMethod.Post
                    && req.RequestUri == new Uri(endpoint, path)
                    && req.Content is ObjectContent<AddNewScreening> content
                    && content.Value == command
                    && content.Formatter is JsonMediaTypeFormatter;
            }

            var response = new HttpResponseMessage(HttpStatusCode.Accepted);
            response.Headers.Location = location.Uri;

            handlerStub.AddAnswer(predicate, answer: response);

            var sut = new ApiClient(new HttpClient(handlerStub), endpoint);

            // Act
            IResult<ScreeningLocation> actual =
                await sut.SendAddScreeningCommand(command);

            // Assert
            actual.Should().BeOfType<Success<ScreeningLocation>>();
            actual.Should().BeEquivalentTo(new { Value = location });
        }

        [TestMethod, AutoData]
        public async Task given_bad_command_then_SendAddScreeningCommand_returns_error_result(
            HttpMessageHandlerDouble handlerStub,
            Uri endpoint,
            AddNewScreening command,
            ModelStateDictionary state,
            string key,
            string errorMessage)
        {
            // Arrange
            bool predicate(HttpRequestMessage req)
            {
                string path = "api/commands/SendAddScreeningCommand";
                return req.Method == HttpMethod.Post
                    && req.RequestUri == new Uri(endpoint, path)
                    && req.Content is ObjectContent<AddNewScreening> content
                    && content.Value == command
                    && content.Formatter is JsonMediaTypeFormatter;
            }

            var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            state.AddModelError(key, errorMessage);
            response.Content = CreateContent(new SerializableError(state));

            handlerStub.AddAnswer(predicate, answer: response);

            var sut = new ApiClient(new HttpClient(handlerStub), endpoint);

            // Act
            IResult<ScreeningLocation> actual =
                await sut.SendAddScreeningCommand(command);

            // Assert
            actual.Should().BeOfType<Error<ScreeningLocation>>();
            actual.Should().BeEquivalentTo(new { Message = errorMessage });
        }

        [TestMethod]
        public void sut_implements_IGetAllMoviesService()
        {
            typeof(ApiClient).Should().Implement<IGetAllMoviesService>();
        }

        [TestMethod, AutoData]
        public async Task GetAllMovies_returns_response_content_correctly(
            HttpMessageHandlerDouble handlerStub,
            Uri endpoint,
            ImmutableArray<MovieDto> content)
        {
            // Arrange
            bool predicate(HttpRequestMessage request)
            {
                string path = "api/queries/Movies";
                return request.Method == HttpMethod.Get
                    && request.RequestUri == new Uri(endpoint, path)
                    && request.Content == default;
            }

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = CreateContent(content),
            };

            handlerStub.AddAnswer(predicate, answer: response);

            var sut = new ApiClient(new HttpClient(handlerStub), endpoint);

            // Act
            IEnumerable<MovieDto> actual = await sut.GetAllMovies();

            // Assert
            actual.Should().BeEquivalentTo(content);
        }

        [TestMethod]
        public void sut_implements_IFindMovieService()
        {
            typeof(ApiClient).Should().Implement<IFindMovieService>();
        }

        [TestMethod, AutoData]
        public async Task given_success_response_then_FindMovie_returns_response_content_correctly(
            HttpMessageHandlerDouble handlerStub,
            Uri endpoint,
            MovieDto content)
        {
            // Arrange
            Guid movieId = content.Id;

            bool predicate(HttpRequestMessage request)
            {
                string path = $"api/queries/Movies/{movieId}";
                return request.Method == HttpMethod.Get
                    && request.RequestUri == new Uri(endpoint, path)
                    && request.Content == default;
            }

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = CreateContent(content),
            };

            handlerStub.AddAnswer(predicate, answer: response);

            var sut = new ApiClient(new HttpClient(handlerStub), endpoint);

            // Act
            MovieDto actual = await sut.FindMovie(movieId);

            // Assert
            actual.Should().BeEquivalentTo(content);
        }

        [TestMethod, AutoData]
        public async Task given_not_found_response_then_FindMovie_returns_null(
            HttpMessageHandlerDouble handlerStub, Uri endpoint, Guid movieId)
        {
            // Arrange
            bool predicate(HttpRequestMessage request)
            {
                string path = $"api/queries/Movies/{movieId}";
                return request.Method == HttpMethod.Get
                    && request.RequestUri == new Uri(endpoint, path)
                    && request.Content == default;
            }

            handlerStub.AddAnswer(predicate, HttpStatusCode.NotFound);

            var sut = new ApiClient(new HttpClient(handlerStub), endpoint);

            // Act
            MovieDto actual = await sut.FindMovie(movieId);

            // Assert
            actual.Should().BeNull();
        }

        [TestMethod]
        public void sut_implements_IResourceAwaiter()
        {
            typeof(ApiClient).Should().Implement<IResourceAwaiter>();
        }

        [TestMethod, AutoData]
        public async Task AwaitResource_awaits_until_response_is_success(
            HttpMessageHandlerDouble stub,
            Uri endpoint,
            string path,
            [Range(1, 2000)] int delay)
        {
            // Arrange
            bool complete = false;

            bool predicate(HttpRequestMessage request)
                => request.Method == HttpMethod.Get
                && request.RequestUri == new Uri(endpoint, path)
                && complete;

            stub.AddAnswer(predicate, HttpStatusCode.OK);
            stub.AddAnswer(_ => true, HttpStatusCode.NotFound);

            var sut = new ApiClient(new HttpClient(stub), endpoint);

            // Act
            Task.Delay(millisecondsDelay: delay)
                .ContinueWith(_ => complete = true)
                .GetAwaiter();

            var stopwatch = Stopwatch.StartNew();

            await sut.AwaitResource(new Uri(path, UriKind.Relative));

            stopwatch.Stop();

            // Assert
            stopwatch.ElapsedMilliseconds.Should().BeCloseTo(delay, delta: 550);
        }

        private static ObjectContent<T> CreateContent<T>(T content) =>
            new ObjectContent<T>(content, new JsonMediaTypeFormatter());
    }
}
