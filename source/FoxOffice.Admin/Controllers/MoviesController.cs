namespace FoxOffice.Admin.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FoxOffice.Admin.Models;
    using FoxOffice.Admin.Services;
    using FoxOffice.Commands;
    using FoxOffice.ReadModel;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;

    [Route("Movies")]
    public class MoviesController : Controller
    {
        [HttpGet]
        public async Task<ActionResult> Index(
            [FromServices] IGetAllMoviesService service)
        {
            IEnumerable<MovieDto> movies = await service.GetAllMovies();
            return View(MovieViewModel.TranslateRange(movies));
        }

        [HttpGet("Create")]
        public ActionResult Create() => View(new CreateMovieViewModel());

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(
            [FromForm] CreateMovieViewModel model,
            [FromServices] ISendCreateMovieCommandService service,
            [FromServices] IResourceAwaiter awaiter)
        {
            CreateNewMovie command = model.CreateCommand();

            IResult<MovieLocation> result = await
                service.SendCreateMovieCommand(command);

            switch (result)
            {
                case Success<MovieLocation> success:
                    await awaiter.AwaitResource(success.Value.Uri);
                    return RedirectToAction(nameof(Index));

                case Error<MovieLocation> error:
                    ModelState.AddModelError(string.Empty, error.Message);
                    break;
            }

            return View(model);
        }

        [HttpGet("Screenings/{movieId}")]
        public async Task<ActionResult> Screenings(
            [FromServices] IFindMovieService service, [FromRoute] Guid movieId)
        {
            MovieDto movie = await service.FindMovie(movieId);
            return View(new ScreeningsViewModel
            {
                MovieId = movieId,
                Screenings = ScreeningViewModel.Translate(movie.Screenings),
            });
        }

        [HttpGet("AddScreening/{movieId}")]
        public async Task<ActionResult> AddScreening(
            [FromServices] IGetAllTheatersService service,
            [FromRoute] Guid movieId)
        {
            return View(new AddScreeningViewModel
            {
                MovieId = movieId,
                Theaters = new List<SelectListItem>(
                    from t in await service.GetAllTheaters()
                    select new SelectListItem
                    {
                        Value = $"{t.Id}",
                        Text = t.Name,
                    }),
                ScreeningTime = DateTime.Today.AddDays(2),
            });
        }

        [HttpPost("AddScreening/{movieId}")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddScreening(
            [FromRoute] Guid movieId,
            [FromForm] AddScreeningViewModel model,
            [FromServices] ISendAddScreeningCommandService service,
            [FromServices] IResourceAwaiter awaiter)
        {
            AddNewScreening command = model.CreateCommand();

            IResult<ScreeningLocation> result = await
                service.SendAddScreeningCommand(command);

            switch (result)
            {
                case Success<ScreeningLocation> success:
                    await awaiter.AwaitResource(success.Value.Uri);
                    var routeValues = new { movieId };
                    return RedirectToAction("Screenings", routeValues);

                case Error<ScreeningLocation> error:
                    ModelState.AddModelError(string.Empty, error.Message);
                    break;
            }

            return View(model);
        }
    }
}
