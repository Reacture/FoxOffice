namespace FoxOffice.Admin.Services
{
    using System.Threading.Tasks;
    using FoxOffice.Commands;

    public interface ISendCreateTheaterCommandService
    {
        Task<IResult<TheaterLocation>> SendCreateTheaterCommand(
            CreateNewTheater command);
    }
}
