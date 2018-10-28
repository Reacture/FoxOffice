namespace FoxOffice.Events
{
    using System;
    using Khala.EventSourcing;

    public class MovieCreated : DomainEvent
    {
        public Guid MovieId => SourceId;

        public string Title { get; set; }
    }
}
