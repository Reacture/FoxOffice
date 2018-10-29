namespace FoxOffice.Admin.Models
{
    using System;
    using System.Collections.Generic;
    using FoxOffice.ReadModel;

    public class ScreeningsViewModel
    {
        public Guid MovieId { get; set; }

        public string MovieTitle { get; set; }

        public IReadOnlyList<ScreeningViewModel> Screenings { get; set; }

        internal static ScreeningsViewModel Translate(MovieDto source)
        {
            return new ScreeningsViewModel
            {
                MovieId = source.Id,
                MovieTitle = source.Title,
                Screenings = ScreeningViewModel.Translate(source.Screenings),
            };
        }
    }
}
