namespace FoxOffice.ReadModel.DataAccess
{
    using System;
    using System.Collections.Immutable;
    using System.Threading.Tasks;

    public interface IMovieReader
    {
        Task<Movie> FindMovie(Guid movieId);

        Task<ImmutableArray<Movie>> GetAllMovies();
    }
}
