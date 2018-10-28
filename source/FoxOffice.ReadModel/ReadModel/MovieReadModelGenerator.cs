namespace FoxOffice.ReadModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using FoxOffice.Events;
    using FoxOffice.ReadModel.DataAccess;
    using Khala.Messaging;

    public class MovieReadModelGenerator :
        InterfaceAwareHandler, IHandles<MovieCreated>, IHandles<ScreeningAdded>
    {
        private readonly IMovieRepository _movieRepository;
        private readonly ITheaterReader _theaterReader;

        public MovieReadModelGenerator(
            IMovieRepository movieRepository,
            ITheaterReader theaterReader)
        {
            _movieRepository = movieRepository ?? throw new ArgumentNullException(nameof(movieRepository));
            _theaterReader = theaterReader ?? throw new ArgumentNullException(nameof(theaterReader));
        }

        public Task Handle(
            Envelope<MovieCreated> envelope,
            CancellationToken cancellationToken)
        {
            if (envelope == null)
            {
                throw new ArgumentNullException(nameof(envelope));
            }

            MovieCreated domainEvent = envelope.Message;
            return _movieRepository.CreateMovie(new Movie
            {
                Id = domainEvent.MovieId,
                Title = domainEvent.Title,
                CreatedAt = domainEvent.RaisedAt,
            });
        }

        public Task Handle(
            Envelope<ScreeningAdded> envelope,
            CancellationToken cancellationToken)
        {
            if (envelope == null)
            {
                throw new ArgumentNullException(nameof(envelope));
            }

            return HandleScreeningAdded(envelope, cancellationToken);
        }

        private async Task HandleScreeningAdded(
            Envelope<ScreeningAdded> envelope,
            CancellationToken cancellationToken)
        {
            ScreeningAdded domainEvent = envelope.Message;

            Movie movie = await _movieRepository.FindMovie(domainEvent.MovieId);

            Guid theaterId = domainEvent.TheaterId;
            Theater theater = await _theaterReader.FindTheater(theaterId);

            movie.Screenings.Add(new Screening
            {
                Id = domainEvent.ScreeningId,
                TheaterId = theaterId,
                TheaterName = theater.Name,
                Seats = new List<Seat>(
                    from r in Enumerable.Range(0, domainEvent.SeatRowCount)
                    from c in Enumerable.Range(0, domainEvent.SeatColumnCount)
                    select new Seat
                    {
                        Row = r,
                        Column = c,
                        IsReserved = false,
                    }),
                ScreeningTime = domainEvent.ScreeningTime,
                DefaultFee = domainEvent.DefaultFee,
                ChildrenFee = domainEvent.ChildrenFee,
                CreatedAt = domainEvent.RaisedAt,
            });

            await _movieRepository.UpdateMovie(movie);
        }
    }
}
