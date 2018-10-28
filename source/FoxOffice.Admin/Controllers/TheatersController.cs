namespace FoxOffice.Admin.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using FoxOffice.Admin.Models;
    using FoxOffice.Admin.Services;
    using FoxOffice.Commands;
    using FoxOffice.ReadModel;
    using Microsoft.AspNetCore.Mvc;

    public class TheatersController : Controller
    {
        public async Task<ActionResult> Index(
            [FromServices] IGetAllTheatersService service)
        {
            IEnumerable<TheaterDto> theaters = await service.GetAllTheaters();
            return View(TheaterViewModel.TranslateRange(theaters));
        }

        public ActionResult Create() => View(new CreateTheaterViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(
            [FromForm] CreateTheaterViewModel model,
            [FromServices] ISendCreateTheaterCommandService service)
        {
            CreateNewTheater command = model.CreateCommand();

            IResult<TheaterLocation> result = await
                service.SendCreateTheaterCommand(command);

            switch (result)
            {
                case Success<TheaterLocation> success:
                    return RedirectToAction(nameof(Index));

                case Error<TheaterLocation> error:
                    ModelState.AddModelError(string.Empty, error.Message);
                    break;
            }

            return View(model);
        }
    }
}
