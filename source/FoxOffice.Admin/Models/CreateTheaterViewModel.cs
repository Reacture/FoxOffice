namespace FoxOffice.Admin.Models
{
    using FoxOffice.Commands;

    public class CreateTheaterViewModel
    {
        public string Name { get; set; } = string.Empty;

        public int SeatRowCount { get; set; } = 1;

        public int SeatColumnCount { get; set; } = 1;

        public CreateNewTheater CreateCommand() => new CreateNewTheater
        {
            Name = Name,
            SeatRowCount = SeatRowCount,
            SeatColumnCount = SeatColumnCount,
        };
    }
}
