namespace FoxOffice.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Screening
    {
        public Guid Id { get; set; }

        public Guid TheaterId { get; set; }

        public List<Seat> Seats { get; set; }

        public DateTime ScreeningTime { get; set; }

        public decimal DefaultFee { get; set; }

        public decimal ChildrenFee { get; set; }

        internal static Screening Create(
            Guid id,
            Guid theaterId,
            int seatRowCount,
            int seatColumnCount,
            DateTime screeningTime,
            decimal defaultFee,
            decimal childrenFee)
        {
            return new Screening
            {
                Id = id,
                TheaterId = theaterId,
                Seats = new List<Seat>(
                    from r in Enumerable.Range(0, seatRowCount)
                    from c in Enumerable.Range(0, seatColumnCount)
                    select new Seat(row: r, column: c, isReserved: false)),
                ScreeningTime = screeningTime,
                DefaultFee = defaultFee,
                ChildrenFee = childrenFee,
            };
        }
    }
}
