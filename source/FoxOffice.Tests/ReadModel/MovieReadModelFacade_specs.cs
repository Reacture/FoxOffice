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
    public class MovieReadModelFacade_specs
    {
        [TestMethod, AutoData]
        public void sut_has_guard_clauses(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(MovieReadModelFacade));
        }

        [TestMethod, AutoData]
        public async Task FindMovie_assembles_dto_correctly(
            Movie movie, InMemoryMovieRepository readerStub)
        {
            readerStub.Data[movie.Id] = movie;
            var sut = new MovieReadModelFacade(readerStub);

            MovieDto actual = await sut.FindMovie(movie.Id);

            actual.Should().BeEquivalentTo(
                expectation: movie,
                config: c => c.ExcludingMissingMembers());
        }

        [TestMethod, AutoData]
        public async Task given_entity_not_found_then_FindMovie_returns_null(
            MovieReadModelFacade sut, Guid movieId)
        {
            MovieDto actual = await sut.FindMovie(movieId);
            actual.Should().BeNull();
        }

        [TestMethod, AutoData]
        public async Task GetAllMovies_assembles_all_transfer_objects_correctly(
            ImmutableArray<Movie> movies,
            InMemoryMovieRepository readerStub)
        {
            movies.ForEach(x => readerStub.Data[x.Id] = x);
            var sut = new MovieReadModelFacade(readerStub);

            ImmutableArray<MovieDto> actual = await sut.GetAllMovies();

            actual.Should().BeEquivalentTo(
                expectation: movies,
                config: c => c.ExcludingMissingMembers());
        }

        [TestMethod, AutoData]
        public async Task GetAllMovies_sort_transfer_objects_by_CreatedAt(
            ImmutableArray<Movie> entities,
            InMemoryMovieRepository readerStub)
        {
            entities.ForEach(x => readerStub.Data[x.Id] = x);
            var sut = new MovieReadModelFacade(readerStub);

            ImmutableArray<MovieDto> actual = await sut.GetAllMovies();

            actual.Should().BeInDescendingOrder(x => x.CreatedAt);
        }
    }
}
