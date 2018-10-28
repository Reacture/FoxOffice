namespace FoxOffice.ReadModel
{
    using System;
    using System.Collections.Generic;

    public class Screening : Entity
    {
        public Guid TheaterId { get; set; }

        public string TheaterName { get; set; }

        public List<Seat> Seats { get; set; } = new List<Seat>();

        public DateTime ScreeningTime { get; set; }

        public decimal DefaultFee { get; set; }

        public decimal ChildrenFee { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
