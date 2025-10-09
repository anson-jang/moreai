using System;
using System.ComponentModel.DataAnnotations;

namespace moreai.web.Models
{
    public class Member
    {
        public int Id { get; set; }
        
        [Required]
        public required string Username { get; set; }
        
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        
        [Required]
        public required string PasswordHash { get; set; }
        
        [Required]
        public required string PasswordSalt { get; set; }
        
        [Required]
        public required string FirstName { get; set; }
        
        [Required]
        public required string LastName { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }
        public string Role { get; set; } = "User"; // 可以是 "Admin" 或 "User"
        
        // 會員資料維護欄位
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? CompanyName { get; set; }

        public virtual ICollection<MemberGroup> Groups { get; set; } = new List<MemberGroup>();

        public bool IsAdmin => Role == "Admin" || Groups.Any(mg => mg.Group?.IsAdminGroup == true && mg.LeftAt == null);
    }
}