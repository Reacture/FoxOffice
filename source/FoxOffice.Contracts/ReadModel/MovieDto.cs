namespace FoxOffice.ReadModel
{
    using System;
    using System.Collections.Generic;

    public class MovieDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public List<ScreeningDto> Screenings { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
