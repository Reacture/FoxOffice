namespace FoxOffice.Domain
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using AutoFixture.Idioms;
    using FluentAssertions;
    using FoxOffice.Events;
    using Khala.EventSourcing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class Theater_specs
    {
        [TestMethod]
        public void Theater_inherits_EventSourced()
        {
            typeof(Theater).Should().BeDerivedFrom<EventSourced>();
        }

        [TestMethod, AutoData]
        public void constructor_raises_TheaterCreated_event_correctly(
            Guid theaterId, string name, int seatRowCount, int seatColumnCount)
        {
            // Act
            var sut = new Theater(
                theaterId, name, seatRowCount, seatColumnCount);

            // Assert
            sut.PendingEvents
                .Should().ContainSingle().Which
                .Should().BeOfType<TheaterCreated>().Which
                .Should().BeEquivalentTo(new
                {
                    Name = name,
                    SeatRowCount = seatRowCount,
                    SeatColumnCount = seatColumnCount,
                });
        }

        [TestMethod, AutoData]
        public void sut_has_guard_clauses(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(Theater));
        }

        [TestMethod, AutoData]
        public void constructor_has_guard_clause_against_non_positive_seatRowCount(
            Guid theaterId,
            string name,
            [Range(int.MinValue, 0)] int seatRowCount,
            int seatColumnCount)
        {
            // Act
            Action action = ()
                => new Theater(theaterId, name, seatRowCount, seatColumnCount);

            // Assert
            action.Should()
                .Throw<ArgumentOutOfRangeException>()
                .Where(x => x.ParamName == "seatRowCount");
        }

        [TestMethod, AutoData]
        public void constructor_has_guard_clause_against_non_positive_seatColumnCount(
            Guid theaterId,
            string name,
            int seatRowCount,
            [Range(int.MinValue, 0)] int seatColumnCount)
        {
            // Act
            Action action = ()
                => new Theater(theaterId, name, seatRowCount, seatColumnCount);

            // Assert
            action.Should()
                .Throw<ArgumentOutOfRangeException>()
                .Where(x => x.ParamName == "seatColumnCount");
        }

        [TestMethod, AutoData]
        public void constructor_sets_SeatRowCount_and_SeatColumnCount_correctly(
            Guid theaterId, string name, int seatRowCount, int seatColumnCount)
        {
            // Act
            var sut = new Theater(theaterId, name, seatRowCount, seatColumnCount);

            // Assert
            sut.SeatRowCount.Should().Be(seatRowCount);
            sut.SeatColumnCount.Should().Be(seatColumnCount);
        }
    }
}
