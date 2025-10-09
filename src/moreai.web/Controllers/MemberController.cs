using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using moreai.Models;
using moreai.web.Data;

namespace moreai.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MemberController> _logger;

        public MemberController(ApplicationDbContext context, ILogger<MemberController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Member/Profile
        public async Task<IActionResult> Profile()
        {
            var userEmail = User.Identity.Name;
            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.Email == userEmail);

            if (member == null)
            {
                return NotFound();
            }

            var viewModel = new MemberProfileViewModel
            {
                Phone = member.Phone,
                Address = member.Address,
                CompanyName = member.CompanyName
            };

            return View(viewModel);
        }

        // POST: Member/Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(MemberProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userEmail = User.Identity.Name;
            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.Email == userEmail);

            if (member == null)
            {
                return NotFound();
            }

            member.Phone = model.Phone;
            member.Address = model.Address;
            member.CompanyName = model.CompanyName;

            try
            {
                await _context.SaveChangesAsync();
                TempData["Message"] = "會員資料更新成功！";
                return RedirectToAction(nameof(Profile));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新會員資料時發生錯誤");
                ModelState.AddModelError("", "更新會員資料時發生錯誤，請稍後再試。");
                return View(model);
            }
        }
    }
}