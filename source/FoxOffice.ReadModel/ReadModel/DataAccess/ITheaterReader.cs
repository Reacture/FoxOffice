namespace FoxOffice.ReadModel.DataAccess
{
    using System;
    using System.Collections.Immutable;
    using System.Threading.Tasks;

    public interface ITheaterReader
    {
        Task<Theater> FindTheater(Guid theaterId);

        Task<ImmutableArray<Theater>> GetAllTheaters();
    }
}
