namespace FoxOffice.ReadModel.DataAccess
{
    using System.Threading.Tasks;

    public interface ITheaterRepository
    {
        Task CreateTheater(Theater theater);
    }
}
