using System.ComponentModel.DataAnnotations;

namespace moreai.web.Models
{
    public class Group
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<MemberGroup> Members { get; set; } = new List<MemberGroup>();

        public bool IsAdminGroup => Name == "Admin";
    }
}