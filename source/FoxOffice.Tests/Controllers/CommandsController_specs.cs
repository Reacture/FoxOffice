namespace FoxOffice.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using FoxOffice.Commands;
    using Khala.Messaging;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CommandsController_specs
    {
        [TestMethod]
        public void sut_inherits_ControllerBase()
        {
            typeof(CommandsController).Should().BeDerivedFrom<ControllerBase>();
        }

        [TestMethod]
        public void sut_is_decorated_with_ApiController_attribute()
        {
            typeof(CommandsController).Should()
                .BeDecoratedWith<ApiControllerAttribute>();
        }

        [TestMethod, AutoData]
        public async Task SendCreateTheaterCommand_sends_command_correctly(
            CreateNewTheater source,
            InProcessMessageLogger messageBusSpy,
            [NoAutoProperties] CommandsController sut)
        {
            // Act
            await sut.SendCreateTheaterCommand(source, messageBusSpy);

            // Assert
            IEnumerable<Envelope> log = messageBusSpy.Log;

            log.Should().ContainSingle();
            log.Single().Message.Should().BeOfType<CreateTheater>();

            var actual = (CreateTheater)log.Single().Message;
            actual.TheaterId.Should().NotBeEmpty();
            actual.Should().BeEquivalentTo(new
            {
                source.Name,
                source.SeatRowCount,
                source.SeatColumnCount,
            });
        }

        [TestMethod, AutoData]
        public async Task SendCreateTheaterCommand_returns_AcceptedResult(
            CreateNewTheater content,
            InProcessMessageLogger messageBusDummy,
            [NoAutoProperties] CommandsController sut)
        {
            // Act
            IActionResult actual = await
                sut.SendCreateTheaterCommand(content, messageBusDummy);

            // Assert
            actual.Should().BeOfType<AcceptedResult>();
        }

        [TestMethod, AutoData]
        public async Task SendCreateTheaterCommand_sets_location_correctly(
            CreateNewTheater content,
            InProcessMessageLogger messageBusSpy,
            [NoAutoProperties] CommandsController sut)
        {
            // Act
            IActionResult result = await
                sut.SendCreateTheaterCommand(content, messageBusSpy);

            // Assert
            var accepted = (AcceptedResult)result;
            IEnumerable<Envelope> log = messageBusSpy.Log;
            Guid theaterId = log.Single().Message.As<CreateTheater>().TheaterId;
            accepted.Location.Should().Be($"api/queries/Theaters/{theaterId}");
        }

        [TestMethod, AutoData]
        public async Task SendCreateMovieCommand_sends_command_correctly(
            CreateNewMovie source,
            InProcessMessageLogger messageBusSpy,
            [NoAutoProperties] CommandsController sut)
        {
            // Act
            await sut.SendCreateMovieCommand(source, messageBusSpy);

            // Assert
            IEnumerable<Envelope> log = messageBusSpy.Log;

            log.Should().ContainSingle();
            log.Single().Message.Should().BeOfType<CreateMovie>();

            var actual = (CreateMovie)log.Single().Message;
            actual.MovieId.Should().NotBeEmpty();
            actual.Should().BeEquivalentTo(new { source.Title });
        }

        [TestMethod, AutoData]
        public async Task SendCreateMovieCommand_returns_AcceptedResult(
            CreateNewMovie content,
            InProcessMessageLogger messageBusDummy,
            [NoAutoProperties] CommandsController sut)
        {
            // Act
            IActionResult actual = await
                sut.SendCreateMovieCommand(content, messageBusDummy);

            // Assert
            actual.Should().BeOfType<AcceptedResult>();
        }

        [TestMethod, AutoData]
        public async Task SendCreateMovieCommand_sets_location_correctly(
            CreateNewMovie content,
            InProcessMessageLogger messageBusSpy,
            [NoAutoProperties] CommandsController sut)
        {
            // Act
            IActionResult result = await
                sut.SendCreateMovieCommand(content, messageBusSpy);

            // Assert
            var accepted = (AcceptedResult)result;
            IEnumerable<Envelope> log = messageBusSpy.Log;
            Guid movieId = log.Single().Message.As<CreateMovie>().MovieId;
            accepted.Location.Should().Be($"api/queries/Movies/{movieId}");
        }

        [TestMethod, AutoData]
        public async Task SendAddScreeningCommand_sends_command_correctly(
            AddNewScreening source,
            InProcessMessageLogger messageBusSpy,
            [NoAutoProperties] CommandsController sut)
        {
            // Act
            await sut.SendAddScreeningCommand(source, messageBusSpy);

            // Assert
            IEnumerable<Envelope> log = messageBusSpy.Log;

            log.Should().ContainSingle();
            log.Single().Message.Should().BeOfType<AddScreening>();

            var actual = (AddScreening)log.Single().Message;
            actual.MovieId.Should().NotBeEmpty();
            actual.Should().BeEquivalentTo(source);
        }

        [TestMethod, AutoData]
        public async Task SendAddScreeningCommand_returns_AcceptedResult(
            AddNewScreening source,
            IMessageBus messageBusDummy,
            [NoAutoProperties] CommandsController sut)
        {
            // Act
            IActionResult actual = await
                sut.SendAddScreeningCommand(source, messageBusDummy);

            // Assert
            actual.Should().BeOfType<AcceptedResult>();
        }

        [TestMethod, AutoData]
        public async Task SendAddScreeningCommand_sets_location_correctly(
            AddNewScreening source,
            InProcessMessageLogger messageBusSpy,
            [NoAutoProperties] CommandsController sut)
        {
            // Act
            dynamic result = await
                sut.SendAddScreeningCommand(source, messageBusSpy);

            // Assert
            Guid movieId = source.MovieId;

            IEnumerable<Envelope> log = messageBusSpy.Log;
            dynamic message = log.Single().Message;
            Guid screeningId = message.ScreeningId;

            string actual = result.Location;
            string uri = $"api/queries/Movies/{movieId}/Screenings/{screeningId}";
            actual.Should().Be(uri);
        }
    }
}
