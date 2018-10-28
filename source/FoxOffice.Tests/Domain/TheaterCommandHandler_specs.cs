namespace FoxOffice.Domain
{
    using System.Threading.Tasks;
    using AutoFixture.Idioms;
    using FluentAssertions;
    using FoxOffice.Commands;
    using Khala.EventSourcing;
    using Khala.Messaging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using static EventSourcing;

    [TestClass]
    public class TheaterCommandHandler_specs
    {
        [TestMethod]
        public void sut_inherits_InterfaceAwareHandler()
        {
            typeof(TheaterCommandHandler).Should()
                .BeDerivedFrom<InterfaceAwareHandler>();
        }

        [TestMethod, AutoData]
        public void sut_has_guard_clauses(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(TheaterCommandHandler));
        }

        [TestMethod]
        public void sut_handles_CreateTheater_command()
        {
            typeof(TheaterCommandHandler).Should()
                .Implement<IHandles<CreateTheater>>();
        }

        [TestMethod, AutoData]
        public async Task CreateTheater_command_handler_creates_Theater_aggregate_root(
            CreateTheater command)
        {
            IEventSourcedRepository<Theater>
                repository = GetRepository(Theater.Factory);
            var sut = new TheaterCommandHandler(repository);

            await sut.Handle(new Envelope(command));

            Theater actual = await repository.Find(command.TheaterId);
            actual.Should().NotBeNull();
        }
    }
}
