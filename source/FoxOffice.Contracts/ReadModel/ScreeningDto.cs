namespace FoxOffice.ReadModel
{
    using System;
    using System.Collections.Generic;

    public class ScreeningDto
    {
        public Guid Id { get; set; }

        public Guid TheaterId { get; set; }

        public string TheaterName { get; set; }

        public List<Seat> Seats { get; set; }

        public DateTime ScreeningTime { get; set; }

        public decimal DefaultFee { get; set; }

        public decimal ChildrenFee { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
