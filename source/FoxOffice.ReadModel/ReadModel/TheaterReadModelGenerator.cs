namespace FoxOffice.ReadModel
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using FoxOffice.Events;
    using FoxOffice.ReadModel.DataAccess;
    using Khala.Messaging;

    public class TheaterReadModelGenerator :
        InterfaceAwareHandler, IHandles<TheaterCreated>
    {
        private readonly ITheaterRepository _repository;

        public TheaterReadModelGenerator(ITheaterRepository repository)
            => _repository = repository ?? throw new ArgumentNullException(nameof(repository));

        public Task Handle(
            Envelope<TheaterCreated> envelope,
            CancellationToken cancellationToken)
        {
            if (envelope == null)
            {
                throw new ArgumentNullException(nameof(envelope));
            }

            TheaterCreated domainEvent = envelope.Message;
            return _repository.CreateTheater(new Theater
            {
                Id = domainEvent.TheaterId,
                Name = domainEvent.Name,
                SeatRowCount = domainEvent.SeatRowCount,
                SeatColumnCount = domainEvent.SeatColumnCount,
                CreatedAt = domainEvent.RaisedAt,
            });
        }
    }
}
