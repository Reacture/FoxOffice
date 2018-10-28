namespace FoxOffice.Domain
{
    using System;
    using System.Collections.Generic;
    using FoxOffice.Events;
    using Khala.EventSourcing;

    public class Theater : EventSourced
    {
        public Theater(
            Guid theaterId, string name, int seatRowCount, int seatColumnCount)
            : this(theaterId)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (seatRowCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(seatRowCount));
            }

            if (seatColumnCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(seatColumnCount));
            }

            RaiseEvent(new TheaterCreated
            {
                Name = name,
                SeatRowCount = seatRowCount,
                SeatColumnCount = seatColumnCount,
            });
        }

        private Theater(Guid theaterId)
            : base(theaterId)
        {
        }

        public int SeatRowCount { get; private set; }

        public int SeatColumnCount { get; private set; }

        private void Handle(TheaterCreated domainEvent)
        {
            SeatRowCount = domainEvent.SeatRowCount;
            SeatColumnCount = domainEvent.SeatColumnCount;
        }

        public static Theater Factory(
            Guid theaterId, IEnumerable<IDomainEvent> pastEvents)
        {
            var theater = new Theater(theaterId);
            theater.HandlePastEvents(pastEvents);
            return theater;
        }
    }
}
