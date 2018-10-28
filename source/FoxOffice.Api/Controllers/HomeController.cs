namespace FoxOffice.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    [Route("/")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index() => new RedirectResult("/swagger");
    }
}
