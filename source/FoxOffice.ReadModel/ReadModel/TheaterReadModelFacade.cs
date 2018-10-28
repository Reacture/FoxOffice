namespace FoxOffice.ReadModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading.Tasks;
    using FoxOffice.ReadModel.DataAccess;

    public class TheaterReadModelFacade
    {
        private readonly ITheaterReader _reader;

        public TheaterReadModelFacade(ITheaterReader reader)
            => _reader = reader ?? throw new ArgumentNullException(nameof(reader));

        public Task<TheaterDto> FindTheater(Guid theaterId)
        {
            if (theaterId == Guid.Empty)
            {
                string message = "Value cannot be empty.";
                throw new ArgumentException(message, nameof(theaterId));
            }

            return FindTheaterImpl(theaterId);
        }

        private async Task<TheaterDto> FindTheaterImpl(Guid theaterId)
            => Translate(await _reader.FindTheater(theaterId));

        public async Task<ImmutableArray<TheaterDto>> GetAllTheaters()
        {
            IEnumerable<TheaterDto> query =
                from entity in await _reader.GetAllTheaters()
                orderby entity.CreatedAt descending
                select Translate(entity);

            return ImmutableArray.CreateRange(query);
        }

        private static TheaterDto Translate(Theater entity)
        {
            return entity == null ? default : new TheaterDto
            {
                Id = entity.Id,
                Name = entity.Name,
                SeatRowCount = entity.SeatRowCount,
                SeatColumnCount = entity.SeatColumnCount,
                CreatedAt = entity.CreatedAt,
            };
        }
    }
}
