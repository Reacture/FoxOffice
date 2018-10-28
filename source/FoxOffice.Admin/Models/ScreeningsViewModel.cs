namespace FoxOffice.Admin.Models
{
    using System;
    using System.Collections.Generic;

    public class ScreeningsViewModel
    {
        public Guid MovieId { get; set; }

        public IReadOnlyList<ScreeningViewModel> Screenings { get; set; }
    }
}
