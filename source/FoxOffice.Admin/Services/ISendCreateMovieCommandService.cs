namespace FoxOffice.Admin.Services
{
    using System.Threading.Tasks;
    using FoxOffice.Commands;

    public interface ISendCreateMovieCommandService
    {
        Task<IResult<MovieLocation>> SendCreateMovieCommand(
            CreateNewMovie command);
    }
}
