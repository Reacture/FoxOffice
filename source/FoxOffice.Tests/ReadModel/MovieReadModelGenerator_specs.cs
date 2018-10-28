namespace FoxOffice.ReadModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture;
    using AutoFixture.Idioms;
    using FluentAssertions;
    using FoxOffice.Events;
    using FoxOffice.ReadModel.DataAccess;
    using Khala.Messaging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class MovieReadModelGenerator_specs
    {
        [TestMethod]
        public void sut_inherits_InterfaceAwareHandler()
        {
            typeof(MovieReadModelGenerator)
                .Should().BeDerivedFrom<InterfaceAwareHandler>();
        }

        [TestMethod, AutoData]
        public void sut_has_gaurd_clauses(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(MovieReadModelGenerator));
        }

        [TestMethod]
        public void sut_handles_MovieCreated_event()
        {
            typeof(MovieReadModelGenerator)
                .Should().Implement<IHandles<MovieCreated>>();
        }

        [TestMethod, AutoData]
        public async Task MovieCreated_event_handler_creates_read_model_entity_correctly(
            MovieCreated domainEvent,
            InMemoryMovieRepository repositorySpy)
        {
            var sut = new MovieReadModelGenerator(
                repositorySpy, Mock.Of<ITheaterReader>());

            await sut.Handle(new Envelope(domainEvent));

            IDictionary<Guid, Movie> data = repositorySpy.Data;
            data.Should().ContainKey(domainEvent.MovieId);
            data[domainEvent.MovieId].Should().BeEquivalentTo(new
            {
                domainEvent.Title,
                CreatedAt = domainEvent.RaisedAt,
                ETag = default(string),
            });
        }

        [TestMethod]
        public void sut_handles_ScreeningAdded_event()
        {
            typeof(MovieReadModelGenerator)
                .Should().Implement<IHandles<ScreeningAdded>>();
        }

        [TestMethod, AutoData]
        public async Task ScreeningAdded_event_handler_add_screening_entity_correctly(
            MovieCreated movieCreated,
            Theater theater,
            InMemoryMovieRepository movieRepositoryDouble,
            InMemoryTheaterRepository theaterReaderStub,
            IFixture builder)
        {
            // Arrange
            theaterReaderStub.Data[theater.Id] = theater;

            var sut = new MovieReadModelGenerator(
                movieRepositoryDouble, theaterReaderStub);

            await sut.Handle(new Envelope(movieCreated));

            Guid movieId = movieCreated.MovieId;

            ScreeningAdded domainEvent = builder
                .Build<ScreeningAdded>()
                .With(x => x.SourceId, movieId)
                .With(x => x.TheaterId, theater.Id)
                .With(x => x.SeatRowCount, theater.SeatRowCount)
                .With(x => x.SeatColumnCount, theater.SeatColumnCount)
                .Create();

            // Act
            await sut.Handle(new Envelope(domainEvent));

            // Assert
            Movie actual = movieRepositoryDouble.Data[movieId];
            actual.Screenings
                .Should().Contain(s => s.Id == domainEvent.ScreeningId).Which
                .Should().BeEquivalentTo(new
                {
                    Id = domainEvent.ScreeningId,
                    domainEvent.TheaterId,
                    TheaterName = theater.Name,
                    Seats =
                        from r in Enumerable.Range(0, theater.SeatRowCount)
                        from c in Enumerable.Range(0, theater.SeatColumnCount)
                        select new Seat
                        {
                            Row = r,
                            Column = c,
                            IsReserved = false,
                        },
                    domainEvent.ScreeningTime,
                    domainEvent.DefaultFee,
                    domainEvent.ChildrenFee,
                    CreatedAt = domainEvent.RaisedAt,
                });
        }
    }
}
