using Microsoft.AspNetCore.Mvc;

namespace Smartadmin.Controllers
    {
        public class ForumController : Controller
        {
            public IActionResult Discussion()
        {
            return View();
        }

        public IActionResult Threads()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Search()
        {
            return View();
        }
        }
    }