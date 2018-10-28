namespace FoxOffice.ReadModel
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoFixture.Idioms;
    using FluentAssertions;
    using FoxOffice.Events;
    using FoxOffice.ReadModel.DataAccess;
    using Khala.Messaging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TheaterReadModelGenerator_specs
    {
        [TestMethod]
        public void sut_inherits_InterfaceAwareHandler()
        {
            typeof(TheaterReadModelGenerator)
                .Should().BeDerivedFrom<InterfaceAwareHandler>();
        }

        [TestMethod, AutoData]
        public void sut_has_guard_clauses(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(TheaterReadModelGenerator));
        }

        [TestMethod]
        public void sut_handles_TheaterCreated_event()
        {
            typeof(TheaterReadModelGenerator)
                .Should().Implement<IHandles<TheaterCreated>>();
        }

        [TestMethod, AutoData]
        public async Task TheaterCreated_event_handler_creates_read_model_entity_correctly(
            TheaterCreated domainEvent,
            InMemoryTheaterRepository repositorySpy)
        {
            // Arrange
            var sut = new TheaterReadModelGenerator(repositorySpy);

            // Act
            await sut.Handle(new Envelope(domainEvent));

            // Assert
            IDictionary<Guid, Theater> data = repositorySpy.Data;
            data.Should().ContainKey(domainEvent.TheaterId);
            data[domainEvent.TheaterId].Should().BeEquivalentTo(new
            {
                domainEvent.Name,
                domainEvent.SeatRowCount,
                domainEvent.SeatColumnCount,
                CreatedAt = domainEvent.RaisedAt,
                ETag = default(string),
            });
        }
    }
}
