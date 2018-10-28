namespace FoxOffice.Commands
{
    using System;

    public class CreateTheater
    {
        public Guid TheaterId { get; set; }

        public string Name { get; set; }

        public int SeatRowCount { get; set; }

        public int SeatColumnCount { get; set; }
    }
}
