namespace FoxOffice.Admin.Controllers
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using FoxOffice.Admin.Models;
    using FoxOffice.Admin.Services;
    using FoxOffice.Commands;
    using FoxOffice.ReadModel;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class MoviesController_specs
    {
        [TestMethod, AutoData]
        public async Task Index_returns_ViewResult(
            MovieDto[] movies,
            Mock<IGetAllMoviesService> stub,
            [NoAutoProperties] MoviesController sut)
        {
            stub.Setup(x => x.GetAllMovies()).ReturnsAsync(movies);
            ActionResult actual = await sut.Index(service: stub.Object);
            actual.Should().BeOfType<ViewResult>();
        }

        [TestMethod, AutoData]
        public async Task Index_sets_model_correctly(
            MovieDto[] movies,
            Mock<IGetAllMoviesService> stub,
            [NoAutoProperties] MoviesController sut)
        {
            stub.Setup(x => x.GetAllMovies()).ReturnsAsync(movies);

            var result = (ViewResult)await sut.Index(service: stub.Object);

            object actual = result.Model;
            actual.Should()
                .BeOfType<ImmutableArray<MovieViewModel>>().And
                .BeEquivalentTo(movies, c => c.ExcludingMissingMembers());
        }

        [TestMethod, AutoData]
        public void Create_returns_ViewResult(
            [NoAutoProperties] MoviesController sut)
        {
            ActionResult actual = sut.Create();
            actual.Should().BeOfType<ViewResult>();
        }

        [TestMethod, AutoData]
        public void Create_sets_model_correctly(
            [NoAutoProperties] MoviesController sut)
        {
            var result = (ViewResult)sut.Create();

            object actual = result.Model;
            actual.Should().BeOfType<CreateMovieViewModel>();
            actual.Should().BeEquivalentTo(new { Title = string.Empty });
        }

        [TestMethod, AutoData]
        public async Task given_service_returns_error_then_Create_returns_ViewResult(
            CreateMovieViewModel model,
            ISendCreateMovieCommandService serviceStub,
            IResourceAwaiter awaiterDummy,
            Error<MovieLocation> error,
            [NoAutoProperties] MoviesController sut)
        {
            // Arrange
            Mock.Get(serviceStub)
                .Setup(
                    x =>
                    x.SendCreateMovieCommand(It.IsAny<CreateNewMovie>()))
                .ReturnsAsync(error);

            // Act
            ActionResult actual = await
                sut.Create(model, serviceStub, awaiterDummy);

            // Assert
            actual.Should().BeOfType<ViewResult>();
            actual.As<ViewResult>().Model.Should().BeSameAs(model);
        }

        [TestMethod, AutoData]
        public async Task given_service_returns_error_then_Create_adds_model_error(
            CreateMovieViewModel model,
            ISendCreateMovieCommandService serviceStub,
            IResourceAwaiter awaiterDummy,
            Error<MovieLocation> error,
            [NoAutoProperties] MoviesController sut)
        {
            // Arrange
            Mock.Get(serviceStub)
                .Setup(
                    x =>
                    x.SendCreateMovieCommand(It.IsAny<CreateNewMovie>()))
                .ReturnsAsync(error);

            // Act
            await sut.Create(model, serviceStub, awaiterDummy);

            // Assert
            ModelStateEntry state = sut.ModelState[string.Empty];
            state.Errors.Should().Contain(e => e.ErrorMessage == error.Message);
        }

        [TestMethod, AutoData]
        public async Task given_service_returns_success_then_Create_returns_RedirectToActionResult(
            CreateMovieViewModel model,
            ISendCreateMovieCommandService serviceStub,
            IResourceAwaiter awaiterDummy,
            Success<MovieLocation> success,
            [NoAutoProperties] MoviesController sut)
        {
            // Arrange
            Mock.Get(serviceStub)
                .Setup(
                    x =>
                    x.SendCreateMovieCommand(It.Is<CreateNewMovie>(
                        p =>
                        p.Title == model.Title)))
                .ReturnsAsync(success);

            // Act
            ActionResult actual = await
                sut.Create(model, serviceStub, awaiterDummy);

            // Assert
            actual.Should().BeOfType<RedirectToActionResult>();
            actual.As<RedirectToActionResult>().ActionName.Should().Be("Index");
        }

        [TestMethod, AutoData]
        public async Task given_services_returns_success_then_Create_awaits_resource_creation(
            CreateMovieViewModel model,
            ISendCreateMovieCommandService serviceStub,
            IResourceAwaiter awaiterSpy,
            Success<MovieLocation> success,
            [NoAutoProperties] MoviesController sut)
        {
            // Arrange
            Mock.Get(serviceStub)
                .Setup(
                    x =>
                    x.SendCreateMovieCommand(It.Is<CreateNewMovie>(
                        p =>
                        p.Title == model.Title)))
                .ReturnsAsync(success);

            // Act
            ActionResult actual = await
                sut.Create(model, serviceStub, awaiterSpy);

            // Assert
            Uri location = success.Value.Uri;
            Mock.Get(awaiterSpy).Verify(x => x.AwaitResource(location));
        }

        [TestMethod, AutoData]
        public async Task Screening_returns_ViewResult(
            MovieDto movie,
            IFindMovieService serviceStub,
            [NoAutoProperties] MoviesController sut)
        {
            Mock.Get(serviceStub)
                .Setup(x => x.FindMovie(movie.Id))
                .ReturnsAsync(movie);

            ActionResult actual = await sut.Screenings(serviceStub, movie.Id);

            actual.Should().BeOfType<ViewResult>();
        }

        [TestMethod, AutoData]
        public async Task Screening_sets_model_correctly(
            MovieDto movie,
            IFindMovieService serviceStub,
            [NoAutoProperties] MoviesController sut)
        {
            // Arrange
            Mock.Get(serviceStub)
                .Setup(x => x.FindMovie(movie.Id))
                .ReturnsAsync(movie);

            // Act
            dynamic result = await sut.Screenings(serviceStub, movie.Id);

            // Assert
            object actual = result.Model;
            actual.Should().BeOfType<ScreeningsViewModel>();
            actual.Should().BeEquivalentTo(new
            {
                MovieId = movie.Id,
                MovieTitle = movie.Title,
                Screenings = from s in movie.Screenings
                             select ScreeningViewModel.Translate(s),
            });
        }

        [TestMethod, AutoData]
        public async Task AddScreening_returns_ViewResult(
            TheaterDto[] theaters,
            IGetAllTheatersService serviceStub,
            Guid movieId,
            [NoAutoProperties] MoviesController sut)
        {
            Mock.Get(serviceStub)
                .Setup(x => x.GetAllTheaters())
                .ReturnsAsync(theaters);

            ActionResult actual = await sut.AddScreening(serviceStub, movieId);

            actual.Should().BeAssignableTo<ViewResult>();
        }

        [TestMethod, AutoData]
        public async Task AddScreening_sets_model_correctly(
            TheaterDto[] theaters,
            IGetAllTheatersService serviceStub,
            Guid movieId,
            [NoAutoProperties] MoviesController sut)
        {
            // Arrange
            Mock.Get(serviceStub)
                .Setup(x => x.GetAllTheaters())
                .ReturnsAsync(theaters);

            // Act
            dynamic result = await sut.AddScreening(serviceStub, movieId);

            // Assert
            object actual = result.Model;
            actual.Should().BeOfType<AddScreeningViewModel>();
            actual.Should().BeEquivalentTo(new
            {
                MovieId = movieId,
                TheaterId = default(Guid),
                DefaultFee = default(decimal),
                ChildrenFee = default(decimal),
            });
            DateTime screeningTime = ((dynamic)actual).ScreeningTime;
            screeningTime.Should().BeCloseTo(DateTime.Today.AddDays(2));
            actual.Should().BeEquivalentTo(new
            {
                Theaters = from t in theaters
                           select new SelectListItem
                           {
                               Text = t.Name,
                               Value = $"{t.Id}",
                           },
            });
        }

        [TestMethod, AutoData]
        public async Task AddScreening_returns_RedirectToActionResult(
            AddScreeningViewModel model,
            ISendAddScreeningCommandService serviceStub,
            IResourceAwaiter awaiterDummy,
            Success<ScreeningLocation> success,
            [NoAutoProperties] MoviesController sut)
        {
            // Arrange
            Mock.Get(serviceStub)
                .Setup(
                    x =>
                    x.SendAddScreeningCommand(It.Is<AddNewScreening>(
                        p =>
                        p.MovieId == model.MovieId &&
                        p.TheaterId == model.TheaterId &&
                        p.ScreeningTime == model.ScreeningTime &&
                        p.DefaultFee == model.DefaultFee &&
                        p.ChildrenFee == model.ChildrenFee)))
                .ReturnsAsync(success);

            Guid movieId = model.MovieId;

            // Act
            ActionResult actual = await
                sut.AddScreening(movieId, model, serviceStub, awaiterDummy);

            // Assert
            actual.Should().BeOfType<RedirectToActionResult>();
        }

        [TestMethod, AutoData]
        public async Task AddScreening_sets_action_name_correctly(
            AddScreeningViewModel model,
            ISendAddScreeningCommandService serviceStub,
            IResourceAwaiter awaiterDummy,
            Success<ScreeningLocation> success,
            [NoAutoProperties] MoviesController sut)
        {
            // Arrange
            Mock.Get(serviceStub)
                .Setup(
                    x =>
                    x.SendAddScreeningCommand(It.IsAny<AddNewScreening>()))
                .ReturnsAsync(success);

            Guid movieId = model.MovieId;

            // Act
            dynamic result = await
                sut.AddScreening(movieId, model, serviceStub, awaiterDummy);

            // Assert
            string actual = result.ActionName;
            actual.Should().Be("Screenings");
        }

        [TestMethod, AutoData]
        public async Task AddScreening_sets_route_values_correctly(
            AddScreeningViewModel model,
            ISendAddScreeningCommandService serviceStub,
            IResourceAwaiter awaiterDummy,
            Success<ScreeningLocation> success,
            [NoAutoProperties] MoviesController sut)
        {
            // Arrange
            Mock.Get(serviceStub)
                .Setup(
                    x =>
                    x.SendAddScreeningCommand(It.IsAny<AddNewScreening>()))
                .ReturnsAsync(success);

            Guid movieId = model.MovieId;

            // Act
            dynamic result = await
                sut.AddScreening(movieId, model, serviceStub, awaiterDummy);

            // Assert
            RouteValueDictionary actual = result.RouteValues;
            actual.Should().Contain("movieId", movieId);
        }

        [TestMethod, AutoData]
        public async Task AddScreening_awaits_resource_creation(
            AddScreeningViewModel model,
            ISendAddScreeningCommandService serviceStub,
            IResourceAwaiter awaiterSpy,
            Success<ScreeningLocation> success,
            [NoAutoProperties] MoviesController sut)
        {
            // Arrange
            Mock.Get(serviceStub)
                .Setup(
                    x =>
                    x.SendAddScreeningCommand(It.IsAny<AddNewScreening>()))
                .ReturnsAsync(success);

            Guid movieId = model.MovieId;

            // Act
            await sut.AddScreening(movieId, model, serviceStub, awaiterSpy);

            // Assert
            Uri location = success.Value.Uri;
            Mock.Get(awaiterSpy).Verify(x => x.AwaitResource(location));
        }

        [TestMethod, AutoData]
        public async Task given_service_returns_error_then_AddScreening_adds_model_error(
            AddScreeningViewModel model,
            ISendAddScreeningCommandService serviceStub,
            IResourceAwaiter awaiterDummy,
            Error<ScreeningLocation> error,
            [NoAutoProperties] MoviesController sut)
        {
            // Arrange
            Mock.Get(serviceStub)
                .Setup(
                    x =>
                    x.SendAddScreeningCommand(It.IsAny<AddNewScreening>()))
                .ReturnsAsync(error);

            Guid movieId = model.MovieId;

            // Act
            await sut.AddScreening(movieId, model, serviceStub, awaiterDummy);

            // Assert
            ModelStateEntry state = sut.ModelState[string.Empty];
            state.Errors.Should().Contain(e => e.ErrorMessage == error.Message);
        }

        [TestMethod, AutoData]
        public async Task given_service_returns_error_then_AddScreening_returns_ViewResult(
            AddScreeningViewModel model,
            ISendAddScreeningCommandService serviceStub,
            IResourceAwaiter awaiterDummy,
            Error<ScreeningLocation> error,
            [NoAutoProperties] MoviesController sut)
        {
            // Arrange
            Mock.Get(serviceStub)
                .Setup(
                    x =>
                    x.SendAddScreeningCommand(It.IsAny<AddNewScreening>()))
                .ReturnsAsync(error);

            Guid movieId = model.MovieId;

            // Act
            ActionResult actual = await
                sut.AddScreening(movieId, model, serviceStub, awaiterDummy);

            // Assert
            actual.Should().BeOfType<ViewResult>();
        }

        [TestMethod, AutoData]
        public async Task given_service_returns_error_then_AddScreening_sets_model(
            AddScreeningViewModel model,
            ISendAddScreeningCommandService serviceStub,
            IResourceAwaiter awaiterDummy,
            Error<ScreeningLocation> error,
            [NoAutoProperties] MoviesController sut)
        {
            // Arrange
            Mock.Get(serviceStub)
                .Setup(
                    x =>
                    x.SendAddScreeningCommand(It.IsAny<AddNewScreening>()))
                .ReturnsAsync(error);

            Guid movieId = model.MovieId;

            // Act
            dynamic result = await
                sut.AddScreening(movieId, model, serviceStub, awaiterDummy);

            // Assert
            object actual = result.Model;
            actual.Should().BeSameAs(model);
        }
    }
}
