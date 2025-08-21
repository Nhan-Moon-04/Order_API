namespace Restaurant.Domain.Entities
{
    public class DishGroup : BaseEntity
    {
        public required string GroupId { get; set; } 
        public required string GroupName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Dishes> Dishes { get; set; } = new List<Dishes>();
    }
}
