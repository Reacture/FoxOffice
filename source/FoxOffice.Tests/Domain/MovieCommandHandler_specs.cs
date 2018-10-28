namespace FoxOffice.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using AutoFixture.Idioms;
    using FluentAssertions;
    using FoxOffice.Commands;
    using Khala.EventSourcing;
    using Khala.Messaging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using static EventSourcing;

    [TestClass]
    public class MovieCommandHandler_specs
    {
        [TestMethod]
        public void sut_inherits_InterfaceAwareHandler()
        {
            typeof(MovieCommandHandler).Should()
                .BeDerivedFrom<InterfaceAwareHandler>();
        }

        [TestMethod, AutoData]
        public void sut_has_guard_clauses(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(MovieCommandHandler));
        }

        [TestMethod]
        public void sut_handles_CreateMovie_command()
        {
            typeof(MovieCommandHandler).Should()
                .Implement<IHandles<CreateMovie>>();
        }

        [TestMethod, AutoData]
        public async Task CreateMovie_command_handler_creates_Movie_aggregate_root(
            CreateMovie command)
        {
            IEventSourcedRepository<Movie>
                repository = GetRepository(Movie.Factory);
            var sut = new MovieCommandHandler(
                repository, GetRepository(Theater.Factory));

            await sut.Handle(new Envelope(command));

            Movie actual = await repository.Find(command.MovieId);
            actual.Should().NotBeNull();
            actual.Title.Should().Be(command.Title);
        }

        [TestMethod]
        public void sut_handles_AddScreening_command()
        {
            typeof(MovieCommandHandler).Should()
                .Implement<IHandles<AddScreening>>();
        }

        [TestMethod, AutoData]
        public async Task AddScreening_command_handler_adds_Screening_correctly(
            Guid theaterId,
            string name,
            [Range(1, 20)] int seatRowCount,
            [Range(1, 20)] int seatColumnCount,
            Movie movie,
            IFixture builder)
        {
            // Arange
            IEventSourcedRepository<Theater>
                theaterRepository = GetRepository(Theater.Factory);
            var theater = new Theater(
                theaterId, name, seatRowCount, seatColumnCount);
            await theaterRepository.SaveAndPublish(theater);

            IEventSourcedRepository<Movie>
                movieRepository = GetRepository(Movie.Factory);
            await movieRepository.SaveAndPublish(movie);

            var sut = new MovieCommandHandler(
                movieRepository, theaterRepository);

            AddScreening command = builder.Build<AddScreening>()
                .With(e => e.MovieId, movie.Id)
                .With(e => e.TheaterId, theaterId)
                .Create();

            // Act
            await sut.Handle(new Envelope(command));

            // Assert
            Movie actual = await movieRepository.Find(movie.Id);
            actual.Screenings
                .Should().Contain(s => s.Id == command.ScreeningId).Which
                .Should().BeEquivalentTo(new
                {
                    Id = command.ScreeningId,
                    command.TheaterId,
                    Seats = from r in Enumerable.Range(0, seatRowCount)
                            from c in Enumerable.Range(0, seatColumnCount)
                            let isReserved = false
                            select new Seat(row: r, column: c, isReserved),
                    command.ScreeningTime,
                    command.DefaultFee,
                    command.ChildrenFee,
                });
        }
    }
}
