namespace FoxOffice.Domain
{
    using System;
    using FluentAssertions;
    using FoxOffice.Events;
    using Khala.EventSourcing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class Movie_specs
    {
        [TestMethod]
        public void sut_inherits_EventSourced()
        {
            typeof(Movie).Should().BeDerivedFrom<EventSourced>();
        }

        [TestMethod, AutoData]
        public void constructor_raises_MovieCreated_event_correctly(
            Guid movieId, string title)
        {
            // Act
            var sut = new Movie(movieId, title);

            // Assert
            sut.PendingEvents
                .Should().ContainSingle().Which
                .Should().BeOfType<MovieCreated>().Which
                .Should().BeEquivalentTo(new { Title = title });
        }

        [TestMethod, AutoData]
        public void AddScreening_raises_ScreeningAdded_event_correctly(
            Movie sut,
            Guid screeningId,
            Guid theaterId,
            int seatRowCount,
            int seatColumnCount,
            DateTime screeningTime,
            decimal defaultFee,
            decimal childrenFee)
        {
            // Arrange
            sut.FlushPendingEvents();

            // Act
            sut.AddScreening(
                screeningId,
                theaterId,
                seatRowCount,
                seatColumnCount,
                screeningTime,
                defaultFee,
                childrenFee);

            // Assert
            sut.PendingEvents.Should().ContainSingle()
                .Which.Should().BeOfType<ScreeningAdded>()
                .And.BeEquivalentTo(new
                {
                    ScreeningId = screeningId,
                    TheaterId = theaterId,
                    SeatRowCount = seatRowCount,
                    SeatColumnCount = seatColumnCount,
                    ScreeningTime = screeningTime,
                    DefaultFee = defaultFee,
                    ChildrenFee = childrenFee,
                });
        }
    }
}
