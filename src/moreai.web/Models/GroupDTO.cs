namespace moreai.web.Models
{
    public class GroupDTO
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
    }

    public class GroupUpdateDTO
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}