namespace FoxOffice.Controllers
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using FoxOffice.Commands;
    using Khala.Messaging;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/commands")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        [HttpPost(nameof(SendCreateTheaterCommand))]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        public async Task<IActionResult> SendCreateTheaterCommand(
            [FromBody] CreateNewTheater source,
            [FromServices] IMessageBus messageBus)
        {
            CreateTheater command = Translate(source);
            await messageBus.Send(new Envelope(command));
            return Accepted(uri: $"api/queries/Theaters/{command.TheaterId}");
        }

        private static CreateTheater Translate(CreateNewTheater source)
        {
            return new CreateTheater
            {
                TheaterId = Guid.NewGuid(),
                Name = source.Name,
                SeatRowCount = source.SeatRowCount,
                SeatColumnCount = source.SeatColumnCount,
            };
        }

        [HttpPost(nameof(SendCreateMovieCommand))]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        public async Task<IActionResult> SendCreateMovieCommand(
            [FromBody] CreateNewMovie source,
            [FromServices] IMessageBus messageBus)
        {
            CreateMovie command = Translate(source);
            await messageBus.Send(new Envelope(command));
            return Accepted(uri: $"api/queries/Movies/{command.MovieId}");
        }

        private static CreateMovie Translate(CreateNewMovie source)
        {
            return new CreateMovie
            {
                MovieId = Guid.NewGuid(),
                Title = source.Title,
            };
        }

        [HttpPost(nameof(SendAddScreeningCommand))]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        public async Task<IActionResult> SendAddScreeningCommand(
            [FromBody] AddNewScreening source,
            [FromServices] IMessageBus messageBus)
        {
            AddScreening command = Translate(source);
            await messageBus.Send(new Envelope(command));
            string uri = $"api/queries/Movies/{command.MovieId}/Screenings/{command.ScreeningId}";
            return Accepted(uri);
        }

        private static AddScreening Translate(AddNewScreening source)
        {
            return new AddScreening
            {
                MovieId = source.MovieId,
                ScreeningId = Guid.NewGuid(),
                TheaterId = source.TheaterId,
                ScreeningTime = source.ScreeningTime,
                DefaultFee = source.DefaultFee,
                ChildrenFee = source.ChildrenFee,
            };
        }
    }
}
