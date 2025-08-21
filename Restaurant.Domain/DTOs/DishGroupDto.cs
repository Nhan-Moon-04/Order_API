namespace Restaurant.Domain.DTOs
{
    public class DishGroupDto
    {
        public required string GroupId { get; set; }
        public required string GroupName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
