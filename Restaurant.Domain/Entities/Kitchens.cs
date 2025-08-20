namespace Restaurant.Domain.Entities
{
    public class Kitchens : BaseEntity
    {
        public required string KitchenId { get; set; }
        public required string KitchenName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public virtual ICollection<Dishes> Dishes { get; set; } = new List<Dishes>();
    }
}
