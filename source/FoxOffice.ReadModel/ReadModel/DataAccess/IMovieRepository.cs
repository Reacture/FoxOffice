namespace FoxOffice.ReadModel.DataAccess
{
    using System.Threading.Tasks;

    public interface IMovieRepository : IMovieReader
    {
        Task CreateMovie(Movie movie);

        Task UpdateMovie(Movie movie);
    }
}
