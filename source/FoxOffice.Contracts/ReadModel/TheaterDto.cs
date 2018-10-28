namespace FoxOffice.ReadModel
{
    using System;

    public class TheaterDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int SeatRowCount { get; set; }

        public int SeatColumnCount { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
