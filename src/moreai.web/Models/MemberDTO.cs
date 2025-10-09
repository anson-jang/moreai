namespace moreai.web.Models
{
    public class MemberDTO
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Password { get; set; }
    }

    public class MemberLoginDTO
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }

    public class MemberUpdateDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
    }
}