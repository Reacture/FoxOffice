namespace FoxOffice
{
    public readonly struct Seat
    {
        public Seat(int row, int column, bool isReserved)
        {
            Row = row;
            Column = column;
            IsReserved = isReserved;
        }

        public int Row { get; }

        public int Column { get; }

        public bool IsReserved { get; }
    }
}
