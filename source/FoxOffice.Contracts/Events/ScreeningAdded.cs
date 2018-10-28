namespace FoxOffice.Events
{
    using System;
    using Khala.EventSourcing;

    public class ScreeningAdded : DomainEvent
    {
        public Guid MovieId => SourceId;

        public Guid ScreeningId { get; set; }

        public Guid TheaterId { get; set; }

        public int SeatRowCount { get; set; }

        public int SeatColumnCount { get; set; }

        public DateTime ScreeningTime { get; set; }

        public decimal DefaultFee { get; set; }

        public decimal ChildrenFee { get; set; }
    }
}
