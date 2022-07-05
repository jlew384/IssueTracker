using Microsoft.AspNetCore.Mvc;

namespace IssueTracker.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
