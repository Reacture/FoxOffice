namespace FoxOffice.Admin.Models
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using FoxOffice.ReadModel;

    public class MovieViewModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        [Display(Name = "Created Time")]
        public DateTime CreatedAt { get; set; }

        internal static MovieViewModel Translate(MovieDto source)
        {
            return new MovieViewModel
            {
                Id = source.Id,
                Title = source.Title,
                CreatedAt = source.CreatedAt,
            };
        }

        internal static ImmutableArray<MovieViewModel> TranslateRange(
            IEnumerable<MovieDto> source)
        {
            return source.Select(Translate).ToImmutableArray();
        }
    }
}
