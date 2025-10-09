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
    public class GroupsController : ControllerBase
    {
        private readonly IGroupRepository _groupRepository;
        private readonly ITokenService _tokenService;

        public GroupsController(IGroupRepository groupRepository, ITokenService tokenService)
        {
            _groupRepository = groupRepository;
            _tokenService = tokenService;
        }

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

        [HttpGet]
        public async Task<IActionResult> GetAllGroups()
        {
            try
            {
                // 驗證用戶身份
                _ = GetCurrentUserId();

                var groups = await _groupRepository.GetAllAsync();
                return Ok(groups);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "未授權的訪問" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "獲取群組列表時發生錯誤", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGroup(int id)
        {
            try
            {
                // 驗證用戶身份
                _ = GetCurrentUserId();

                var group = await _groupRepository.GetByIdAsync(id);
                if (group == null)
                {
                    return NotFound(new { message = "找不到指定的群組" });
                }

                return Ok(group);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "未授權的訪問" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "獲取群組資料時發生錯誤", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] GroupDTO groupDto)
        {
            try
            {
                // 驗證用戶身份
                _ = GetCurrentUserId();

                // 檢查群組名稱是否已存在
                if (await _groupRepository.ExistsAsync(groupDto.Name))
                {
                    return BadRequest(new { message = "群組名稱已存在" });
                }

                var group = new Group
                {
                    Name = groupDto.Name,
                    Description = groupDto.Description,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                var groupId = await _groupRepository.CreateAsync(group);
                group.Id = groupId;

                return CreatedAtAction(nameof(GetGroup), new { id = group.Id }, group);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "未授權的訪問" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "建立群組時發生錯誤", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGroup(int id, [FromBody] GroupUpdateDTO updateDto)
        {
            try
            {
                // 驗證用戶身份
                _ = GetCurrentUserId();

                var group = await _groupRepository.GetByIdAsync(id);
                if (group == null)
                {
                    return NotFound(new { message = "找不到指定的群組" });
                }

                // 如果更改了名稱，檢查新名稱是否已存在
                if (updateDto.Name != group.Name && await _groupRepository.ExistsAsync(updateDto.Name))
                {
                    return BadRequest(new { message = "群組名稱已存在" });
                }

                group.Name = updateDto.Name;
                group.Description = updateDto.Description;
                group.IsActive = updateDto.IsActive;

                await _groupRepository.UpdateAsync(group);

                return Ok(group);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "未授權的訪問" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "更新群組時發生錯誤", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            try
            {
                // 驗證用戶身份
                _ = GetCurrentUserId();

                var group = await _groupRepository.GetByIdAsync(id);
                if (group == null)
                {
                    return NotFound(new { message = "找不到指定的群組" });
                }

                await _groupRepository.DeleteAsync(id);

                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "未授權的訪問" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "刪除群組時發生錯誤", error = ex.Message });
            }
        }

        [HttpGet("member/{memberId}")]
        public async Task<IActionResult> GetMemberGroups(int memberId)
        {
            try
            {
                // 驗證用戶身份
                var currentUserId = GetCurrentUserId();
                if (currentUserId != memberId)
                {
                    return Forbid();
                }

                var groups = await _groupRepository.GetMemberGroupsAsync(memberId);
                return Ok(groups);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "未授權的訪問" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "獲取會員群組時發生錯誤", error = ex.Message });
            }
        }

        [HttpPost("member/{memberId}/groups/{groupId}")]
        public async Task<IActionResult> AddMemberToGroup(int memberId, int groupId)
        {
            try
            {
                // 驗證用戶身份
                _ = GetCurrentUserId();

                // 檢查群組是否存在
                var group = await _groupRepository.GetByIdAsync(groupId);
                if (group == null)
                {
                    return NotFound(new { message = "找不到指定的群組" });
                }

                await _groupRepository.AddMemberToGroupAsync(memberId, groupId);
                return Ok(new { message = "成功將會員加入群組" });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "未授權的訪問" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "將會員加入群組時發生錯誤", error = ex.Message });
            }
        }

        [HttpDelete("member/{memberId}/groups/{groupId}")]
        public async Task<IActionResult> RemoveMemberFromGroup(int memberId, int groupId)
        {
            try
            {
                // 驗證用戶身份
                _ = GetCurrentUserId();

                // 檢查群組是否存在
                var group = await _groupRepository.GetByIdAsync(groupId);
                if (group == null)
                {
                    return NotFound(new { message = "找不到指定的群組" });
                }

                await _groupRepository.RemoveMemberFromGroupAsync(memberId, groupId);
                return Ok(new { message = "成功將會員從群組中移除" });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "未授權的訪問" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "將會員從群組移除時發生錯誤", error = ex.Message });
            }
        }
    }
}