namespace FoxOffice.Admin.Services
{
    using System.Threading.Tasks;
    using FoxOffice.Commands;

    public interface ISendAddScreeningCommandService
    {
        Task<IResult<ScreeningLocation>> SendAddScreeningCommand(
            AddNewScreening command);
    }
}
