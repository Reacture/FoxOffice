namespace FoxOffice.Domain
{
    using System;
    using System.Collections.Generic;
    using FoxOffice.Events;
    using Khala.EventSourcing;

    public class Movie : EventSourced
    {
        private readonly List<Screening> _screenings;

        public Movie(Guid movieId, string title)
            : this(movieId)
        {
            RaiseEvent(new MovieCreated { Title = title });
        }

        private Movie(Guid movieId)
            : base(movieId)
        {
            _screenings = new List<Screening>();
        }

        public string Title { get; private set; }

        public IEnumerable<Screening> Screenings => _screenings;

        private void Handle(MovieCreated domainEvent)
        {
            Title = domainEvent.Title;
        }

        public static Movie Factory(
            Guid movieId, IEnumerable<IDomainEvent> pastEvents)
        {
            var movie = new Movie(movieId);
            movie.HandlePastEvents(pastEvents);
            return movie;
        }

        public void AddScreening(
            Guid screeningId,
            Guid theaterId,
            int seatRowCount,
            int seatColumnCount,
            DateTime screeningTime,
            decimal defaultFee,
            decimal childrenFee)
        {
            RaiseEvent(new ScreeningAdded
            {
                ScreeningId = screeningId,
                TheaterId = theaterId,
                SeatRowCount = seatRowCount,
                SeatColumnCount = seatColumnCount,
                ScreeningTime = screeningTime,
                DefaultFee = defaultFee,
                ChildrenFee = childrenFee,
            });
        }

        private void Handle(ScreeningAdded domainEvent)
        {
            _screenings.Add(Screening.Create(
                domainEvent.ScreeningId,
                domainEvent.TheaterId,
                domainEvent.SeatRowCount,
                domainEvent.SeatColumnCount,
                domainEvent.ScreeningTime,
                domainEvent.DefaultFee,
                domainEvent.ChildrenFee));
        }
    }
}
