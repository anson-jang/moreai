using Microsoft.AspNetCore.Mvc;

namespace Smartadmin.Controllers
    {
        public class ErrorController : Controller
        {
            public IActionResult Error404()
        {
            return View();
        }

        public IActionResult Error4042()
        {
            return View();
        }

        public IActionResult Error500()
        {
            return View();
        }
        }
    }