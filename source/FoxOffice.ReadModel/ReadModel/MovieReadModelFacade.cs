namespace FoxOffice.ReadModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using FoxOffice.ReadModel.DataAccess;

    public class MovieReadModelFacade
    {
        private static readonly IMapper _mapper;

        private readonly IMovieReader _reader;

        static MovieReadModelFacade()
        {
            _mapper = new MapperConfiguration(expr =>
            {
                expr.CreateMap<Screening, ScreeningDto>();
                expr.CreateMap<Movie, MovieDto>();
            }).CreateMapper();
        }

        public MovieReadModelFacade(IMovieReader reader)
            => _reader = reader ?? throw new ArgumentNullException(nameof(reader));

        public Task<MovieDto> FindMovie(Guid movieId)
        {
            if (movieId == Guid.Empty)
            {
                string message = "Value cannot be empty.";
                throw new ArgumentException(message, nameof(movieId));
            }

            return FindMovieImpl(movieId);
        }

        private async Task<MovieDto> FindMovieImpl(Guid movieId)
            => Translate(await _reader.FindMovie(movieId));

        public async Task<ImmutableArray<MovieDto>> GetAllMovies()
        {
            IEnumerable<MovieDto> query =
                from entity in await _reader.GetAllMovies()
                orderby entity.CreatedAt descending
                select Translate(entity);

            return ImmutableArray.CreateRange(query);
        }

        private MovieDto Translate(Movie movie) => _mapper.Map<MovieDto>(movie);
    }
}
