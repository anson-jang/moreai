using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using moreai.web.Data;
using moreai.web.Models;

namespace moreai.web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class GroupController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GroupController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GroupController(
            ApplicationDbContext context, 
            ILogger<GroupController> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        private async Task<bool> IsUserAdmin()
        {
            var userEmail = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail)) return false;

            var member = await _context.Members
                .Include(m => m.Groups)
                .ThenInclude(mg => mg.Group)
                .FirstOrDefaultAsync(m => m.Email == userEmail);

            return member?.IsAdmin ?? false;
        }

        public async Task<IActionResult> Index()
        {
            if (!await IsUserAdmin())
            {
                return Forbid();
            }

            var groups = await _context.Groups
                .Include(g => g.Members)
                .ToListAsync();
            return View(groups);
        }

        public async Task<IActionResult> Create()
        {
            if (!await IsUserAdmin())
            {
                return Forbid();
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description")] Group group)
        {
            if (!await IsUserAdmin())
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                group.CreatedAt = DateTime.UtcNow;
                group.IsActive = true;
                _context.Add(group);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(group);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (!await IsUserAdmin())
            {
                return Forbid();
            }

            if (id == null)
            {
                return NotFound();
            }

            var group = await _context.Groups.FindAsync(id);
            if (group == null)
            {
                return NotFound();
            }
            return View(group);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Group group)
        {
            if (!await IsUserAdmin())
            {
                return Forbid();
            }

            if (id != group.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingGroup = await _context.Groups.FindAsync(id);
                    if (existingGroup == null)
                    {
                        return NotFound();
                    }

                    existingGroup.Name = group.Name;
                    existingGroup.Description = group.Description;
                    existingGroup.UpdatedAt = DateTime.UtcNow;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupExists(group.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(group);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (!await IsUserAdmin())
            {
                return Forbid();
            }

            if (id == null)
            {
                return NotFound();
            }

            var group = await _context.Groups
                .FirstOrDefaultAsync(m => m.Id == id);
            if (group == null)
            {
                return NotFound();
            }

            return View(group);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!await IsUserAdmin())
            {
                return Forbid();
            }

            var group = await _context.Groups.FindAsync(id);
            if (group != null)
            {
                _context.Groups.Remove(group);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool GroupExists(int id)
        {
            return _context.Groups.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Members(int? id)
        {
            if (!await IsUserAdmin())
            {
                return Forbid();
            }

            if (id == null)
            {
                return NotFound();
            }

            var group = await _context.Groups
                .Include(g => g.Members)
                .ThenInclude(mg => mg.Member)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (group == null)
            {
                return NotFound();
            }

            var availableMembers = await _context.Members
                .Where(m => !m.Groups.Any(g => g.GroupId == id))
                .ToListAsync();

            ViewBag.AvailableMembers = availableMembers;
            return View(group);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMember(int groupId, int memberId)
        {
            if (!await IsUserAdmin())
            {
                return Forbid();
            }

            var memberGroup = new MemberGroup
            {
                GroupId = groupId,
                MemberId = memberId,
                JoinedAt = DateTime.UtcNow
            };

            _context.MemberGroups.Add(memberGroup);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Members), new { id = groupId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveMember(int groupId, int memberId)
        {
            if (!await IsUserAdmin())
            {
                return Forbid();
            }

            var memberGroup = await _context.MemberGroups
                .FirstOrDefaultAsync(mg => mg.GroupId == groupId && mg.MemberId == memberId);

            if (memberGroup != null)
            {
                _context.MemberGroups.Remove(memberGroup);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Members), new { id = groupId });
        }
    }
}