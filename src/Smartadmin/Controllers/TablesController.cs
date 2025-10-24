using Microsoft.AspNetCore.Mvc;

namespace Smartadmin.Controllers
    {
        public class TablesController : Controller
        {
            public IActionResult Basic()
        {
            return View();
        }

        public IActionResult StyleGenerator()
        {
            return View();
        }
        }
    }