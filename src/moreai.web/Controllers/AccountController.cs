using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using moreai.Models;
using moreai.web.Data;
using moreai.web.Helpers;
using moreai.web.Models;
using moreai.web.Services;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;

namespace moreai.web.Controllers;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AccountController> _logger;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public AccountController(
        ApplicationDbContext context,
        ILogger<AccountController> logger,
        IEmailService emailService,
        IConfiguration configuration)
    {
        _context = context;
        _logger = logger;
        _emailService = emailService;
        _configuration = configuration;
    }

    [AllowAnonymous]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (await _context.Members.AnyAsync(m => m.Username == model.Username))
            {
                ModelState.AddModelError("Username", "此使用者名稱已被使用");
                return View(model);
            }

            if (await _context.Members.AnyAsync(m => m.Email == model.Email))
            {
                ModelState.AddModelError("Email", "此電子郵件已被使用");
                return View(model);
            }

            PasswordHelper.CreatePasswordHash(model.Password, out string passwordHash, out string passwordSalt);

            var member = new Member
            {
                Username = model.Username,
                Email = model.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                FirstName = model.FirstName,
                LastName = model.LastName,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Members.Add(member);
            await _context.SaveChangesAsync();

            _logger.LogInformation("使用者 {Username} 已成功註冊", member.Username);

            // 自動登入新註冊的使用者
            await SignInUserAsync(member);

            return RedirectToAction("Index", "Home");
        }

        return View(model);
    }

    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _context.Members
                .Include(m => m.Groups)
                .ThenInclude(mg => mg.Group)
                .FirstOrDefaultAsync(m => m.Username == model.Username || m.Email == model.Username);

            if (user != null && PasswordHelper.VerifyPasswordHash(model.Password, user.PasswordHash, user.PasswordSalt))
            {
                // 檢查用戶狀態
                if (!user.IsActive)
                {
                    ModelState.AddModelError(string.Empty, "此帳號已被停用");
                    return View(model);
                }

                await SignInUserAsync(user, model.RememberMe, model.ReturnUrl);
                _logger.LogInformation("用戶 {Username} 登入成功，角色: {Roles}", 
                    user.Email, 
                    user.Role == "Admin" || user.Groups.Any(mg => mg.Group?.Name == "Admin" && mg.LeftAt == null) ? "Admin" : "User");

                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "帳號或密碼錯誤");
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("CookieAuth");
        return RedirectToAction("Index", "Home");
    }

    private async Task SignInUserAsync(Member user, bool isPersistent = false, string? returnUrl = null)
    {
        // 檢查用戶群組
        var userWithGroups = await _context.Members
            .Include(m => m.Groups)
            .ThenInclude(mg => mg.Group)
            .FirstOrDefaultAsync(m => m.Id == user.Id);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email), // 使用 Email 作為主要識別
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
        };

        // 檢查用戶是否為管理員
        if (userWithGroups?.Role == "Admin" || 
            userWithGroups?.Groups.Any(mg => mg.Group?.Name == "Admin" && mg.LeftAt == null) == true)
        {
            claims.Add(new Claim(ClaimTypes.Role, "Admin"));
        }
        else
        {
            claims.Add(new Claim(ClaimTypes.Role, "User"));
        }

        var identity = new ClaimsIdentity(claims, "CookieAuth");
        var principal = new ClaimsPrincipal(identity);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = isPersistent,
            RedirectUri = returnUrl
        };

        await HttpContext.SignInAsync("CookieAuth", principal, authProperties);

        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    [AllowAnonymous]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _context.Members.FirstOrDefaultAsync(m => m.Email == model.Email);
            if (user == null)
            {
                // 不要透露使用者不存在的訊息
                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            var token = GeneratePasswordResetToken();
            var tokenExpiry = DateTime.UtcNow.AddHours(24);

            user.PasswordResetToken = token;
            user.PasswordResetTokenExpiry = tokenExpiry;
            await _context.SaveChangesAsync();

            var resetLink = Url.Action("ResetPassword", "Account",
                new { email = model.Email, token = token },
                protocol: Request.Scheme);

            var emailBody = $@"
                <h2>重設密碼</h2>
                <p>請點擊下面的連結重設您的密碼：</p>
                <p><a href='{resetLink}'>重設密碼</a></p>
                <p>此連結將在24小時後失效。</p>
                <p>如果您沒有要求重設密碼，請忽略此郵件。</p>";

            await _emailService.SendEmailAsync(
                model.Email,
                "重設密碼",
                emailBody);

            return RedirectToAction(nameof(ForgotPasswordConfirmation));
        }

        return View(model);
    }

    [AllowAnonymous]
    public IActionResult ForgotPasswordConfirmation()
    {
        return View();
    }

    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword(string email, string token)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login");
        }

        var user = await _context.Members
            .FirstOrDefaultAsync(m => m.Email == email && m.PasswordResetToken == token);

        if (user == null || user.PasswordResetTokenExpiry < DateTime.UtcNow)
        {
            ModelState.AddModelError(string.Empty, "無效的密碼重設連結或連結已過期");
            return RedirectToAction("Login");
        }

        var model = new ResetPasswordViewModel
        {
            Email = email,
            Token = token
        };

        return View(model);
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _context.Members
            .FirstOrDefaultAsync(m => m.Email == model.Email && m.PasswordResetToken == model.Token);

        if (user == null || user.PasswordResetTokenExpiry < DateTime.UtcNow)
        {
            ModelState.AddModelError(string.Empty, "無效的密碼重設連結或連結已過期");
            return View(model);
        }

        PasswordHelper.CreatePasswordHash(model.NewPassword, out string passwordHash, out string passwordSalt);

        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiry = null;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(ResetPasswordConfirmation));
    }

    [AllowAnonymous]
    public IActionResult ResetPasswordConfirmation()
    {
        return View();
    }

    private string GeneratePasswordResetToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}