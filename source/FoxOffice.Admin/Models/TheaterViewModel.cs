namespace FoxOffice.Admin.Models
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using FoxOffice.ReadModel;

    public class TheaterViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int SeatRowCount { get; set; }

        public int SeatColumnCount { get; set; }

        public DateTime CreatedAt { get; set; }

        internal static TheaterViewModel Translate(TheaterDto source)
        {
            return new TheaterViewModel
            {
                Id = source.Id,
                Name = source.Name,
                SeatRowCount = source.SeatRowCount,
                SeatColumnCount = source.SeatColumnCount,
                CreatedAt = source.CreatedAt,
            };
        }

        internal static ImmutableArray<TheaterViewModel> TranslateRange(
            IEnumerable<TheaterDto> source)
        {
            return source.Select(Translate).ToImmutableArray();
        }
    }
}
