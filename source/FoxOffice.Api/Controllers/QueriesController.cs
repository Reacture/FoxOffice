namespace FoxOffice.Controllers
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using FoxOffice.ReadModel;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/queries")]
    [ApiController]
    public class QueriesController : ControllerBase
    {
        [HttpGet("Theaters")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(TheaterDto[]))]
        public async Task<IActionResult> GetAllTheaters(
            [FromServices] TheaterReadModelFacade readModelFacade)
        {
            return Ok(await readModelFacade.GetAllTheaters());
        }

        [HttpGet("Theaters/{theaterId}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(TheaterDto))]
        public async Task<IActionResult> FindTheater(
            [FromRoute] Guid theaterId,
            [FromServices] TheaterReadModelFacade readModelFacade)
        {
            TheaterDto theater = await readModelFacade.FindTheater(theaterId);
            return theater == null ? NotFound() : (IActionResult)Ok(theater);
        }

        [HttpGet("Movies")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(MovieDto[]))]
        public async Task<IActionResult> GetAllMovies(
            [FromServices] MovieReadModelFacade readModelFacade)
        {
            return Ok(await readModelFacade.GetAllMovies());
        }

        [HttpGet("Movies/{movieId}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(MovieDto))]
        public async Task<IActionResult> FindMovie(
            [FromRoute] Guid movieId,
            [FromServices] MovieReadModelFacade readModelFacade)
        {
            MovieDto movie = await readModelFacade.FindMovie(movieId);
            return movie == null ? NotFound() : (IActionResult)Ok(movie);
        }

        [HttpGet("Movies/{movieId}/Screenings/{screeningId}")]
        public async Task<IActionResult> FindScreening(
            [FromRoute] Guid movieId,
            [FromRoute] Guid screeningId,
            [FromServices] MovieReadModelFacade readModelFacade)
        {
            MovieDto movie = await readModelFacade.FindMovie(movieId);

            ScreeningDto screening = movie?
                .Screenings
                .SingleOrDefault(s => s.Id == screeningId);

            return screening == null
                ? NotFound()
                : (IActionResult)Ok(screening);
        }
    }
}
