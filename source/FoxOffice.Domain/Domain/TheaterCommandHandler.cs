namespace FoxOffice.Domain
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using FoxOffice.Commands;
    using Khala.EventSourcing;
    using Khala.Messaging;

    public class TheaterCommandHandler :
        InterfaceAwareHandler, IHandles<CreateTheater>
    {
        private readonly IEventSourcedRepository<Theater> _repository;

        public TheaterCommandHandler(
            IEventSourcedRepository<Theater> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public Task Handle(
            Envelope<CreateTheater> envelope,
            CancellationToken cancellationToken)
        {
            if (envelope == null)
            {
                throw new ArgumentNullException(nameof(envelope));
            }

            CreateTheater command = envelope.Message;

            var theater = new Theater(
                command.TheaterId,
                command.Name,
                command.SeatRowCount,
                command.SeatColumnCount);

            return _repository.SaveAndPublish(theater);
        }
    }
}
