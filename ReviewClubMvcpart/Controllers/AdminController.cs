using Microsoft.AspNetCore.Mvc;

namespace ReviewClubMvcpart.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
