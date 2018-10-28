namespace FoxOffice.Admin.Models
{
    using FoxOffice.Commands;

    public class CreateMovieViewModel
    {
        public string Title { get; set; } = string.Empty;

        internal CreateNewMovie CreateCommand() => new CreateNewMovie
        {
            Title = Title,
        };
    }
}
