namespace FoxOffice.ReadModel
{
    using System;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoFixture.Idioms;
    using FluentAssertions;
    using FoxOffice.ReadModel.DataAccess;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TheaterReadModelFacade_specs
    {
        [TestMethod, AutoData]
        public void sut_has_guard_clauses(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(TheaterReadModelFacade));
        }

        [TestMethod, AutoData]
        public async Task FindTheater_assembles_transfer_object_correctly(
            Theater entity, InMemoryTheaterRepository readerStub)
        {
            readerStub.Data[entity.Id] = entity;
            var sut = new TheaterReadModelFacade(readerStub);

            TheaterDto actual = await sut.FindTheater(theaterId: entity.Id);

            actual.Should().BeEquivalentTo(new
            {
                entity.Id,
                entity.Name,
                entity.SeatRowCount,
                entity.SeatColumnCount,
                entity.CreatedAt,
            });
        }

        [TestMethod, AutoData]
        public async Task given_model_not_found_then_FindTheater_returns_null(
            TheaterReadModelFacade sut, Guid theaterId)
        {
            TheaterDto actual = await sut.FindTheater(theaterId);
            actual.Should().BeNull();
        }

        [TestMethod, AutoData]
        public async Task GetAllTheaters_assembles_all_transfer_objects_correctly(
            ImmutableArray<Theater> entities,
            InMemoryTheaterRepository readerStub)
        {
            entities.ForEach(x => readerStub.Data[x.Id] = x);
            var sut = new TheaterReadModelFacade(readerStub);

            ImmutableArray<TheaterDto> actual = await sut.GetAllTheaters();

            actual.Should().BeEquivalentTo(
                expectation: entities,
                opts => opts.ExcludingMissingMembers());
        }

        [TestMethod, AutoData]
        public async Task GetAllTheaters_sort_transfer_objects_by_CreatedAt(
            ImmutableArray<Theater> entities,
            InMemoryTheaterRepository readerStub)
        {
            entities.ForEach(x => readerStub.Data[x.Id] = x);
            var sut = new TheaterReadModelFacade(readerStub);

            ImmutableArray<TheaterDto> actual = await sut.GetAllTheaters();

            actual.Should().BeInDescendingOrder(x => x.CreatedAt);
        }
    }
}
