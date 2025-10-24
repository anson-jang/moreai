using Microsoft.AspNetCore.Mvc;

namespace Smartadmin.Controllers
    {
        public class DashboardController : Controller
        {

        public IActionResult Index()
        {
            return View();
        }
        }
    }