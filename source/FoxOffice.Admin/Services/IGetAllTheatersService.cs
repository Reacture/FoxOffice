namespace FoxOffice.Admin.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FoxOffice.ReadModel;

    public interface IGetAllTheatersService
    {
        Task<IEnumerable<TheaterDto>> GetAllTheaters();
    }
}
