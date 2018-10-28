namespace FoxOffice.Domain
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using FoxOffice.Commands;
    using Khala.EventSourcing;
    using Khala.Messaging;

    public class MovieCommandHandler :
        InterfaceAwareHandler, IHandles<CreateMovie>, IHandles<AddScreening>
    {
        private readonly IEventSourcedRepository<Movie> _movieRepository;
        private readonly IEventSourcedRepository<Theater> _theaterRepository;

        public MovieCommandHandler(
            IEventSourcedRepository<Movie> movieRepository,
            IEventSourcedRepository<Theater> theaterRepository)
        {
            _movieRepository = movieRepository ?? throw new ArgumentNullException(nameof(movieRepository));
            _theaterRepository = theaterRepository ?? throw new ArgumentNullException(nameof(theaterRepository));
        }

        public Task Handle(
            Envelope<CreateMovie> envelope,
            CancellationToken cancellationToken)
        {
            if (envelope == null)
            {
                throw new ArgumentNullException(nameof(envelope));
            }

            CreateMovie command = envelope.Message;
            var movie = new Movie(command.MovieId, command.Title);
            return _movieRepository.SaveAndPublish(movie);
        }

        public Task Handle(
            Envelope<AddScreening> envelope,
            CancellationToken cancellationToken)
        {
            if (envelope == null)
            {
                throw new ArgumentNullException(nameof(envelope));
            }

            return HandleAddScreening(envelope, cancellationToken);
        }

        private async Task HandleAddScreening(
            Envelope<AddScreening> envelope,
            CancellationToken cancellationToken)
        {
            AddScreening command = envelope.Message;

            Movie movie = await _movieRepository.Find(command.MovieId);
            Theater theater = await _theaterRepository.Find(command.TheaterId);

            movie.AddScreening(
                command.ScreeningId,
                command.TheaterId,
                theater.SeatRowCount,
                theater.SeatColumnCount,
                command.ScreeningTime,
                command.DefaultFee,
                command.ChildrenFee);

            await _movieRepository.SaveAndPublish(
                movie, correlation: envelope, cancellationToken);
        }
    }
}
