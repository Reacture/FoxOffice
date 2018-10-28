namespace FoxOffice.Admin.Controllers
{
    using System.Collections.Immutable;
    using System.Threading.Tasks;
    using FluentAssertions;
    using FoxOffice.Admin.Models;
    using FoxOffice.Admin.Services;
    using FoxOffice.Commands;
    using FoxOffice.ReadModel;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class TheatersController_specs
    {
        [TestMethod, AutoData]
        public async Task Index_returns_ViewResult(
            TheaterDto[] theaters,
            Mock<IGetAllTheatersService> stub,
            [NoAutoProperties] TheatersController sut)
        {
            stub.Setup(x => x.GetAllTheaters()).ReturnsAsync(theaters);
            ActionResult actual = await sut.Index(service: stub.Object);
            actual.Should().BeOfType<ViewResult>();
        }

        [TestMethod, AutoData]
        public async Task Index_sets_model_correctly(
            TheaterDto[] theaters,
            Mock<IGetAllTheatersService> stub,
            [NoAutoProperties] TheatersController sut)
        {
            stub.Setup(x => x.GetAllTheaters()).ReturnsAsync(theaters);

            var result = (ViewResult)await sut.Index(service: stub.Object);

            object actual = result.Model;
            actual.Should().BeOfType<ImmutableArray<TheaterViewModel>>();
            actual.Should().BeEquivalentTo(theaters);
        }

        [TestMethod, AutoData]
        public void Create_returns_ViewResult(
            [NoAutoProperties] TheatersController sut)
        {
            ActionResult actual = sut.Create();
            actual.Should().BeOfType<ViewResult>();
        }

        [TestMethod, AutoData]
        public void Create_sets_model_correctly(
            [NoAutoProperties] TheatersController sut)
        {
            var result = (ViewResult)sut.Create();

            object actual = result.Model;
            actual.Should().BeOfType<CreateTheaterViewModel>();
            actual.Should().BeEquivalentTo(new
            {
                Name = string.Empty,
                SeatRowCount = 1,
                SeatColumnCount = 1,
            });
        }

        [TestMethod, AutoData]
        public async Task given_service_returns_error_then_Create_returns_ViewResult(
            CreateTheaterViewModel model,
            ISendCreateTheaterCommandService serviceStub,
            Error<TheaterLocation> error,
            [NoAutoProperties] TheatersController sut)
        {
            // Arrange
            Mock.Get(serviceStub)
                .Setup(
                    x =>
                    x.SendCreateTheaterCommand(It.IsAny<CreateNewTheater>()))
                .ReturnsAsync(error);

            // Act
            ActionResult actual = await sut.Create(model, serviceStub);

            // Assert
            actual.Should().BeOfType<ViewResult>();
            actual.As<ViewResult>().Model.Should().BeSameAs(model);
        }

        [TestMethod, AutoData]
        public async Task given_service_returns_error_then_Create_adds_model_error(
            CreateTheaterViewModel model,
            ISendCreateTheaterCommandService serviceStub,
            Error<TheaterLocation> error,
            [NoAutoProperties] TheatersController sut)
        {
            // Arrange
            Mock.Get(serviceStub)
                .Setup(
                    x =>
                    x.SendCreateTheaterCommand(It.IsAny<CreateNewTheater>()))
                .ReturnsAsync(error);

            // Act
            await sut.Create(model, serviceStub);

            // Assert
            ModelStateEntry state = sut.ModelState[string.Empty];
            state.Errors.Should().Contain(e => e.ErrorMessage == error.Message);
        }

        [TestMethod, AutoData]
        public async Task given_service_returns_success_then_Create_returns_RedirectToActionResult(
            CreateTheaterViewModel model,
            ISendCreateTheaterCommandService serviceStub,
            Success<TheaterLocation> success,
            [NoAutoProperties] TheatersController sut)
        {
            // Arrange
            Mock.Get(serviceStub)
                .Setup(
                    x =>
                    x.SendCreateTheaterCommand(It.Is<CreateNewTheater>(
                        p =>
                        p.Name == model.Name &&
                        p.SeatRowCount == model.SeatRowCount &&
                        p.SeatColumnCount == model.SeatColumnCount)))
                .ReturnsAsync(success);

            // Act
            ActionResult actual = await sut.Create(model, serviceStub);

            // Assert
            actual.Should().BeOfType<RedirectToActionResult>();
            actual.As<RedirectToActionResult>().ActionName.Should().Be("Index");
        }
    }
}
