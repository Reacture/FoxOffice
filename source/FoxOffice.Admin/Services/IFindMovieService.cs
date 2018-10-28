namespace FoxOffice.Admin.Services
{
    using System;
    using System.Threading.Tasks;
    using FoxOffice.ReadModel;

    public interface IFindMovieService
    {
        Task<MovieDto> FindMovie(Guid movieId);
    }
}
