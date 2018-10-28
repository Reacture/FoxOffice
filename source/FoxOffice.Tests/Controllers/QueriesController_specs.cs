namespace FoxOffice.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using FoxOffice.ReadModel;
    using FoxOffice.ReadModel.DataAccess;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class QueriesController_specs
    {
        [TestMethod]
        public void sut_inherits_ControllerBase()
        {
            typeof(QueriesController).Should().BeDerivedFrom<ControllerBase>();
        }

        [TestMethod]
        public void sut_is_decorated_with_ApiController_attribute()
        {
            typeof(QueriesController).Should()
                .BeDecoratedWith<ApiControllerAttribute>();
        }

        [TestMethod, AutoData]
        public async Task GetAllTheaters_returns_OkObjectResult(
            Theater[] theaters,
            InMemoryTheaterRepository readerStub,
            [NoAutoProperties] QueriesController sut)
        {
            theaters.ForEach(t => readerStub.Data[t.Id] = t);
            var facade = new TheaterReadModelFacade(readerStub);

            IActionResult actual = await sut.GetAllTheaters(facade);

            actual.Should().BeOfType<OkObjectResult>();
        }

        [TestMethod, AutoData]
        public async Task GetAllTheaters_returns_transfer_objects_as_content(
            Theater[] theaters,
            InMemoryTheaterRepository readerStub,
            [NoAutoProperties] QueriesController sut)
        {
            theaters.ForEach(t => readerStub.Data[t.Id] = t);
            var facade = new TheaterReadModelFacade(readerStub);

            var actual = (OkObjectResult)await sut.GetAllTheaters(facade);

            actual.Value.Should().BeEquivalentTo(await facade.GetAllTheaters());
        }

        [TestMethod, AutoData]
        public async Task given_theater_found_then_FindTheater_returns_OkObjectResult(
            Theater theater,
            InMemoryTheaterRepository readerStub,
            [NoAutoProperties] QueriesController sut)
        {
            readerStub.Data[theater.Id] = theater;
            var facade = new TheaterReadModelFacade(readerStub);

            IActionResult actual = await sut.FindTheater(theater.Id, facade);

            actual.Should().BeOfType<OkObjectResult>();
        }

        [TestMethod, AutoData]
        public async Task given_theater_found_then_FindTheater_returns_transfer_object_as_content(
            Theater theater,
            InMemoryTheaterRepository readerStub,
            [NoAutoProperties] QueriesController sut)
        {
            readerStub.Data[theater.Id] = theater;
            var facade = new TheaterReadModelFacade(readerStub);

            IActionResult result = await sut.FindTheater(theater.Id, facade);

            object actual = result.As<OkObjectResult>().Value;
            actual.Should().BeOfType<TheaterDto>();
            actual.Should().BeEquivalentTo(new
            {
                theater.Id,
                theater.Name,
                theater.SeatRowCount,
                theater.SeatColumnCount,
            });
        }

        [TestMethod, AutoData]
        public async Task given_theater_not_found_then_FindTheater_returns_NotFoundResult(
            Guid theaterId,
            InMemoryTheaterRepository readerStub,
            [NoAutoProperties] QueriesController sut)
        {
            var facade = new TheaterReadModelFacade(readerStub);
            IActionResult actual = await sut.FindTheater(theaterId, facade);
            actual.Should().BeOfType<NotFoundResult>();
        }

        [TestMethod, AutoData]
        public async Task GetAllMovies_returns_OkObjectResult(
            Movie[] movies,
            InMemoryMovieRepository readerStub,
            [NoAutoProperties] QueriesController sut)
        {
            movies.ForEach(t => readerStub.Data[t.Id] = t);
            var facade = new MovieReadModelFacade(readerStub);

            IActionResult actual = await sut.GetAllMovies(facade);

            actual.Should().BeOfType<OkObjectResult>();
        }

        [TestMethod, AutoData]
        public async Task GetAllMovies_returns_transfer_objects_as_content(
            Movie[] movies,
            InMemoryMovieRepository readerStub,
            [NoAutoProperties] QueriesController sut)
        {
            movies.ForEach(t => readerStub.Data[t.Id] = t);
            var facade = new MovieReadModelFacade(readerStub);

            var actual = (OkObjectResult)await sut.GetAllMovies(facade);

            actual.Value.Should().BeEquivalentTo(await facade.GetAllMovies());
        }

        [TestMethod, AutoData]
        public async Task given_movie_found_then_FindMovie_returns_OkObjectResult(
            Movie movie,
            InMemoryMovieRepository readerStub,
            [NoAutoProperties] QueriesController sut)
        {
            readerStub.Data[movie.Id] = movie;
            var facade = new MovieReadModelFacade(readerStub);

            IActionResult actual = await sut.FindMovie(movie.Id, facade);

            actual.Should().BeOfType<OkObjectResult>();
        }

        [TestMethod, AutoData]
        public async Task given_movie_found_then_FindMovie_returns_transfer_object_as_content(
            Movie movie,
            InMemoryMovieRepository readerStub,
            [NoAutoProperties] QueriesController sut)
        {
            readerStub.Data[movie.Id] = movie;
            var facade = new MovieReadModelFacade(readerStub);

            IActionResult result = await sut.FindMovie(movie.Id, facade);

            object actual = result.As<OkObjectResult>().Value;
            actual.Should().BeOfType<MovieDto>();
            actual.Should().BeEquivalentTo(new
            {
                movie.Id,
                movie.Title,
                movie.CreatedAt,
            });
        }

        [TestMethod, AutoData]
        public async Task given_movie_not_found_then_FindMovie_returns_NotFoundResult(
            Guid movieId,
            InMemoryMovieRepository readerStub,
            [NoAutoProperties] QueriesController sut)
        {
            var facade = new MovieReadModelFacade(readerStub);
            IActionResult actual = await sut.FindMovie(movieId, facade);
            actual.Should().BeOfType<NotFoundResult>();
        }

        [TestMethod, AutoData]
        public async Task given_screening_found_then_FindScreening_returns_OkObjectResult(
            Movie movie,
            InMemoryMovieRepository readerStub,
            [NoAutoProperties] QueriesController sut)
        {
            readerStub.Data[movie.Id] = movie;
            var facade = new MovieReadModelFacade(readerStub);
            Screening screening = movie.Screenings.Shuffle().First();

            IActionResult actual = await
                sut.FindScreening(movie.Id, screening.Id, facade);

            actual.Should().BeOfType<OkObjectResult>();
        }

        [TestMethod, AutoData]
        public async Task given_screening_found_then_FindScreening_returns_transfer_object_as_content(
            Movie movie,
            InMemoryMovieRepository readerStub,
            [NoAutoProperties] QueriesController sut)
        {
            // Arrange
            readerStub.Data[movie.Id] = movie;
            var facade = new MovieReadModelFacade(readerStub);
            Screening screening = movie.Screenings.Shuffle().First();

            // Act
            dynamic result = await
                sut.FindScreening(movie.Id, screening.Id, facade);

            // Assert
            object actual = result.Value;
            actual.Should().BeOfType<ScreeningDto>();
            actual.Should().BeEquivalentTo(
                expectation: screening,
                config: c => c.ExcludingMissingMembers());
        }

        [TestMethod, AutoData]
        public async Task given_movie_not_found_then_FindScreening_returns_NotFoundResult(
            Guid movieId,
            Guid screeningId,
            IMovieReader readerStub,
            [NoAutoProperties] QueriesController sut)
        {
            var facade = new MovieReadModelFacade(readerStub);

            IActionResult actual = await
                sut.FindScreening(movieId, screeningId, facade);

            actual.Should().BeOfType<NotFoundResult>();
        }

        [TestMethod, AutoData]
        public async Task given_screening_not_found_then_FindScreening_returns_NotFoundResult(
            Movie movie,
            Guid screeningId,
            InMemoryMovieRepository readerStub,
            [NoAutoProperties] QueriesController sut)
        {
            readerStub.Data[movie.Id] = movie;
            var facade = new MovieReadModelFacade(readerStub);

            IActionResult actual = await
                sut.FindScreening(movie.Id, screeningId, facade);

            actual.Should().BeOfType<NotFoundResult>();
        }
    }
}
