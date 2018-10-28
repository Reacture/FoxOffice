namespace FoxOffice.ReadModel.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Threading.Tasks;

    public class InMemoryTheaterRepository : ITheaterRepository, ITheaterReader
    {
        private readonly Dictionary<Guid, Theater> _data;

        public InMemoryTheaterRepository()
            => _data = new Dictionary<Guid, Theater>();

        public IDictionary<Guid, Theater> Data => _data;

        public async Task CreateTheater(Theater theater)
        {
            await Task.Delay(millisecondsDelay: 1);
            _data.Add(theater.Id, theater);
        }

        public async Task<Theater> FindTheater(Guid theaterId)
        {
            await Task.CompletedTask;
            return _data.TryGetValue(theaterId, out Theater t) ? t : default;
        }

        public Task<ImmutableArray<Theater>> GetAllTheaters()
            => Task.FromResult(_data.Values.ToImmutableArray());
    }
}
