namespace FoxOffice.Events
{
    using System;
    using Khala.EventSourcing;

    public class TheaterCreated : DomainEvent
    {
        public Guid TheaterId => SourceId;

        public string Name { get; set; }

        public int SeatRowCount { get; set; }

        public int SeatColumnCount { get; set; }
    }
}
