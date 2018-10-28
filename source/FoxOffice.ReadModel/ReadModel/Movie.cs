namespace FoxOffice.ReadModel
{
    using System;
    using System.Collections.Generic;

    public class Movie : Entity
    {
        public string Title { get; set; }

        public List<Screening> Screenings { get; set; } = new List<Screening>();

        public DateTime CreatedAt { get; set; }
    }
}
