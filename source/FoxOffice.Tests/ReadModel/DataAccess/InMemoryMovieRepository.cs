namespace FoxOffice.ReadModel.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Threading.Tasks;
    using AutoMapper;

    public class InMemoryMovieRepository : IMovieRepository, IMovieReader
    {
        private readonly IMapper _mapper = new MapperConfiguration(c => c.CreateMap<Movie, Movie>()).CreateMapper();
        private readonly Dictionary<Guid, Movie> _data = new Dictionary<Guid, Movie>();

        public IDictionary<Guid, Movie> Data => _data;

        public async Task CreateMovie(Movie movie)
        {
            await Task.Delay(millisecondsDelay: 1);
            _data.Add(movie.Id, Clone(movie));
        }

        public async Task UpdateMovie(Movie movie)
        {
            await Task.Delay(millisecondsDelay: 1);
            _data[movie.Id] = Clone(movie);
        }

        public async Task<Movie> FindMovie(Guid movieId)
        {
            await Task.CompletedTask;
            return _data.TryGetValue(movieId, out Movie m) ? Clone(m) : default;
        }

        private Movie Clone(Movie movie) => _mapper.Map<Movie>(movie);

        public Task<ImmutableArray<Movie>> GetAllMovies()
            => Task.FromResult(_data.Values.ToImmutableArray());
    }
}
