using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using moreai.web.Models;
using moreai.web.Services;
using moreai.web.Repositories;

namespace moreai.web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MembersController : ControllerBase
    {
        private readonly IMemberRepository _memberRepository;
        private readonly IPasswordService _passwordService;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;

        // 自定義授權屬性
        private int GetCurrentUserId()
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                throw new UnauthorizedAccessException("未提供有效的授權標頭");
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            var userId = _tokenService.GetUserIdFromToken(token);
            if (!userId.HasValue)
            {
                throw new UnauthorizedAccessException("無效的 Token");
            }

            return userId.Value;
        }

        public MembersController(
            IMemberRepository memberRepository,
            IPasswordService passwordService,
            ITokenService tokenService,
            IEmailService emailService)
        {
            _memberRepository = memberRepository;
            _passwordService = passwordService;
            _tokenService = tokenService;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] MemberDTO memberDto)
        {
            try
            {
                // 檢查使用者名稱是否已存在
                if (await _memberRepository.ExistsAsync(memberDto.Username))
                {
                    return BadRequest(new { message = "使用者名稱已被使用" });
                }

                // 檢查 Email 是否已存在
                if (await _memberRepository.ExistsEmailAsync(memberDto.Email))
                {
                    return BadRequest(new { message = "Email 已被使用" });
                }

                // 建立密碼雜湊
                _passwordService.CreatePasswordHash(memberDto.Password, out string passwordHash, out string passwordSalt);

                // 建立新會員
                var member = new Member
                {
                    Username = memberDto.Username,
                    Email = memberDto.Email,
                    FirstName = memberDto.FirstName,
                    LastName = memberDto.LastName,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                // 儲存會員資料
                var memberId = await _memberRepository.CreateAsync(member);
                member.Id = memberId;

                // 產生 JWT Token
                var token = _tokenService.GenerateToken(member);

                // 發送歡迎郵件
                await _emailService.SendWelcomeEmailAsync(member.Email, member.Username);

                return Ok(new
                {
                    token,
                    user = new
                    {
                        id = member.Id,
                        username = member.Username,
                        email = member.Email,
                        firstName = member.FirstName,
                        lastName = member.LastName
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "註冊過程中發生錯誤", error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] MemberLoginDTO loginDto)
        {
            try
            {
                // 取得會員資料
                var member = await _memberRepository.GetByUsernameAsync(loginDto.Username);
                if (member == null)
                {
                    return BadRequest(new { message = "使用者名稱或密碼錯誤" });
                }

                // 驗證密碼
                if (!_passwordService.VerifyPasswordHash(loginDto.Password, member.PasswordHash, member.PasswordSalt))
                {
                    return BadRequest(new { message = "使用者名稱或密碼錯誤" });
                }

                // 更新最後登入時間
                member.LastLoginAt = DateTime.UtcNow;
                await _memberRepository.UpdateAsync(member);

                // 產生 JWT Token
                var token = _tokenService.GenerateToken(member);

                return Ok(new
                {
                    token,
                    user = new
                    {
                        id = member.Id,
                        username = member.Username,
                        email = member.Email,
                        firstName = member.FirstName,
                        lastName = member.LastName
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "登入過程中發生錯誤", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMember(int id)
        {
            try
            {
                // 檢查當前使用者是否有權限訪問此資料
                var currentUserId = GetCurrentUserId();
                if (currentUserId != id)
                {
                    return Forbid();
                }

                var member = await _memberRepository.GetByIdAsync(id);
                if (member == null)
                {
                    return NotFound(new { message = "找不到指定的會員" });
                }

                return Ok(new
                {
                    id = member.Id,
                    username = member.Username,
                    email = member.Email,
                    firstName = member.FirstName,
                    lastName = member.LastName,
                    createdAt = member.CreatedAt,
                    lastLoginAt = member.LastLoginAt
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "未授權的訪問" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "獲取會員資料時發生錯誤", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMember(int id, [FromBody] MemberUpdateDTO updateDto)
        {
            try
            {
                // 檢查當前使用者是否有權限更新此資料
                var currentUserId = GetCurrentUserId();
                if (currentUserId != id)
                {
                    return Forbid();
                }

                var member = await _memberRepository.GetByIdAsync(id);
                if (member == null)
                {
                    return NotFound(new { message = "找不到指定的會員" });
                }

                // 如果要更改密碼，需要驗證當前密碼
                if (!string.IsNullOrEmpty(updateDto.NewPassword))
                {
                    if (string.IsNullOrEmpty(updateDto.CurrentPassword))
                    {
                        return BadRequest(new { message = "必須提供當前密碼" });
                    }

                    if (!_passwordService.VerifyPasswordHash(updateDto.CurrentPassword, member.PasswordHash, member.PasswordSalt))
                    {
                        return BadRequest(new { message = "當前密碼錯誤" });
                    }

                    // 建立新的密碼雜湊
                    _passwordService.CreatePasswordHash(updateDto.NewPassword, out string newPasswordHash, out string newPasswordSalt);
                    member.PasswordHash = newPasswordHash;
                    member.PasswordSalt = newPasswordSalt;
                }

                // 更新其他資料
                if (!string.IsNullOrEmpty(updateDto.Email) && updateDto.Email != member.Email)
                {
                    if (await _memberRepository.ExistsEmailAsync(updateDto.Email))
                    {
                        return BadRequest(new { message = "此 Email 已被使用" });
                    }
                    member.Email = updateDto.Email;
                }

                if (!string.IsNullOrEmpty(updateDto.FirstName))
                {
                    member.FirstName = updateDto.FirstName;
                }

                if (!string.IsNullOrEmpty(updateDto.LastName))
                {
                    member.LastName = updateDto.LastName;
                }

                await _memberRepository.UpdateAsync(member);

                return Ok(new
                {
                    id = member.Id,
                    username = member.Username,
                    email = member.Email,
                    firstName = member.FirstName,
                    lastName = member.LastName
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "未授權的訪問" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "更新會員資料時發生錯誤", error = ex.Message });
            }
        }
    }
}