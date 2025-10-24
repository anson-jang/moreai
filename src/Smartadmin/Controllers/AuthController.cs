using Microsoft.AspNetCore.Mvc;

namespace Smartadmin.Controllers
    {
        public class AuthController : Controller
        {
            public IActionResult Twofactor()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Forgetpassword()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Lockscreen()
        {
            return View();
        }
        }
    }