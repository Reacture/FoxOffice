namespace FoxOffice.Commands
{
    using System;

    public class AddScreening
    {
        public Guid MovieId { get; set; }

        public Guid ScreeningId { get; set; }

        public Guid TheaterId { get; set; }

        public DateTime ScreeningTime { get; set; }

        public decimal DefaultFee { get; set; }

        public decimal ChildrenFee { get; set; }
    }
}
