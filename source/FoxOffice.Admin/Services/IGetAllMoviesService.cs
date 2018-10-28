namespace FoxOffice.Admin.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FoxOffice.ReadModel;

    public interface IGetAllMoviesService
    {
        Task<IEnumerable<MovieDto>> GetAllMovies();
    }
}
