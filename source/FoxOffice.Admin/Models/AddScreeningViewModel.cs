namespace FoxOffice.Admin.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using FoxOffice.Commands;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class AddScreeningViewModel
    {
        public Guid MovieId { get; set; }

        [Required]
        [Display(Name = "Theater")]
        public Guid TheaterId { get; set; }

        public List<SelectListItem> Theaters { get; set; }

        [Display(Name = "Screening Time")]
        public DateTime ScreeningTime { get; set; }

        [Display(Name = "Default Fee")]
        public decimal DefaultFee { get; set; }

        [Display(Name = "Children Fee")]
        public decimal ChildrenFee { get; set; }

        internal AddNewScreening CreateCommand() => new AddNewScreening
        {
            MovieId = MovieId,
            TheaterId = TheaterId,
            ScreeningTime = ScreeningTime,
            DefaultFee = DefaultFee,
            ChildrenFee = ChildrenFee,
        };
    }
}
