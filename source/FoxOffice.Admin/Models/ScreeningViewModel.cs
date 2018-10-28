namespace FoxOffice.Admin.Models
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using FoxOffice.ReadModel;

    public class ScreeningViewModel
    {
        public Guid Id { get; set; }

        public Guid TheaterId { get; set; }

        [Display(Name = "Theater")]
        public string TheaterName { get; set; }

        [Display(Name = "Screening Time")]
        public DateTime ScreeningTime { get; set; }

        [Display(Name = "Default Fee")]
        public decimal DefaultFee { get; set; }

        [Display(Name = "Children Fee")]
        public decimal ChildrenFee { get; set; }

        public DateTime CreatedAt { get; set; }

        public static ScreeningViewModel Translate(ScreeningDto source)
        {
            return source == null ? default : new ScreeningViewModel
            {
                Id = source.Id,
                TheaterId = source.TheaterId,
                TheaterName = source.TheaterName,
                ScreeningTime = source.ScreeningTime,
                DefaultFee = source.DefaultFee,
                ChildrenFee = source.ChildrenFee,
                CreatedAt = source.CreatedAt,
            };
        }

        public static ImmutableArray<ScreeningViewModel> Translate(
            IEnumerable<ScreeningDto> source)
        {
            return ImmutableArray.CreateRange(source.Select(Translate));
        }
    }
}
