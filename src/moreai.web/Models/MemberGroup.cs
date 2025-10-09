using System;
using System.ComponentModel.DataAnnotations;

namespace moreai.web.Models
{
    public class MemberGroup
    {
        [Required]
        public int MemberId { get; set; }

        [Required]
        public int GroupId { get; set; }

        public DateTime JoinedAt { get; set; }
        public DateTime? LeftAt { get; set; }

        public virtual Member Member { get; set; } = null!;
        public virtual Group Group { get; set; } = null!;
    }
}